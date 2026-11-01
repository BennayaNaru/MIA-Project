import { Link } from "react-router-dom";
import { STEPS } from "../../constants/site";
import Icon from "../common/Icon";

export default function HowItWorks() {
  return (
    <div className="bg-surface-container-low border-y border-outline-variant/60 py-10 px-4 sm:px-6">
      <div className="max-w-7xl mx-auto">
        <div className="text-center max-w-xl mx-auto mb-8">
          <h2 className="text-2xl font-display font-bold text-primary">Cara Kerja Evaluasi Perbandingan</h2>
          <p className="text-xs text-on-surface-variant mt-1">
            Empat langkah membandingkan capaian sekolah dengan aturan perbandingan yang adil.
          </p>
        </div>

        <ol className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {STEPS.map((s, i) => (
            <li key={s.title} className="bg-white p-5 rounded-xl border border-outline-variant flex flex-col justify-between">
              <div>
                <div
                  className={
                    "w-8 h-8 rounded-full text-white font-bold flex items-center justify-center text-xs mb-3 " +
                    (s.accent ? "bg-secondary" : "bg-primary")
                  }
                >
                  {i + 1}
                </div>
                <h3 className="text-sm font-bold text-primary mb-1">{s.title}</h3>
                <p className="text-xs text-on-surface-variant">{s.desc}</p>
              </div>
              <p className="text-[11px] text-secondary font-semibold mt-3 pt-2 border-t border-outline-variant/40">{s.hint}</p>
            </li>
          ))}
        </ol>

        <div className="mt-8 text-center">
          <Link
            to="/cari"
            className="px-6 py-2.5 rounded-xl bg-primary hover:bg-[#122D42] text-white text-xs font-bold inline-flex items-center gap-2 shadow-sm transition-all"
          >
            <Icon name="explore" className="text-[18px]" />
            <span>Buka Direktori Sekolah</span>
          </Link>
        </div>
      </div>
    </div>
  );
}