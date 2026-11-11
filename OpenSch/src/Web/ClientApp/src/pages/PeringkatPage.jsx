import { useEffect, useMemo, useState } from 'react'
import Icon from '../components/common/Icon'
import { useCompare } from '../context/CompareContext'

const field =
  'mt-1 w-full rounded-lg border border-outline-variant px-2.5 py-1.5 text-xs focus:border-secondary focus:ring-1 focus:ring-secondary'

const LIMITS = [5, 10, 20, 50]

export default function PeringkatPage() {
  const { schools, schoolsLoading, schoolsError, openDetail } = useCompare()
  const [level, setLevel] = useState('')
  const [city, setCity] = useState('')
  const [limit, setLimit] = useState(10)
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [refresh, setRefresh] = useState(0)

  const levels = useMemo(
    () => [...new Set(schools.map((school) => school.level))].sort((a, b) => a.localeCompare(b)),
    [schools],
  )
  const cities = useMemo(
    () => [...new Set(schools.map((school) => school.kota))].sort((a, b) => a.localeCompare(b)),
    [schools],
  )

  // Default to the documented demo group once the catalog arrives.
  useEffect(() => {
    if (!level && levels.length) {
      setLevel(levels.includes('SMP') ? 'SMP' : levels[0])
    }
    if (!city && cities.length) {
      setCity(cities.includes('Jakarta Selatan') ? 'Jakarta Selatan' : cities[0])
    }
  }, [levels, cities, level, city])

  useEffect(() => {
    if (!level || !city) return undefined

    const controller = new AbortController()
    setLoading(true)
    setError('')

    fetch(
      `/api/schools/ranking/summary?level=${encodeURIComponent(level)}&city=${encodeURIComponent(city)}&limit=${limit}`,
      { headers: { Accept: 'application/json' }, signal: controller.signal },
    )
      .then(async (response) => {
        if (!response.ok) {
          const body = await response.json().catch(() => null)
          const detail = body?.errors
            ? Object.values(body.errors).flat()[0]
            : body?.detail || body?.title
          throw new Error(detail || 'Ringkasan peringkat tidak dapat dimuat.')
        }
        return response.json()
      })
      .then((data) => setSummary(data))
      .catch((err) => {
        if (err.name !== 'AbortError') {
          setSummary(null)
          setError(err.message)
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [level, city, limit, refresh])

  const top = summary?.schools ?? []
  const narrative = summary?.narrative
  const total = summary?.totalSchools ?? 0
  const busy = schoolsLoading || loading

  return (
    <div className="mx-auto max-w-7xl space-y-6 px-4 py-6 sm:px-6">
      <div className="flex flex-col justify-between gap-4 lg:flex-row lg:items-end">
        <div>
          <div className="mb-1 inline-flex items-center gap-1 rounded-full bg-secondary/10 px-2.5 py-0.5 text-xs font-bold text-secondary">
            <Icon name="leaderboard" className="text-[14px]" />
            <span>Peringkat Kelompok</span>
          </div>
          <h2 className="font-display text-2xl font-bold text-primary">Peringkat Sekolah per Jenjang &amp; Kota</h2>
          <p className="max-w-2xl text-xs text-on-surface-variant">
            Peringkat dihitung dari skor komposit dan hanya dibandingkan dengan sekolah pada jenjang dan kota
            administrasi yang sama.
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-3 rounded-xl border border-outline-variant bg-white px-4 py-2 text-xs shadow-sm">
          <div>
            <span className="text-outline">Sekolah di kelompok:</span>
            <span className="ml-1 font-bold text-primary">{total}</span>
          </div>
          <span className="hidden text-outline-variant sm:inline">|</span>
          <div>
            <span className="text-outline">Peringkat 1:</span>
            <span className="ml-1 font-bold text-secondary">{top[0]?.name ?? '—'}</span>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-3 rounded-2xl border border-outline-variant bg-white p-4 shadow-sm sm:grid-cols-3">
        <label className="block text-xs font-semibold text-on-surface">
          Jenjang
          <select className={field} value={level} onChange={(event) => setLevel(event.target.value)}>
            {levels.map((option) => (
              <option key={option} value={option}>{option}</option>
            ))}
          </select>
        </label>
        <label className="block text-xs font-semibold text-on-surface">
          Kota Administrasi
          <select className={field} value={city} onChange={(event) => setCity(event.target.value)}>
            {cities.map((option) => (
              <option key={option} value={option}>{option}</option>
            ))}
          </select>
        </label>
        <label className="block text-xs font-semibold text-on-surface">
          Tampilkan
          <select className={field} value={limit} onChange={(event) => setLimit(Number(event.target.value))}>
            {LIMITS.map((option) => (
              <option key={option} value={option}>Top {option}</option>
            ))}
          </select>
        </label>
      </div>

      {schoolsError && (
        <div className="rounded-2xl border border-error/30 bg-error-container/20 p-4 text-xs text-error">
          {schoolsError}
        </div>
      )}

      {error && (
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-error/30 bg-error-container/20 p-4 text-xs text-error">
          <span className="flex items-center gap-2">
            <Icon name="error" className="text-[16px]" />
            {error}
          </span>
          <button
            onClick={() => setRefresh((value) => value + 1)}
            className="flex items-center gap-1 rounded-lg bg-white px-3 py-1.5 font-semibold text-primary shadow-sm"
          >
            <Icon name="refresh" className="text-[14px]" /> Coba lagi
          </button>
        </div>
      )}

      {busy && !error && (
        <div className="rounded-2xl border border-outline-variant bg-white p-10 text-center shadow-sm">
          <div className="mx-auto mb-2 h-6 w-6 animate-spin rounded-full border-2 border-secondary border-t-transparent" />
          <p className="text-xs text-on-surface-variant">Menghitung peringkat dan menyusun ringkasan...</p>
        </div>
      )}

      {!busy && !error && narrative && (
        <div className="rounded-2xl border border-secondary/25 bg-secondary/5 p-5 shadow-sm">
          <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
            <h3 className="flex items-center gap-1.5 text-sm font-bold text-primary">
              <Icon name="auto_awesome" className="text-[18px] text-secondary" />
              <span>Ringkasan Otomatis</span>
            </h3>
            <span className="rounded-full bg-white px-2 py-0.5 text-[10px] font-bold text-secondary">
              Rule-based · Tanpa AI
            </span>
          </div>
          <p className="text-sm font-bold text-primary">{narrative.headline}</p>
          <ul className="mt-2 list-disc space-y-1 pl-4 text-xs leading-relaxed text-on-surface-variant">
            {narrative.points.map((point) => <li key={point}>{point}</li>)}
          </ul>
          <p className="mt-3 border-t border-secondary/20 pt-2 text-[11px] text-outline">{narrative.scopeNote}</p>
        </div>
      )}

      {!busy && !error && top.length > 0 && (
        <div className="space-y-2">
          <div className="flex flex-wrap items-center justify-between gap-2 text-xs text-outline">
            <span>
              Menampilkan <strong className="text-primary">{top.length}</strong> dari{' '}
              <strong className="text-primary">{total}</strong> sekolah pada kelompok ini.
            </span>
            <span>Skor komposit = (literasi + numerasi) / 2</span>
          </div>

          <div className="overflow-hidden rounded-2xl border border-outline-variant bg-white shadow-sm">
            <div className="custom-scroll overflow-x-auto">
              <table className="w-full border-collapse text-left text-xs">
                <thead>
                  <tr className="border-b border-outline-variant bg-surface-container-low font-semibold text-outline">
                    <th className="p-3">Peringkat</th>
                    <th className="p-3">Nama Sekolah</th>
                    <th className="p-3">Kecamatan</th>
                    <th className="p-3">Jenjang</th>
                    <th className="p-3">Literasi</th>
                    <th className="p-3">Numerasi</th>
                    <th className="p-3">Skor Komposit</th>
                    <th className="p-3 text-right">Aksi</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-outline-variant/40">
                  {top.map((school) => (
                    <tr
                      key={school.id}
                      className={`transition-colors hover:bg-surface-container-low ${school.rank <= 3 ? 'bg-secondary/5' : ''}`}
                    >
                      <td className="p-3">
                        <span
                          className={`inline-flex min-w-7 justify-center rounded px-2 py-0.5 font-bold ${
                            school.rank === 1 ? 'bg-secondary text-white' : 'bg-surface-container text-primary'
                          }`}
                        >
                          #{school.rank}
                        </span>
                      </td>
                      <td className="p-3 font-semibold text-primary">
                        <button
                          type="button"
                          onClick={() => openDetail(school.id)}
                          className="text-left hover:underline"
                        >
                          {school.name}
                        </button>
                      </td>
                      <td className="p-3 text-outline">Kec. {school.district}</td>
                      <td className="p-3">
                        <span className="rounded bg-primary px-2 py-0.5 text-[10px] font-bold text-white">
                          {school.level}
                        </span>
                      </td>
                      <td className="p-3 font-bold text-secondary">{school.literacyScore}</td>
                      <td className="p-3 font-bold text-primary">{school.numeracyScore}</td>
                      <td className="p-3 font-extrabold text-primary">{school.compositeScore}</td>
                      <td className="p-3 text-right">
                        <button
                          type="button"
                          onClick={() => openDetail(school.id)}
                          className="rounded bg-surface-container px-2 py-1 font-semibold text-primary hover:bg-surface-container-high"
                        >
                          Detail
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {!busy && !error && narrative && top.length === 0 && (
        <div className="rounded-2xl border border-outline-variant bg-white p-8 text-center shadow-sm">
          <Icon name="leaderboard" className="text-3xl text-secondary" />
          <h3 className="mt-2 text-sm font-bold text-primary">Belum ada sekolah pada kelompok ini</h3>
          <p className="mt-1 text-xs text-on-surface-variant">
            Pilih jenjang atau kota administrasi lain untuk melihat peringkat yang tersedia.
          </p>
        </div>
      )}
    </div>
  )
}
