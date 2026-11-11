import { SITE } from "../../constants/site";
import { AUTH } from "../../constants/auth";
import Icon from "../common/Icon";

export default function AuthShowcase() {
  return (
    <div className="relative w-full lg:w-1/2 min-h-[420px] lg:min-h-screen flex flex-col justify-between gap-8 p-6 lg:p-12 overflow-hidden text-white bg-primary">
      <img
        src="/images/kampus.jpg"
        alt=""
        onError={(e) => (e.currentTarget.style.display = "none")}
        className="absolute inset-0 w-full h-full object-cover pointer-events-none"
      />
      <div className="absolute inset-0 bg-gradient-to-t from-primary/80 via-primary-container/50 to-primary/30" />  
    
      <div className="relative z-10 flex items-center gap-3">
        <div className="w-10 h-10 rounded-lg bg-white flex items-center justify-center shadow-md">
          <Icon name="analytics" className="text-secondary text-2xl" />
        </div>
        <span className="text-xl font-display font-bold tracking-tight">{SITE.name}</span>
      </div>

      <div className="relative z-10 max-w-xl">
        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-lg bg-white/10 border border-white/20 mb-4 text-secondary-container text-sm font-semibold backdrop-blur-sm">
          <Icon name="verified" className="text-[16px]" />
          <span>{AUTH.badge}</span>
        </div>
        <h1 className="text-3xl lg:text-5xl font-display font-bold leading-tight tracking-tight mb-4">{AUTH.title}</h1>
        <p className="text-base text-white/85 leading-relaxed max-w-lg mb-6">{AUTH.description}</p>
        <div className="grid grid-cols-3 gap-3 pt-4 border-t border-white/25">
          {AUTH.chips.map((c) => (
            <div key={c.label} className="p-3 rounded-lg bg-white/10 backdrop-blur-md border border-white/20">
              <div className="text-2xl font-display font-bold text-secondary-container leading-none mb-1">{c.value}</div>
              <p className="text-xs text-white/80">{c.label}</p>
            </div>
          ))}
        </div>
      </div>

      <div className="relative z-10 pt-4 border-t border-white/20 flex items-center gap-2 text-xs text-white/80">
        <Icon name="science" className="text-[16px]" />
        Data demo (sintetis), bukan data resmi
      </div>
    </div>
  );
}