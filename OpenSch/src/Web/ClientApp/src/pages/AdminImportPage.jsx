import { useCallback, useEffect, useState } from "react";
import Icon from "../components/common/Icon";

const TEMPLATE_CSV =
  "npsn,nama,jenjang,status,wilayah,kecamatan,murid,guru,ruang_kelas,literasi,numerasi,tanggal_data\n" +
  "20100001,SDN Contoh 01,SD,Negeri,Jakarta Selatan,Tebet,320,21,12,85.5,80.0,2026-01-15\n" +
  "20100002,SMP Contoh 02,SMP,Swasta,Jakarta Barat,Kebon Jeruk,410,26,15,78.0,82.5,2026-01-15\n";

const badgeClass = (status) =>
  status === "BERHASIL"
    ? "bg-secondary/15 text-secondary"
    : "bg-error-container text-error";

const formatDate = (value) => {
  if (!value) return "-";
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? "-" : date.toLocaleString("id-ID");
};

function downloadTemplate() {
  const blob = new Blob([TEMPLATE_CSV], { type: "text/csv;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = "template-impor-sekolah.csv";
  link.click();
  URL.revokeObjectURL(url);
}

export default function AdminImportPage() {
  const [access, setAccess] = useState("checking");
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [report, setReport] = useState(null);
  const [history, setHistory] = useState([]);
  const [error, setError] = useState("");
  const [historyLoading, setHistoryLoading] = useState(true);

  const loadHistory = useCallback(() => {
    setHistoryLoading(true);
    fetch("/api/admin/imports", { credentials: "include", headers: { Accept: "application/json" } })
      .then((response) => (response.ok ? response.json() : Promise.reject(response.status)))
      .then((data) => setHistory(data))
      .catch(() => setHistory([]))
      .finally(() => setHistoryLoading(false));
  }, []);

  useEffect(() => {
    fetch("/api/admin/me", { credentials: "include", headers: { Accept: "application/json" } })
      .then((response) => (response.ok ? response.json() : { isAdministrator: false }))
      .then((data) => {
        const allowed = Boolean(data?.isAdministrator);
        setAccess(allowed ? "allowed" : "denied");
        if (allowed) loadHistory();
      })
      .catch(() => setAccess("denied"));
  }, [loadHistory]);

  const upload = async (event) => {
    event.preventDefault();
    if (!file) {
      setError("Pilih file CSV terlebih dahulu.");
      return;
    }

    setUploading(true);
    setError("");
    setReport(null);

    const body = new FormData();
    body.append("file", file);

    // Raw fetch on purpose: the generated client types this endpoint as Promise<void>,
    // which would discard the row-level validation report the portal has to display.
    try {
      const response = await fetch("/api/admin/imports", {
        method: "POST",
        credentials: "include",
        body,
      });
      const payload = await response.json().catch(() => null);

      if (payload && payload.status) {
        setReport(payload);
      } else {
        setError(payload?.detail || payload?.title || "Impor gagal diproses.");
      }

      if (response.ok) setFile(null);
    } catch {
      setError("Tidak dapat menghubungi server. Coba lagi.");
    } finally {
      setUploading(false);
      loadHistory();
    }
  };

  if (access === "checking") {
    return (
      <div className="max-w-5xl mx-auto px-4 py-16 text-center text-sm text-on-surface-variant">
        Memeriksa akses…
      </div>
    );
  }

  if (access === "denied") {
    return (
      <div className="max-w-3xl mx-auto px-4 py-16">
        <div className="rounded-2xl border border-outline-variant bg-surface-container-lowest p-8 text-center shadow-sm">
          <div className="w-12 h-12 rounded-xl bg-error-container text-error flex items-center justify-center mx-auto mb-4">
            <Icon name="lock" className="text-2xl" />
          </div>
          <h1 className="text-xl font-display font-extrabold text-primary">Akses khusus administrator</h1>
          <p className="mt-2 text-sm text-on-surface-variant">
            Halaman impor data hanya tersedia untuk akun dengan peran Administrator.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 py-8">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-display font-extrabold text-primary">Impor Data Sekolah</h1>
          <p className="mt-1 text-sm text-on-surface-variant">
            Unggah CSV untuk menambah atau memperbarui sekolah. Data dicocokkan berdasarkan NPSN.
          </p>
        </div>
        <button
          type="button"
          onClick={downloadTemplate}
          className="inline-flex items-center gap-1.5 rounded-lg border border-outline-variant px-3 py-2 text-xs font-semibold text-primary hover:bg-surface-container-low"
        >
          <Icon name="download" className="text-[16px] text-secondary" />
          Unduh template CSV
        </button>
      </div>

      <form
        onSubmit={upload}
        className="mt-6 rounded-2xl border border-outline-variant bg-surface-container-lowest p-5 shadow-sm"
      >
        <div className="flex flex-col sm:flex-row sm:items-center gap-3">
          <input
            type="file"
            accept=".csv,text/csv"
            onChange={(event) => setFile(event.target.files?.[0] ?? null)}
            className="block w-full text-xs file:mr-3 file:rounded-lg file:border-0 file:bg-primary file:px-3 file:py-2 file:text-xs file:font-semibold file:text-white"
          />
          <button
            type="submit"
            disabled={uploading}
            className="inline-flex items-center justify-center gap-1.5 rounded-lg bg-secondary px-4 py-2 text-xs font-bold text-white shadow-sm disabled:opacity-60"
          >
            <Icon name="upload_file" className="text-[16px]" />
            {uploading ? "Mengimpor…" : "Impor CSV"}
          </button>
        </div>
        <p className="mt-3 text-[11px] text-outline">
          Kolom wajib: npsn, nama, jenjang, wilayah, literasi, numerasi. Opsional: status, kecamatan,
          murid, guru, ruang_kelas, tanggal_data. Maksimal 2 MB. Impor bersifat all-or-nothing.
        </p>
      </form>

      {error && (
        <div className="mt-4 rounded-xl border border-error/30 bg-error-container/60 px-4 py-3 text-xs font-medium text-error">
          {error}
        </div>
      )}

      {report && (
        <div className="mt-4 rounded-2xl border border-outline-variant bg-surface-container-lowest p-5 shadow-sm">
          <div className="flex items-center justify-between gap-3">
            <h2 className="text-sm font-bold text-primary">Hasil impor</h2>
            <span className={"rounded-full px-2.5 py-0.5 text-[11px] font-bold " + badgeClass(report.status)}>
              {report.status}
            </span>
          </div>
          <div className="mt-3 grid grid-cols-2 sm:grid-cols-4 gap-3 text-center">
            {[
              ["Total baris", report.totalRows],
              ["Ditambah", report.inserted],
              ["Diperbarui", report.updated],
              ["Kesalahan", report.errors?.length ?? 0],
            ].map(([label, value]) => (
              <div key={label} className="rounded-xl bg-surface-container-low px-3 py-2">
                <div className="text-lg font-display font-extrabold text-primary">{value}</div>
                <div className="text-[11px] text-outline">{label}</div>
              </div>
            ))}
          </div>

          {report.errors?.length > 0 && (
            <div className="mt-4 overflow-x-auto">
              <table className="w-full text-left text-xs">
                <thead className="text-[11px] uppercase tracking-wide text-outline">
                  <tr>
                    <th className="py-2 pr-3">Baris</th>
                    <th className="py-2 pr-3">Kolom</th>
                    <th className="py-2">Pesan</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-outline-variant">
                  {report.errors.map((item, index) => (
                    <tr key={index}>
                      <td className="py-2 pr-3 font-semibold text-on-surface">{item.rowNumber}</td>
                      <td className="py-2 pr-3 text-on-surface-variant">{item.field}</td>
                      <td className="py-2 text-on-surface-variant">{item.message}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      <div className="mt-6 rounded-2xl border border-outline-variant bg-surface-container-lowest shadow-sm overflow-hidden">
        <div className="px-5 py-4 border-b border-outline-variant">
          <h2 className="text-sm font-bold text-primary">Riwayat impor</h2>
        </div>
        {historyLoading ? (
          <div className="px-5 py-8 text-center text-xs text-on-surface-variant">Memuat riwayat…</div>
        ) : history.length === 0 ? (
          <div className="px-5 py-8 text-center text-xs text-on-surface-variant">Belum ada impor.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead className="bg-surface-container-low text-[11px] uppercase tracking-wide text-outline">
                <tr>
                  <th className="px-4 py-2.5">File</th>
                  <th className="px-4 py-2.5">Status</th>
                  <th className="px-4 py-2.5">Baris</th>
                  <th className="px-4 py-2.5">Berhasil</th>
                  <th className="px-4 py-2.5">Gagal</th>
                  <th className="px-4 py-2.5">Waktu</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-outline-variant">
                {history.map((item) => (
                  <tr key={item.id}>
                    <td className="px-4 py-2.5 font-semibold text-primary">{item.fileName}</td>
                    <td className="px-4 py-2.5">
                      <span className={"rounded-full px-2 py-0.5 text-[10px] font-bold " + badgeClass(item.status)}>
                        {item.status}
                      </span>
                    </td>
                    <td className="px-4 py-2.5 text-on-surface-variant">{item.totalRows}</td>
                    <td className="px-4 py-2.5 text-on-surface-variant">{item.succeeded}</td>
                    <td className="px-4 py-2.5 text-on-surface-variant">{item.failed}</td>
                    <td className="px-4 py-2.5 text-on-surface-variant">{formatDate(item.importedAt)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
