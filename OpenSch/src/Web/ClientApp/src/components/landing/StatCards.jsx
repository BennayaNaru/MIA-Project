import { STATS } from "../../constants/site";

export default function StatCards() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {STATS.map((s) => (
          <div key={s.label} className="bg-white p-4 rounded-xl border border-outline-variant shadow-sm">
            <div className={"text-2xl font-display font-extrabold " + (s.accent ? "text-secondary" : "text-primary")}>
              {s.value}
            </div>
            <div className="text-xs font-bold text-on-surface mt-1">{s.label}</div>
            <div className="text-[11px] text-outline">{s.note}</div>
          </div>
        ))}
      </div>
    </div>
  );
}