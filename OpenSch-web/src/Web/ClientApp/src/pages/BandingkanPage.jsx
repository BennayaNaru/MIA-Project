import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Icon from '../components/common/Icon'
import { useCompare } from '../context/CompareContext'
import { BASELINE } from '../data/schools'

// TODO: sambungkan ke status login dari AuthContext saat halaman Masuk selesai.
// Selama false, toggle AI akan memunculkan modal "Masuk untuk Mengaktifkan AI".
const IS_LOGGED_IN = false

export default function BandingkanPage() {
  const navigate = useNavigate()
  const { selectedSchools: schools, remove, loadDefault } = useCompare()
  const [aiOn, setAiOn] = useState(false)
  const [aiLoading, setAiLoading] = useState(false)
  const [aiRun, setAiRun] = useState(0)
  const [showGate, setShowGate] = useState(false)
  const key = schools.map((s) => s.id).join(',')

  // Simulasi narasi AI: loading 0,5 detik tiap diaktifkan, sekolah berubah, atau Regenerasi
  useEffect(() => {
    if (!aiOn) return
    setAiLoading(true)
    const t = setTimeout(() => setAiLoading(false), 500)
    return () => clearTimeout(t)
  }, [aiOn, aiRun, key])

  const onToggleAi = (checked) => {
    if (!checked) return setAiOn(false)
    if (!IS_LOGGED_IN) return setShowGate(true)
    setAiOn(true)
  }

  const header = (
    <div className="flex flex-col justify-between gap-4 border-b border-outline-variant/60 pb-4 md:flex-row md:items-center">
      <div>
        <div className="mb-1 inline-flex items-center gap-1 text-xs font-bold uppercase tracking-wider text-secondary">
          <Icon name="compare" className="text-[14px]" />
          <span>Matriks Analisis Komparatif 4 Kota DKI</span>
        </div>
        <h2 className="font-display text-2xl font-bold text-primary">Komparasi Multivariat Sekolah</h2>
        <p className="text-xs text-on-surface-variant">
          Membandingkan metrik literasi, numerasi, dan skor komposit secara objektif.
        </p>
      </div>
      <div className="flex items-center gap-3 rounded-xl border border-outline-variant bg-white p-2.5 shadow-sm">
        <div className="text-right">
          <div className="flex items-center justify-end gap-1 text-xs font-bold text-primary">
            <Icon name="psychology" className="text-[14px] text-secondary" />
            <span>Asisten Analisis AI</span>
          </div>
          <span className="text-[10px] text-outline">Simulasi Narasi Terbimbing</span>
        </div>
        <label className="relative inline-flex cursor-pointer items-center">
          <input
            type="checkbox"
            className="peer sr-only"
            checked={aiOn}
            onChange={(e) => onToggleAi(e.target.checked)}
          />
          <div className="h-5 w-10 rounded-full bg-surface-container peer-checked:bg-secondary after:absolute after:left-[2px] after:top-[2px] after:h-4 after:w-4 after:rounded-full after:border after:border-outline-variant after:bg-white after:transition-all after:content-[''] peer-checked:after:translate-x-full peer-checked:after:border-white" />
        </label>
      </div>
    </div>
  )

  // Kurang dari 2 sekolah: tampilkan ajakan memilih
  if (schools.length < 2) {
    return (
      <div className="mx-auto max-w-7xl space-y-6 px-4 py-6 sm:px-6">
        {header}
        <div className="rounded-2xl border border-outline-variant bg-white p-8 text-center">
          <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-secondary/10 text-secondary">
            <Icon name="checklist" className="text-2xl" />
          </div>
          <h3 className="text-lg font-bold text-primary">Pilih Minimal 2 Sekolah untuk Memulai</h3>
          <p className="mx-auto mt-1 max-w-md text-xs text-on-surface-variant">
            Anda belum memilih cukup sekolah untuk dibandingkan. Kunjungi tab Cari Sekolah dan centang 2 atau 3 sekolah.
          </p>
          <div className="mt-4 flex justify-center gap-2">
            <button onClick={() => navigate('/cari')} className="rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-white">
              Cari Sekolah Sekarang
            </button>
            <button onClick={loadDefault} className="rounded-xl bg-secondary px-4 py-2 text-xs font-semibold text-white">
              Muat Contoh 3 Sekolah DKI
            </button>
          </div>
        </div>
      </div>
    )
  }

  // Aturan pembandingan setara (fair comparison)
  const levels = [...new Set(schools.map((s) => s.level))]
  const kotas = [...new Set(schools.map((s) => s.kota))]
  const diffs = []
  if (levels.length > 1) diffs.push(`perbedaan jenjang (${levels.join(', ')})`)
  if (kotas.length > 1) diffs.push(`lintas wilayah Kota Administrasi (${kotas.join(', ')})`)

  const maxLit = Math.max(...schools.map((s) => s.lit))
  const maxNum = Math.max(...schools.map((s) => s.num))
  const maxComp = Math.max(...schools.map((s) => s.composite))
  const topLit = schools.reduce((p, c) => (c.lit > p.lit ? c : p))
  const topNum = schools.reduce((p, c) => (c.num > p.num ? c : p))
  const topComp = schools.reduce((p, c) => (c.composite > p.composite ? c : p))
  const diffLit = (maxLit - Math.min(...schools.map((s) => s.lit))).toFixed(1)
  const diffNum = (maxNum - Math.min(...schools.map((s) => s.num))).toFixed(1)
  const names = schools.map((s) => `${s.name} (${s.level})`).join(' dan ')

  const Tag = ({ tone }) => (
    <span className={`ml-1 rounded px-1 text-[10px] ${tone === 'secondary' ? 'bg-secondary/15' : 'bg-primary/10'}`}>
      Tertinggi
    </span>
  )

  return (
    <div className="mx-auto max-w-7xl space-y-6 px-4 py-6 sm:px-6">
      {header}

      {/* Banner fair comparison */}
      {diffs.length > 0 && (
        <div className="flex items-start gap-3 rounded-xl border border-amber-300 bg-amber-50 p-3.5 text-amber-950 shadow-sm">
          <Icon name="policy" className="mt-0.5 shrink-0 text-[20px] text-amber-600" />
          <div className="text-xs">
            <span className="font-bold text-amber-900">Catatan Kelompok Pembanding (Fair Comparison Rule):</span>
            <p className="mt-0.5 leading-relaxed">
              Terdeteksi <strong>{diffs.join(' serta ')}</strong>. Catatan Kelompok Pembanding: Indeks skor dan peringkat
              hanya dapat dibandingkan secara <em>apple-to-apple</em> dalam jenjang dan wilayah yang sama. Jangan langsung
              menyamakan peringkat SD dengan SMK karena orientasi capaian kompetensinya berbeda.
            </p>
          </div>
        </div>
      )}

      {/* Kartu sekolah */}
      <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
        {schools.map((s, i) => (
          <div key={s.id} className="flex flex-col justify-between rounded-xl border border-outline-variant bg-white p-4 text-xs shadow-sm">
            <div>
              <div className="mb-1 flex items-center justify-between">
                <span className="rounded bg-primary px-2 py-0.5 text-[10px] font-bold text-white">
                  Sekolah {i + 1} ({s.level})
                </span>
                <button onClick={() => remove(s.id)} className="text-outline hover:text-error" aria-label="Hapus">
                  <Icon name="delete" className="text-[16px]" />
                </button>
              </div>
              <h4 className="line-clamp-2 text-sm font-bold text-primary">{s.name}</h4>
              <p className="mt-0.5 text-[11px] text-outline">{s.kota} • Kec. {s.kec}</p>
            </div>
            <div className="mt-3 flex items-center justify-between border-t border-outline-variant/40 pt-2">
              <div>
                <span className="text-[10px] font-bold uppercase text-outline">Skor Komposit</span>
                <div className="text-lg font-extrabold text-primary">{s.composite}</div>
              </div>
              <span className="rounded-full bg-secondary/15 px-2 py-0.5 text-[11px] font-bold text-secondary">#{s.rank} Demo</span>
            </div>
          </div>
        ))}
      </div>

      {/* Grafik batang */}
      <div className="space-y-4 rounded-2xl border border-outline-variant bg-white p-5 shadow-sm">
        <div className="flex flex-col justify-between gap-2 border-b border-outline-variant/40 pb-3 sm:flex-row sm:items-center">
          <div>
            <h3 className="flex items-center gap-1.5 text-sm font-bold text-primary">
              <Icon name="bar_chart" className="text-[18px] text-secondary" />
              <span>Visualisasi Perbandingan Skor (Skala 0 - 100)</span>
            </h3>
            <p className="text-[11px] text-outline">
              Garis perbandingan terhadap baseline 4 Kota Administrasi DKI (Literasi: {BASELINE.lit} | Numerasi: {BASELINE.num})
            </p>
          </div>
          <div className="flex items-center gap-3 text-xs">
            <span className="flex items-center gap-1"><span className="h-2.5 w-2.5 rounded bg-secondary" /> Literasi</span>
            <span className="flex items-center gap-1"><span className="h-2.5 w-2.5 rounded bg-primary" /> Numerasi</span>
            <span className="flex items-center gap-1"><span className="h-2.5 w-2.5 rounded border border-outline-variant bg-surface-container" /> Rerata DKI</span>
          </div>
        </div>

        <div className="space-y-4 pt-2">
          {schools.map((s) => (
            <div key={s.id} className="space-y-1.5 rounded-xl border border-outline-variant/50 bg-surface-container-low p-3 text-xs">
              <div className="flex items-center justify-between font-bold text-primary">
                <span>{s.name} <span className="font-normal text-outline">({s.level} - {s.kota})</span></span>
                <span className="text-xs">Komposit: {s.composite}</span>
              </div>
              {[
                { label: 'Literasi', value: s.lit, base: BASELINE.lit, text: 'text-secondary', bar: 'bg-secondary' },
                { label: 'Numerasi', value: s.num, base: BASELINE.num, text: 'text-primary', bar: 'bg-primary' },
              ].map((m) => (
                <div key={m.label}>
                  <div className="mb-0.5 flex justify-between text-[11px]">
                    <span className={`font-semibold ${m.text}`}>{m.label}: {m.value} / 100</span>
                    <span className="text-outline">vs DKI ({m.base})</span>
                  </div>
                  <div className="relative h-2 w-full rounded-full bg-surface-container">
                    <div className={`h-2 rounded-full ${m.bar}`} style={{ width: `${m.value}%` }} />
                    <div
                      className="absolute bottom-0 top-0 border-r-2 border-dashed border-outline-variant"
                      style={{ left: `${m.base}%` }}
                      title={`Rata DKI ${m.base}`}
                    />
                  </div>
                </div>
              ))}
            </div>
          ))}
        </div>
      </div>

      {/* Matriks */}
      <div className="overflow-hidden rounded-2xl border border-outline-variant bg-white shadow-sm">
        <div className="flex items-center justify-between border-b border-outline-variant bg-surface-container-low p-3.5 text-xs">
          <span className="font-bold text-primary">Matriks Spesifikasi &amp; Selisih Nilai</span>
          <span className="text-outline">Perhitungan Sistem Deterministik</span>
        </div>
        <div className="custom-scroll overflow-x-auto">
          <table className="w-full border-collapse text-left text-xs">
            <tbody className="divide-y divide-outline-variant/40">
              <tr className="bg-surface-container-low font-semibold text-outline">
                <td className="w-40 p-3">Metrik / Sekolah</td>
                {schools.map((s) => <td key={s.id} className="p-3">{s.name}</td>)}
              </tr>
              <tr>
                <td className="p-3 font-semibold text-on-surface">Kota Administrasi</td>
                {schools.map((s) => <td key={s.id} className="p-3">{s.kota}</td>)}
              </tr>
              <tr>
                <td className="p-3 font-semibold text-on-surface">Kecamatan</td>
                {schools.map((s) => <td key={s.id} className="p-3">Kec. {s.kec}</td>)}
              </tr>
              <tr>
                <td className="p-3 font-semibold text-on-surface">Jenjang</td>
                {schools.map((s) => (
                  <td key={s.id} className="p-3">
                    <span className="rounded bg-primary px-2 py-0.5 text-[10px] font-bold text-white">{s.level}</span>
                  </td>
                ))}
              </tr>
              <tr>
                <td className="p-3 font-semibold text-secondary">Nilai Literasi</td>
                {schools.map((s) => (
                  <td key={s.id} className={`p-3 font-bold ${s.lit === maxLit ? 'text-secondary' : 'text-on-surface'}`}>
                    {s.lit} {s.lit === maxLit && <Tag tone="secondary" />}
                  </td>
                ))}
              </tr>
              <tr>
                <td className="p-3 font-semibold text-primary">Nilai Numerasi</td>
                {schools.map((s) => (
                  <td key={s.id} className={`p-3 font-bold ${s.num === maxNum ? 'text-primary' : 'text-on-surface'}`}>
                    {s.num} {s.num === maxNum && <Tag tone="primary" />}
                  </td>
                ))}
              </tr>
              <tr className="bg-surface-container-low/50">
                <td className="p-3 font-extrabold text-primary">Skor Komposit (Lit+Num)/2</td>
                {schools.map((s) => (
                  <td key={s.id} className={`p-3 text-sm font-extrabold ${s.composite === maxComp ? 'text-primary' : 'text-on-surface'}`}>
                    {s.composite}
                  </td>
                ))}
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      {/* Ringkasan rule-based */}
      <div className="space-y-2 rounded-2xl border border-outline-variant bg-white p-5 shadow-sm">
        <div className="mb-2 flex items-center justify-between">
          <h3 className="flex items-center gap-1.5 text-sm font-bold text-primary">
            <Icon name="auto_stories" className="text-[18px] text-secondary" />
            <span>Ringkasan Analisis Deterministik (Tanpa Bias)</span>
          </h3>
          <span className="rounded-full bg-surface-container px-2 py-0.5 text-[10px] font-bold text-primary">Rule-based Logic</span>
        </div>
        <div className="space-y-1.5 text-xs leading-relaxed text-on-surface-variant">
          <p>
            1. <strong>Capaian Literasi Tertinggi:</strong> Dipimpin oleh <strong>{topLit.name}</strong> ({topLit.kota}, {topLit.level}) dengan
            nilai <strong>{topLit.lit}</strong>. Selisih capaian literasi antarsekolah yang dibandingkan adalah <strong>{diffLit} poin</strong>.
          </p>
          <p>
            2. <strong>Capaian Numerasi Tertinggi:</strong> Diraih oleh <strong>{topNum.name}</strong> ({topNum.kota}, {topNum.level}) dengan
            nilai <strong>{topNum.num}</strong>. Rentang selisih numerasi mencapai <strong>{diffNum} poin</strong>.
          </p>
          <p>
            3. <strong>Skor Komposit Teratas:</strong> Formula deterministik mencatat <strong>{topComp.name}</strong> memiliki skor komposit
            tertinggi (<strong>{topComp.composite}</strong>).
          </p>
        </div>
        <div className="flex items-center gap-1 border-t border-outline-variant/40 pt-2 text-[11px] text-outline">
          <Icon name="info" className="text-[14px]" />
          <span>Ringkasan dihasilkan secara komputasi selisih nilai absolut tanpa interpretasi subyektif.</span>
        </div>
      </div>

      {/* Panel AI (simulasi) */}
      {aiOn && (
        <div className="space-y-3 rounded-2xl border-2 border-secondary/30 bg-gradient-to-br from-white to-surface-container-low p-5 shadow-md">
          <div className="flex items-center justify-between border-b border-secondary/20 pb-2">
            <div className="flex items-center gap-2">
              <div className="flex h-7 w-7 items-center justify-center rounded-lg bg-secondary text-white">
                <Icon name="smart_toy" className="text-[16px]" />
              </div>
              <div>
                <h4 className="flex items-center gap-1.5 text-xs font-bold text-primary">
                  <span>Simulasi Narasi AI Assistant</span>
                  <span className="rounded bg-secondary/10 px-1.5 text-[10px] font-bold text-secondary">Sintetis</span>
                </h4>
                <p className="text-[11px] text-outline">Eksplorasi naratif hipotetis pendampingan orang tua</p>
              </div>
            </div>
            <span className="text-xs font-semibold text-secondary">Model: EduRank-DKI-v1.2</span>
          </div>

          {aiLoading ? (
            <div className="space-y-2 py-6 text-center">
              <div className="inline-block h-6 w-6 animate-spin rounded-full border-2 border-secondary border-t-transparent" />
              <p className="text-xs text-on-surface-variant">Menghitung sintesis naratif perbandingan sekolah...</p>
            </div>
          ) : (
            <div className="space-y-2 text-xs leading-relaxed text-on-surface">
              <p>Berdasarkan simulasi sintesis indikator mutu untuk <strong>{names}</strong>:</p>
              <ul className="list-disc space-y-1 pl-4">
                <li>
                  <strong>Karakteristik Kurikulum:</strong> Perbedaan profil numerasi menunjukkan variasi orientasi pengajaran
                  logika analitis versus penguatan literasi pemahaman teks.
                </li>
                <li>
                  <strong>Pertimbangan Orang Tua:</strong> Bagi siswa dengan minat logika teknis/vokasi, perhatikan indikator
                  numerasi sekolah jenjang SMK yang tertera. Sedangkan untuk jenjang dasar (SD/SMP), konsistensi kedua nilai di
                  atas baseline DKI mencerminkan kesiapan fondasi belajar yang berimbang.
                </li>
              </ul>
            </div>
          )}

          <div className="flex items-center justify-between border-t border-secondary/15 pt-2 text-[11px] text-outline">
            <span>⚠️ Simulasi narasi AI tidak menggantikan musyawarah keluarga.</span>
            <button onClick={() => setAiRun((n) => n + 1)} className="flex items-center gap-0.5 font-bold text-secondary hover:underline">
              <Icon name="refresh" className="text-[14px]" /> Regenerasi
            </button>
          </div>
        </div>
      )}

      {/* Modal: masuk untuk mengaktifkan AI */}
      {showGate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm">
          <div className="w-full max-w-sm space-y-3 rounded-2xl border-2 border-secondary/40 bg-white p-5 text-center text-xs shadow-2xl">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-xl bg-secondary/10 text-secondary">
              <Icon name="psychology" className="text-2xl" />
            </div>
            <div>
              <h3 className="text-sm font-bold text-primary">Masuk untuk Mengaktifkan AI</h3>
              <p className="mt-1 text-[11px] text-on-surface-variant">
                {schools.length} sekolah pilihan ({schools.map((s) => s.name).join(', ')}) akan tetap tersimpan dan langsung
                dianalisis begitu Anda masuk.
              </p>
            </div>
            <div className="flex flex-col gap-2 pt-2">
              <button
                onClick={() => { setShowGate(false); navigate('/masuk') }}
                className="w-full rounded-xl bg-secondary py-2 font-bold text-white hover:bg-[#00685e]"
              >
                Masuk Sekarang (Retensi Data)
              </button>
              <button onClick={() => setShowGate(false)} className="w-full rounded-lg py-1.5 text-on-surface-variant hover:bg-surface-container">
                Batal
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}