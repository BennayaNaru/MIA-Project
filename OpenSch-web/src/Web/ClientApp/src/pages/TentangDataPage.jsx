import Icon from '../components/common/Icon'

const SCALES = [
  {
    icon: 'menu_book',
    title: 'Nilai Literasi (0 - 100)',
    tone: 'text-secondary',
    desc: 'Mengukur kapasitas bernalar murid dalam menganalisis bacaan, mengevaluasi fakta, dan merefleksikan gagasan.',
    levels: [
      ['85+', 'Kemampuan evaluatif kritis tingkat lanjut.'],
      ['70 - 84', 'Pemahaman komprehensif memadai.'],
      ['< 70', 'Memerlukan pendampingan teks analitis.'],
    ],
  },
  {
    icon: 'calculate',
    title: 'Nilai Numerasi (0 - 100)',
    tone: 'text-primary',
    desc: 'Mengukur kemampuan mengaplikasikan logika matematika, rasio, pola struktur, dan interpretasi data kuantitatif.',
    levels: [
      ['85+', 'Pemecahan masalah non-rutin luar biasa.'],
      ['70 - 84', 'Penguasaan aplikasi logika proporsional.'],
      ['< 70', 'Perlu penguatan konsep operasional dasar.'],
    ],
  },
]

export default function TentangDataPage() {
  return (
    <div className="mx-auto max-w-4xl space-y-6 px-4 py-8 sm:px-6">
      <div className="mx-auto max-w-xl space-y-2 text-center">
        <span className="rounded-full bg-secondary/10 px-3 py-1 text-xs font-bold text-secondary">
          Dokumentasi Terbuka
        </span>
        <h2 className="font-display text-2xl font-bold text-primary">Metodologi &amp; Batasan Data DKI</h2>
        <p className="text-xs text-on-surface-variant">
          Prinsip perancangan, batasan geografis 4 Kota Administrasi, dan etika penyajian data mutu pendidikan di platform OpenSch.
        </p>
      </div>

      {/* Bagian 1 */}
      <div className="space-y-3 rounded-2xl border border-outline-variant bg-white p-5 shadow-sm">
        <div className="flex items-center gap-2.5">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-surface-container text-primary">
            <Icon name="dataset" className="text-[20px]" />
          </div>
          <div>
            <h3 className="text-sm font-bold text-primary">Batasan Wilayah &amp; Dataset Sintetis</h3>
            <p className="text-xs text-outline">Hanya 4 Kota Administrasi DKI Jakarta &amp; 4 Jenjang (SD, SMP, SMA, SMK)</p>
          </div>
        </div>
        <div className="space-y-2 text-xs leading-relaxed text-on-surface-variant">
          <p>
            Platform OpenSch versi ini dibatasi secara ketat hanya mencakup <strong>4 Wilayah Kota Administrasi DKI Jakarta</strong>:{' '}
            <em>Jakarta Selatan, Jakarta Timur, Jakarta Utara, dan Jakarta Barat</em>. Platform ini secara sengaja tidak
            memasukkan Jakarta Pusat, Kepulauan Seribu, maupun wilayah luar DKI lainnya.
          </p>
          <p>
            Jenjang pendidikan yang dianalisis secara eksklusif dibatasi pada{' '}
            <strong>SD (Sekolah Dasar), SMP (Sekolah Menengah Pertama), dan SMK (Sekolah Menengah Kejuruan)</strong>. Seluruh data
            nama sekolah, nilai literasi (0-100), numerasi (0-100), dan peringkat pada platform ini adalah{' '}
            <strong>DATA SINTETIS / DUMMY</strong> untuk keperluan uji coba teknologi analitik publik tanpa pengaruh sponsor.
          </p>
        </div>
      </div>

      {/* Bagian 2 */}
      <div className="space-y-3 rounded-2xl border border-outline-variant bg-white p-5 shadow-sm">
        <div className="flex items-center gap-2.5">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-secondary/10 text-secondary">
            <Icon name="functions" className="text-[20px]" />
          </div>
          <div>
            <h3 className="text-sm font-bold text-primary">Rumus Skor Komposit &amp; Fair Comparison Rule</h3>
            <p className="text-xs text-outline">Hanya 4 Kota Administrasi DKI Jakarta &amp; 4 Jenjang (SD, SMP, SMA, SMK)</p>
          </div>
        </div>

        <div className="rounded-xl border border-outline-variant/60 bg-surface-container-low p-3.5 font-mono text-xs text-primary">
          <div className="mb-1 font-sans text-[11px] font-bold text-outline">Formula Komposit:</div>
          <div className="rounded-lg border border-outline-variant bg-white p-2.5">
            Skor Komposit = (Nilai Literasi + Nilai Numerasi) / 2
          </div>
          <div className="mt-2 font-sans text-[11px] text-on-surface-variant">
            * Peringkat Demo dihitung relatif terhadap kelompok pembanding sepadan (jenjang &amp; kota administrasi yang sama).
            Nilai literasi berlaku sebagai tie-breaker jika skor komposit identik.
          </div>
        </div>

        <div className="rounded-xl border border-secondary/20 bg-secondary/5 p-3 text-xs">
          <span className="font-bold text-secondary">Prinsip Fair Comparison:</span>
          <p className="mt-0.5 text-on-surface-variant">
            Pengguna diingatkan untuk tidak membandingkan peringkat antar jenjang yang berbeda secara langsung (misal: SD dengan
            SMK) karena profil kurikulum, tujuan capaian vokasional, dan target kompetensinya sangat berbeda.
          </p>
        </div>
      </div>

      {/* Bagian 3: cara membaca nilai */}
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        {SCALES.map((s) => (
          <div key={s.title} className="space-y-2 rounded-xl border border-outline-variant bg-white p-4">
            <span className={`flex items-center gap-1 text-xs font-bold ${s.tone}`}>
              <Icon name={s.icon} className="text-[16px]" /> {s.title}
            </span>
            <p className="text-xs text-on-surface-variant">{s.desc}</p>
            <ul className="list-disc space-y-0.5 pl-4 text-[11px] text-on-surface-variant">
              {s.levels.map(([range, text]) => (
                <li key={range}><strong>{range}:</strong> {text}</li>
              ))}
            </ul>
          </div>
        ))}
      </div>
    </div>
  )
}