import Icon from '../common/Icon'
import { useCompare } from '../../context/CompareContext'

export default function SchoolDetailModal() {
  const { detailId, closeDetail, selectedIds, toggle, schools, baseline } = useCompare()
  const s = schools.find((x) => x.id === detailId)
  if (!s) return null

  const selected = selectedIds.includes(s.id)
  const handleCompare = () => {
    toggle(s.id)
    closeDetail()
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm">
      <div className="flex max-h-[90vh] w-full max-w-xl flex-col overflow-hidden rounded-2xl border border-outline-variant bg-white shadow-2xl">
        <div className="flex items-start justify-between border-b border-outline-variant/60 bg-surface-container-low p-5">
          <div>
            <div className="mb-1 flex items-center gap-1.5 text-xs">
              <span className="rounded bg-primary px-2 py-0.5 font-bold text-white">{s.level}</span>
              <span className="rounded bg-surface-container px-2 py-0.5 text-on-surface">{s.kota} • Kec. {s.kec}</span>
              <span className="rounded bg-secondary/15 px-2 py-0.5 font-bold text-secondary">#{s.rank} Demo</span>
            </div>
            <h3 className="text-lg font-bold text-primary">{s.name}</h3>
            <p className="text-[11px] text-outline">
              ID: {s.id} • Status: Data Sintetis Terverifikasi (DKI 4 Kota)
            </p>
          </div>
          <button onClick={closeDetail} className="p-1 text-outline hover:text-on-surface" aria-label="Tutup">
            <Icon name="close" className="text-[20px]" />
          </button>
        </div>

        <div className="custom-scroll space-y-4 overflow-y-auto p-5 text-xs">
          <div className="grid grid-cols-3 gap-2.5 text-center">
            <div className="rounded-xl border border-outline-variant/60 bg-surface-container-low p-2.5">
              <span className="text-outline">Skor Komposit</span>
              <div className="text-xl font-bold text-primary">{s.composite}</div>
              <span className="text-[10px] text-secondary">(Lit + Num)/2</span>
            </div>
            <div className="rounded-xl border border-secondary/20 bg-secondary/10 p-2.5">
              <span className="font-semibold text-secondary">Skor Literasi</span>
              <div className="text-xl font-bold text-secondary">{s.lit}</div>
              <span className="text-[10px] text-outline">Skala 0-100</span>
            </div>
            <div className="rounded-xl border border-primary/20 bg-primary/10 p-2.5">
              <span className="font-semibold text-primary">Skor Numerasi</span>
              <div className="text-xl font-bold text-primary">{s.num}</div>
              <span className="text-[10px] text-outline">Skala 0-100</span>
            </div>
          </div>

          <div className="space-y-1 rounded-xl border border-outline-variant/60 bg-surface-container-low p-3">
            <span className="flex items-center gap-1 font-bold text-primary">
              <Icon name="insights" className="text-[16px] text-secondary" /> Interpretasi Karakteristik Akademik
            </span>
            <p className="leading-relaxed text-on-surface-variant">
              Pada dataset demo 4 Kota DKI, institusi {s.level} di {s.kota} ini memiliki nilai literasi{' '}
              {s.lit >= baseline.lit ? 'di atas' : 'mendekati'} rata-rata DKI ({baseline.lit}) dan numerasi{' '}
              {s.num >= baseline.num ? 'di atas' : 'mendekati'} rata-rata DKI ({baseline.num}).
            </p>
          </div>
        </div>

        <div className="flex items-center justify-between border-t border-outline-variant/60 bg-surface-container-low p-4 text-xs">
          <button onClick={closeDetail} className="rounded-lg px-3 py-1.5 text-on-surface-variant hover:bg-surface-container">
            Tutup
          </button>
          <button
            onClick={handleCompare}
            className="flex items-center gap-1 rounded-xl bg-secondary px-4 py-1.5 font-bold text-white hover:bg-[#00685e]"
          >
            <Icon name={selected ? 'check' : 'add_circle'} className="text-[16px]" />
            {selected ? 'Terpilih di Perbandingan' : 'Tambahkan ke Perbandingan'}
          </button>
        </div>
      </div>
    </div>
  )
}