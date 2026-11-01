import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { HERO, INDICATORS } from "../../constants/site";
import Icon from "../common/Icon";
import DemoDataBadge from "../common/DemoDataBadge";

const TONE = {
  secondary: { bar: "bg-secondary", text: "text-secondary", icon: "text-secondary" },
  primary: { bar: "bg-primary", text: "text-primary", icon: "text-primary" },
};

function IndicatorPreview() {
  return (
    <div className="bg-white p-6 rounded-2xl shadow-sm border border-outline-variant space-y-4">
      <div className="pb-3 border-b border-outline-variant/60">
        <p className="text-[11px] font-bold text-outline">Pratinjau indikator inti</p>
        <h3 className="text-base font-display font-bold text-primary">Rata-rata 4 Kota Administrasi DKI</h3>
      </div>

      {INDICATORS.map((i) => {
        const t = TONE[i.tone];
        return (
          <div key={i.label}>
            <div className="flex justify-between items-baseline mb-1">
              <span className="text-xs font-semibold text-on-surface flex items-center gap-1">
                <Icon name={i.icon} className={"text-[16px] " + t.icon} /> {i.label}
              </span>
              <span className="text-sm font-bold text-primary">
                {i.value.toFixed(1)} <span className="text-xs text-outline font-normal">/100</span>
              </span>
            </div>
            <div className="w-full bg-surface-container rounded-full h-2.5 overflow-hidden">
              <div className={"h-2.5 rounded-full " + t.bar} style={{ width: i.value + "%" }} />
            </div>
            <div className="flex justify-between text-[11px] text-outline mt-1">
              <span>Target minimum: {i.min.toFixed(1)}</span>
              <span className={"font-semibold " + t.text}>+{(i.value - i.min).toFixed(1)} di atas standar</span>
            </div>
          </div>
        );
      })}

      <div className="p-3 bg-surface-container-low rounded-xl border border-outline-variant/60 flex items-center justify-between gap-3">
        <div className="text-xs">
          <p className="font-bold text-primary">Ingin membandingkan sekolah?</p>
          <p className="text-on-surface-variant text-[11px]">Pilih 2–3 sekolah di halaman Cari Sekolah.</p>
        </div>
        <Link to="/cari" className="px-3 py-1.5 bg-secondary text-white rounded-lg text-xs font-bold hover:bg-[#00685e] transition-colors">
          Jelajahi
        </Link>
      </div>
    </div>
  );
}

export default function Hero() {
  const [q, setQ] = useState("");
  const navigate = useNavigate();

  const onSubmit = (e) => {
    e.preventDefault();
    const text = q.trim();
    navigate(text ? "/cari?q=" + encodeURIComponent(text) : "/cari");
  };

  return (
    <div className="bg-gradient-to-b from-surface-container-low via-surface to-surface border-b border-outline-variant/60 py-12 px-4 sm:px-6">
      <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-12 gap-8 items-center">
        <div className="lg:col-span-7 space-y-5">
          <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-secondary/10 border border-secondary/20 text-secondary text-xs font-bold">
            <Icon name="verified" className="text-[16px]" />
            <span>{HERO.badge}</span>
          </div>
          <h1 className="text-3xl sm:text-4xl lg:text-5xl font-display font-extrabold text-primary tracking-tight leading-tight">
            {HERO.title}
          </h1>
          <p className="text-sm sm:text-base text-on-surface-variant max-w-xl leading-relaxed">{HERO.description}</p>

          <form
            onSubmit={onSubmit}
            className="bg-white p-3 rounded-2xl shadow-sm border border-outline-variant flex flex-col sm:flex-row items-center gap-2 max-w-lg"
          >
            <label className="flex-1 w-full flex items-center gap-2 px-3 py-2 bg-surface rounded-xl border border-outline-variant/60">
              <Icon name="search" className="text-outline text-[18px]" />
              <input
                type="text"
                value={q}
                onChange={(e) => setQ(e.target.value)}
                placeholder={HERO.searchPlaceholder}
                aria-label="Cari sekolah"
                className="bg-transparent border-0 p-0 text-sm focus:ring-0 w-full placeholder:text-outline"
              />
            </label>
            <button
              type="submit"
              className="w-full sm:w-auto px-5 py-2 rounded-xl bg-primary hover:bg-[#122D42] text-white text-xs font-bold flex items-center justify-center gap-1.5 shadow-sm transition-all"
            >
              <span>Cari Sekarang</span>
              <Icon name="arrow_forward" className="text-[16px]" />
            </button>
          </form>

          <div className="flex flex-wrap items-center gap-4 text-xs text-outline font-medium">
            {HERO.points.map((p) => (
              <span key={p} className="flex items-center gap-1">
                <Icon name="check_circle" className="text-secondary text-[16px]" /> {p}
              </span>
            ))}
          </div>
          <DemoDataBadge />
        </div>

        <div className="lg:col-span-5">
          <IndicatorPreview />
        </div>
      </div>
    </div>
  );
}