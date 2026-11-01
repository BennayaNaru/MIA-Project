import { useNavigate } from 'react-router-dom'
import Icon from '../common/Icon'
import { useCompare, MAX_COMPARE } from '../../context/CompareContext'

export default function CompareDock() {
  const { selectedSchools, remove, clear } = useCompare()
  const navigate = useNavigate()
  const visible = selectedSchools.length > 0

  return (
    <div
      className={`fixed bottom-4 left-1/2 z-50 w-[95%] max-w-3xl -translate-x-1/2 transform transition-all duration-300 ${
        visible ? 'translate-y-0 opacity-100' : 'pointer-events-none translate-y-32 opacity-0'
      }`}
    >
      <div className="flex flex-col items-center justify-between gap-3 rounded-2xl border border-white/20 bg-primary p-3 text-white shadow-2xl sm:flex-row">
        <div className="custom-scroll flex w-full items-center gap-2 overflow-x-auto pb-1 sm:w-auto sm:pb-0">
          <span className="shrink-0 rounded-lg bg-white/20 px-2 py-0.5 text-xs font-bold text-secondary-container">
            {selectedSchools.length} dari {MAX_COMPARE}
          </span>
          <div className="flex items-center gap-1.5">
            {selectedSchools.map((s) => (
              <div key={s.id} className="inline-flex shrink-0 items-center gap-1 rounded-lg border border-white/20 bg-white/20 px-2 py-0.5 text-[11px] text-white">
                <span className="max-w-[120px] truncate font-medium">{s.name}</span>
                <button onClick={() => remove(s.id)} className="p-0.5 text-white/80 hover:text-white" aria-label="Hapus">
                  <Icon name="close" className="text-[14px]" />
                </button>
              </div>
            ))}
          </div>
        </div>

        <div className="flex w-full shrink-0 items-center justify-end gap-2 sm:w-auto">
          <button onClick={clear} className="px-2 py-1 text-xs text-outline-variant hover:text-white">
            Reset
          </button>
          <button
            onClick={() => navigate('/bandingkan')}
            className="flex items-center gap-1.5 rounded-xl bg-secondary px-4 py-1.5 text-xs font-bold text-white shadow transition-colors hover:bg-[#00685e]"
          >
            <span>Bandingkan Sekarang</span>
            <Icon name="arrow_forward" className="text-[16px]" />
          </button>
        </div>
      </div>
    </div>
  )
}