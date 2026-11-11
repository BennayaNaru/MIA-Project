import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import Icon from '../components/common/Icon'
import SchoolCard from '../components/schools/SchoolCard'
import SchoolTable from '../components/schools/SchoolTable'
import { useCompare } from '../context/CompareContext'

const field =
    'w-full rounded-lg border border-outline-variant px-2.5 py-1.5 text-xs focus:border-secondary focus:ring-1 focus:ring-secondary'
const INITIAL = { kota: 'Semua', kec: 'Semua', jenjang: 'Semua', sort: 'komposit-desc', minLit: 0, minNum: 0 }

const SORTERS = {
    'komposit-desc': (a, b) => b.composite - a.composite || b.lit - a.lit,
    'lit-desc': (a, b) => b.lit - a.lit,
    'num-desc': (a, b) => b.num - a.num,
    'nama-asc': (a, b) => a.name.localeCompare(b.name),
}

export default function CariSekolahPage() {
    const [params] = useSearchParams()
    const {
        selectedIds, toggle, openDetail,
        schools: ALL_SCHOOLS,
        kecamatanMap: KECAMATAN_MAP,
        benchmark: BENCHMARK,
        schoolsLoading,
        schoolsError,
    } = useCompare()
    const [search, setSearch] = useState(params.get('q') ?? '')
    const [f, setF] = useState(INITIAL)
    const [mode, setMode] = useState('cards')
    useEffect(() => {
        setF((prev) => ({ ...prev, kota: params.get('kota') ?? 'Semua', kec: 'Semua' }))
        if (params.has('q')) setSearch(params.get('q'))
    }, [params])
    const set = (patch) => setF((prev) => ({ ...prev, ...patch }))
    const reset = () => { setSearch(''); setF(INITIAL) }

    const kecOptions =
        f.kota === 'Semua'
            ? Object.entries(KECAMATAN_MAP).flatMap(([k, list]) =>
                list.map((kec) => ({ value: kec, label: `${kec} (${k.replace('Jakarta ', '')})` })))
            : (KECAMATAN_MAP[f.kota] ?? []).map((kec) => ({ value: kec, label: kec }))

    const list = useMemo(() => {
        const q = search.toLowerCase().trim()
        return ALL_SCHOOLS.filter(
            (s) =>
                (s.name.toLowerCase().includes(q) || s.kec.toLowerCase().includes(q)) &&
                (f.kota === 'Semua' || s.kota === f.kota) &&
                (f.kec === 'Semua' || s.kec === f.kec) &&
                (f.jenjang === 'Semua' || s.level === f.jenjang) &&
                s.lit >= f.minLit &&
                s.num >= f.minNum
        ).sort(SORTERS[f.sort])
    }, [search, f, ALL_SCHOOLS])

    const avg = (key) =>
        list.length ? (list.reduce((t, s) => t + s[key], 0) / list.length).toFixed(1) : '0.0'

    const modeBtn = (active) =>
        `flex items-center gap-1 rounded px-2.5 py-1 text-xs ${active ? 'bg-primary font-medium text-white' : 'text-on-surface-variant hover:text-on-surface'
        }`

    return (
        <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
            {/* Judul & statistik */}
            <div className="mb-6 flex flex-col justify-between gap-4 lg:flex-row lg:items-center">
                <div>
                    <div className="mb-1 inline-flex items-center gap-1 rounded-full bg-secondary/10 px-2.5 py-0.5 text-xs font-bold text-secondary">
                        <Icon name="location_city" className="text-[14px]" />
                        <span>Wilayah Kota Administrasi DKI Jakarta</span>
                    </div>
                    <h2 className="font-display text-2xl font-bold text-primary">Katalog &amp; Direktori Mutu Sekolah</h2>
                    <p className="text-xs text-on-surface-variant">
                        Filter multi-indikator untuk 4 Kota Administrasi (Jaksel, Jaktim, Jakut, Jakbar) dan 4 jenjang pendidikan (SD, SMP, SMA, SMK).
                    </p>
                </div>
                <div className="flex items-center gap-3 rounded-xl border border-outline-variant bg-white px-4 py-2 text-xs shadow-sm">
                    <div><span className="text-outline">Ditemukan:</span><span className="ml-1 font-bold text-primary">{list.length}</span></div>
                    <span className="text-outline-variant">|</span>
                    <div><span className="text-outline">Rerata Lit:</span><span className="ml-1 font-bold text-secondary">{avg('lit')}</span></div>
                    <span className="text-outline-variant">|</span>
                    <div><span className="text-outline">Rerata Num:</span><span className="ml-1 font-bold text-primary">{avg('num')}</span></div>
                </div>
            </div>

            {/* Filter */}
            <div className="mb-6 space-y-3 rounded-2xl border border-outline-variant bg-white p-4 shadow-sm">
                <div className="grid grid-cols-1 gap-3 text-xs sm:grid-cols-2 lg:grid-cols-12">
                    <div className="lg:col-span-3">
                        <label className="mb-1 block font-semibold text-on-surface">Nama Sekolah</label>
                        <div className="relative flex items-center">
                            <Icon name="search" className="absolute left-2.5 text-[18px] text-outline" />
                            <input
                                type="text"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                placeholder="Ketik nama sekolah..."
                                className={`${field} pl-8 pr-3`}
                            />
                        </div>
                    </div>

                    <div className="lg:col-span-3">
                        <label className="mb-1 block font-semibold text-on-surface">Kota Administrasi</label>
                        <select className={field} value={f.kota} onChange={(e) => set({ kota: e.target.value, kec: 'Semua' })}>
                            <option value="Semua">Semua 4 Kota Administrasi</option>
                            {Object.keys(KECAMATAN_MAP).map((k) => <option key={k} value={k}>{k}</option>)}
                        </select>
                    </div>

                    <div className="lg:col-span-2">
                        <label className="mb-1 block font-semibold text-on-surface">Kecamatan</label>
                        <select className={field} value={f.kec} onChange={(e) => set({ kec: e.target.value })}>
                            <option value="Semua">Semua Kecamatan</option>
                            {kecOptions.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}
                        </select>
                    </div>

                    <div className="lg:col-span-2">
                        <label className="mb-1 block font-semibold text-on-surface">Jenjang Sekolah</label>
                        <select className={field} value={f.jenjang} onChange={(e) => set({ jenjang: e.target.value })}>
                            <option value="Semua">Semua Jenjang</option>
                            <option value="SD">SD (Sekolah Dasar)</option>
                            <option value="SMP">SMP (Sekolah Menengah Pertama)</option>
                            <option value="SMA">SMA (Sekolah Menengah Atas)</option>
                            <option value="SMK">SMK (Sekolah Menengah Kejuruan)</option>
                        </select>
                    </div>

                    <div className="lg:col-span-2">
                        <label className="mb-1 block font-semibold text-on-surface">Urutan Ranking</label>
                        <select className={field} value={f.sort} onChange={(e) => set({ sort: e.target.value })}>
                            <option value="komposit-desc">Skor Komposit Tertinggi</option>
                            <option value="lit-desc">Nilai Literasi Tertinggi</option>
                            <option value="num-desc">Nilai Numerasi Tertinggi</option>
                            <option value="nama-asc">Nama Sekolah (A - Z)</option>
                        </select>
                    </div>
                </div>

                <div className="grid grid-cols-1 items-center gap-3 border-t border-outline-variant/40 pt-3 text-xs md:grid-cols-12">
                    <div className="flex items-center gap-2 md:col-span-5">
                        <span className="flex w-24 shrink-0 items-center gap-1 font-semibold text-secondary">
                            <Icon name="menu_book" className="text-[14px]" /> Min Literasi:
                        </span>
                        <input
                            type="range" min="0" max="100" value={f.minLit}
                            onChange={(e) => set({ minLit: Number(e.target.value) })}
                            className="h-1.5 w-full cursor-pointer rounded-lg bg-surface-container accent-secondary"
                        />
                        <span className="w-8 text-right font-bold text-primary">{f.minLit}</span>
                    </div>
                    <div className="flex items-center gap-2 md:col-span-5">
                        <span className="flex w-24 shrink-0 items-center gap-1 font-semibold text-primary">
                            <Icon name="calculate" className="text-[14px]" /> Min Numerasi:
                        </span>
                        <input
                            type="range" min="0" max="100" value={f.minNum}
                            onChange={(e) => set({ minNum: Number(e.target.value) })}
                            className="h-1.5 w-full cursor-pointer rounded-lg bg-surface-container accent-primary"
                        />
                        <span className="w-8 text-right font-bold text-primary">{f.minNum}</span>
                    </div>
                    <div className="flex justify-end md:col-span-2">
                        <button
                            onClick={reset}
                            className="flex items-center gap-1 rounded px-2 py-1 text-xs text-outline transition-colors hover:bg-error-container/20 hover:text-error"
                        >
                            <Icon name="restart_alt" className="text-[14px]" /> Reset Filter
                        </button>
                    </div>
                </div>
            </div>

            {/* Switcher tampilan */}
            <div className="mb-4 flex items-center justify-between text-xs">
                <div className="flex items-center gap-2">
                    <span className="text-outline">Tampilan:</span>
                    <div className="inline-flex rounded-lg border border-outline-variant bg-white p-0.5">
                        <button className={modeBtn(mode === 'cards')} onClick={() => setMode('cards')}>
                            <Icon name="grid_view" className="text-[14px]" /> Kartu
                        </button>
                        <button className={modeBtn(mode === 'table')} onClick={() => setMode('table')}>
                            <Icon name="table_rows" className="text-[14px]" /> Tabel
                        </button>
                    </div>
                </div>
                <div className="text-outline">
                    Menampilkan <span className="font-bold text-primary">{list.length}</span> sekolah sintetis DKI
                </div>
            </div>

            {/* Hasil */}
            {schoolsLoading ? (
                <div className="rounded-2xl border border-outline-variant bg-white py-12 text-center">
                    <div className="mx-auto mb-3 h-6 w-6 animate-spin rounded-full border-2 border-secondary border-t-transparent" />
                    <p className="text-xs text-on-surface-variant">Memuat katalog sekolah dari server...</p>
                </div>
            ) : schoolsError ? (
                <div className="rounded-2xl border border-error/30 bg-error-container/20 p-4 text-center text-xs text-error">
                    {schoolsError}
                </div>
            ) : list.length === 0 ? (
                <div className="rounded-2xl border border-dashed border-outline-variant bg-white py-12 text-center">
                    <Icon name="search_off" className="mb-2 text-4xl text-outline" />
                    <h3 className="text-base font-bold text-primary">Tidak Ada Sekolah Ditemukan</h3>
                    <p className="mx-auto mt-1 max-w-sm text-xs text-on-surface-variant">
                        Coba sesuaikan filter pencarian wilayah kota administrasi, kecamatan, atau turunkan batas minimum nilai.
                    </p>
                    <button onClick={reset} className="mt-3 rounded-lg bg-primary px-4 py-1.5 text-xs font-semibold text-white">
                        Reset Semua Filter
                    </button>
                </div>
            ) : mode === 'cards' ? (
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {list.map((s) => (
                        <SchoolCard
                            key={s.id}
                            school={s}
                            selected={selectedIds.includes(s.id)}
                            onToggle={toggle}
                            onDetail={openDetail}
                            benchmark={BENCHMARK}
                        />
                    ))}
                </div>
            ) : (
                <SchoolTable schools={list} selectedIds={selectedIds} onToggle={toggle} onDetail={openDetail} />
            )}
        </div>
    )
}