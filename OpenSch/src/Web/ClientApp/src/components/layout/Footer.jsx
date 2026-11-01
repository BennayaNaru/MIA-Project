import { Link } from "react-router-dom";
import { SITE } from "../../constants/site";
import DemoDataBadge from "../common/DemoDataBadge";

export default function Footer() {
  return (
    <footer className="bg-surface-container-low border-t border-outline-variant mt-auto text-xs text-on-surface-variant">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-6 flex flex-col sm:flex-row justify-between items-center gap-4">
        <div className="space-y-1.5">
          <div>
            <span className="font-bold text-primary font-display">{SITE.name}</span>
            <span className="ml-1 text-[11px] text-secondary font-semibold">
              4 Kota Administrasi DKI Jakarta (SD, SMP, SMA, SMK)
            </span>
          </div>
          <DemoDataBadge />
          <p className="text-[11px] text-outline">
            © {new Date().getFullYear()} {SITE.name}. Seluruh data sekolah dan capaian adalah data sintetis fiktif untuk uji coba.
          </p>
        </div>
        <div className="flex flex-wrap gap-4 font-medium">
          <Link to="/tentang" className="hover:text-primary">Metodologi Penilaian</Link>
          <Link to="/tentang" className="hover:text-primary">Definisi Dataset</Link>
          <Link to="/tentang" className="hover:text-primary">Pernyataan Transparansi</Link>
        </div>
      </div>
    </footer>
  );
}