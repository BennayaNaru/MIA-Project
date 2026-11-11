import Icon from '../common/Icon'

function Metric({ icon, label, value, text, bar }) {
  return (
    <div>
      <div className="mb-0.5 flex items-center justify-between">
        <span className="flex items-center gap-1 text-on-surface-variant">
          <Icon name={icon} className={`text-[14px] ${text}`} /> {label}
        </span>
        <span className={`font-bold ${text}`}>
          {value} <span className="text-[10px] font-normal text-outline">/100</span>
        </span>
      </div>
      <div className="h-1.5 w-full rounded-full bg-surface-container">
        <div className={`h-1.5 rounded-full ${bar}`} style={{ width: `${value}%` }} />
      </div>
    </div>
  )
}

export default function SchoolCard({ school: s, selected, onToggle, onDetail, benchmark = 0 }) {
  const delta = Number((s.composite - benchmark).toFixed(1))

  return (
    <div
      className={`flex flex-col justify-between rounded-2xl border bg-white p-4 text-xs shadow-sm transition-all hover:border-secondary/60 ${
        selected ? 'border-secondary shadow-md ring-1 ring-secondary' : 'border-outline-variant'
      }`}
    >
      <div>
        <div className="mb-2 flex items-center justify-between gap-1">
          <div className="flex flex-wrap items-center gap-1">
            <span className="rounded bg-primary px-2 py-0.5 font-bold text-white">{s.level}</span>
            <span className="rounded bg-surface-container px-2 py-0.5 font-medium text-on-surface">{s.kota}</span>
            <span className="rounded bg-surface-container-low px-2 py-0.5 text-[10px] text-outline">Kec. {s.kec}</span>
          </div>
          <span
            className={`rounded-full px-2 py-0.5 font-bold ${
              s.rank <= 3 ? 'bg-secondary/15 text-secondary' : 'bg-surface-container text-outline'
            }`}
          >
            #{s.rank} Demo
          </span>
        </div>

        <h3
          onClick={() => onDetail(s.id)}
          className="line-clamp-1 cursor-pointer text-sm font-bold text-primary transition-colors hover:text-secondary"
        >
          {s.name}
        </h3>

        <div className="mt-3 space-y-2 border-t border-outline-variant/40 pt-2">
          <Metric icon="menu_book" label="Literasi" value={s.lit} text="text-secondary" bar="bg-secondary" />
          <Metric icon="calculate" label="Numerasi" value={s.num} text="text-primary" bar="bg-primary" />
        </div>

        <div className="mt-3 flex items-center justify-between rounded-xl border border-outline-variant/40 bg-surface-container-low p-2">
          <div>
            <div className="text-[9px] font-bold uppercase text-outline">Skor Komposit</div>
            <div className="text-sm font-extrabold text-primary">{s.composite}</div>
          </div>
          <div className="text-right">
            <div className="text-[9px] font-bold uppercase text-outline">vs DKI 4 Kota</div>
            <div className={`text-[11px] font-bold ${delta >= 0 ? 'text-secondary' : 'text-outline'}`}>
              {delta >= 0 ? '+' : ''}{delta.toFixed(1)}
            </div>
          </div>
        </div>
      </div>

      <div className="mt-4 flex items-center justify-between border-t border-outline-variant/40 pt-2">
        <label className="inline-flex cursor-pointer select-none items-center gap-1.5">
          <input
            type="checkbox"
            checked={selected}
            onChange={() => onToggle(s.id)}
            className="h-4 w-4 rounded border-outline text-secondary focus:ring-secondary"
          />
          <span className="font-semibold text-primary">Bandingkan</span>
        </label>
        <button
          onClick={() => onDetail(s.id)}
          className="flex items-center gap-0.5 rounded-lg px-2.5 py-1 font-semibold text-primary hover:bg-surface-container"
        >
          <span>Detail</span>
          <Icon name="chevron_right" className="text-[14px]" />
        </button>
      </div>
    </div>
  )
}