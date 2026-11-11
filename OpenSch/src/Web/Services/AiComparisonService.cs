using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenSch.Application.Services;

namespace OpenSch.Web.Services;

public interface IAiComparisonService
{
    Task<AiComparisonResult> CompareAsync(IReadOnlyList<SchoolDto> schools, CancellationToken cancellationToken);
}

public sealed class AiComparisonOptions
{
    public const string SectionName = "AiComparison";

    public bool Enabled { get; init; }
    public string? Endpoint { get; init; }
    public string? Model { get; init; }
    public string? ApiKey { get; init; }
}

/// <summary>
/// Produces a comparison from canonical server data. It can call an OpenAI-compatible
/// chat-completions endpoint when configured, and deliberately returns a deterministic
/// result when no provider is configured so the feature remains honest and usable.
/// </summary>
public sealed class AiComparisonService(
    IHttpClientFactory httpClientFactory,
    IOptions<AiComparisonOptions> options,
    ILogger<AiComparisonService> logger) : IAiComparisonService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private const string AiWarning = "Ringkasan ini dibuat oleh AI dan dapat keliru, tidak lengkap, atau tidak sesuai konteks. Periksa kembali angka sumber dan kunjungi sekolah sebelum mengambil keputusan.";

    public async Task<AiComparisonResult> CompareAsync(IReadOnlyList<SchoolDto> schools, CancellationToken cancellationToken)
    {
        var payload = CreatePayload(schools);
        var inputJson = JsonSerializer.Serialize(payload, JsonOptions);
        var config = options.Value;

        if (config.Enabled && Uri.TryCreate(config.Endpoint, UriKind.Absolute, out var endpoint) &&
            !string.IsNullOrWhiteSpace(config.ApiKey) && !string.IsNullOrWhiteSpace(config.Model))
        {
            try
            {
                var narrative = await RequestNarrativeAsync(endpoint, config, inputJson, cancellationToken);
                return new AiComparisonResult(
                    "ai",
                    narrative,
                    [AiWarning, "Data yang dikirim ke AI dibatasi pada metrik sekolah sintetis yang terlihat di bawah."],
                    payload);
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
            {
                logger.LogWarning(exception, "AI comparison provider was unavailable; returning the deterministic comparison.");
            }
        }

        return new AiComparisonResult(
            "deterministic-fallback",
            CreateDeterministicNarrative(schools),
            [
                AiWarning,
                "Penyedia AI belum dikonfigurasi atau tidak tersedia. Sistem menampilkan ringkasan deterministik dari angka yang sama, bukan jawaban AI.",
            ],
            payload);
    }

    private async Task<AiNarrative> RequestNarrativeAsync(Uri endpoint, AiComparisonOptions config, string inputJson, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(new
            {
                model = config.Model,
                temperature = 0.2,
                response_format = new { type = "json_object" },
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Anda adalah asisten perbandingan sekolah. Gunakan hanya JSON yang diberikan. Jangan mengarang fakta, peringkat, kualitas, atau rekomendasi. Balas JSON valid dengan properti summary, keyFindings (array), dan considerations (array). Selalu ingatkan pengguna untuk memverifikasi data.",
                    },
                    new { role = "user", content = inputJson },
                },
            }, options: JsonOptions),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);

        var client = httpClientFactory.CreateClient(nameof(AiComparisonService));
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var content = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new JsonException("The AI provider returned an empty response.");
        }

        var narrative = JsonSerializer.Deserialize<AiNarrative>(content, JsonOptions);
        return narrative is { Summary.Length: > 0 } ? narrative : throw new JsonException("The AI provider returned an invalid comparison schema.");
    }

    private static AiComparisonPayload CreatePayload(IReadOnlyList<SchoolDto> schools) => new(
        "Compare the supplied schools using only the supplied indicators. Do not decide which school is best.",
        "synthetic-demo-data",
        schools.Select(s => new AiSchoolData(s.Id, s.Name, s.City, s.District, s.Level, s.LiteracyScore, s.NumeracyScore, s.CompositeScore, s.Rank)).ToList(),
        new AiComparisonRules(
            "compositeScore = round((literacyScore + numeracyScore) / 2, 1)",
            "Only compare ranks directly for the same education level and city.",
            "Do not infer teaching quality, safety, cost, facilities, admissions, or student suitability from these metrics."));

    private static AiNarrative CreateDeterministicNarrative(IReadOnlyList<SchoolDto> schools)
    {
        var literacyLeader = schools.MaxBy(s => s.LiteracyScore)!;
        var numeracyLeader = schools.MaxBy(s => s.NumeracyScore)!;
        var compositeLeader = schools.MaxBy(s => s.CompositeScore)!;
        var sameLevelAndCity = schools.Select(s => (s.Level, s.City)).Distinct().Count() == 1;

        return new AiNarrative(
            $"Dari {schools.Count} sekolah yang dipilih, {compositeLeader.Name} memiliki skor komposit tertinggi ({compositeLeader.CompositeScore}).",
            [
                $"Literasi tertinggi: {literacyLeader.Name} ({literacyLeader.LiteracyScore}).",
                $"Numerasi tertinggi: {numeracyLeader.Name} ({numeracyLeader.NumeracyScore}).",
                $"Skor komposit menggunakan rumus rata-rata sederhana literasi dan numerasi.",
            ],
            [
                sameLevelAndCity
                    ? "Sekolah berada pada jenjang dan kota yang sama, sehingga perbandingan angka lebih sebanding."
                    : "Sekolah lintas jenjang atau kota terdeteksi; jangan menyamakan peringkat secara langsung.",
                "Metrik ini tidak mencakup biaya, fasilitas, kebutuhan siswa, keamanan, atau proses penerimaan.",
            ]);
    }
}

public sealed record AiComparisonPayload(string Task, string DataClassification, IReadOnlyList<AiSchoolData> Schools, AiComparisonRules Rules);
public sealed record AiSchoolData(string Id, string Name, string City, string District, string Level, decimal LiteracyScore, decimal NumeracyScore, decimal CompositeScore, int Rank);
public sealed record AiComparisonRules(string CompositeScoreFormula, string FairComparisonRule, string ExcludedInferences);
public sealed record AiNarrative(string Summary, IReadOnlyList<string> KeyFindings, IReadOnlyList<string> Considerations);
public sealed record AiComparisonResult(string Source, AiNarrative Narrative, IReadOnlyList<string> Warnings, AiComparisonPayload AnalysisPayload);
