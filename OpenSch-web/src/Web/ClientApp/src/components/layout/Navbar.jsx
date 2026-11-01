import { Link, NavLink, useNavigate } from "react-router-dom";
import { SITE, NAV_LINKS, REGIONS } from "../../constants/site";
import Icon from "../common/Icon";
import { useCompare } from '../../context/CompareContext'

const linkClass = ({ isActive }) =>
  "inline-flex items-center gap-1.5 pb-1 whitespace-nowrap transition-all " +
  (isActive
    ? "text-secondary border-b-2 border-secondary"
    : "text-on-surface-variant hover:text-primary");

export default function Navbar() {
  const navigate = useNavigate();
  const { selectedIds } = useCompare();
  const pickRegion = (value) =>
    navigate(value ? "/cari?kota=" + encodeURIComponent(value) : "/cari");

  return (
    <header className="bg-surface-container-lowest sticky top-0 z-40 border-b border-outline-variant shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 flex items-center justify-between h-16">
        <Link to="/" className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary flex items-center justify-center shadow-sm">
            <Icon name="insights" className="text-2xl text-secondary-container" />
          </div>
          <div>
            <span className="text-xl font-display font-extrabold tracking-tight text-primary">
              {SITE.name}
            </span>
            <p className="text-xs text-outline hidden lg:block">{SITE.tagline}</p>
          </div>
        </Link>

        <nav className="hidden md:flex items-center space-x-6 text-sm font-semibold">
          {NAV_LINKS.map((l) => (
            <NavLink key={l.to} to={l.to} end={l.end} className={linkClass}>
              {l.label}
            </NavLink>
          ))}
        </nav>

        <div className="flex items-center gap-2">
          <div className="relative group hidden sm:block">
            <button
              type="button"
              className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-surface-container-low hover:bg-surface-container border border-outline-variant text-xs font-semibold text-primary"
            >
              <Icon name="location_city" className="text-[16px] text-secondary" />
              <span>Pilih kota</span>
              <Icon name="expand_more" className="text-[16px] text-outline" />
            </button>
            <div className="absolute right-0 mt-1 w-56 bg-white rounded-xl shadow-xl border border-outline-variant p-1.5 hidden group-hover:block group-focus-within:block z-50">
              {REGIONS.map((r) => (
                <button
                  key={r.label}
                  type="button"
                  onClick={() => pickRegion(r.value)}
                  className="w-full text-left px-2.5 py-1.5 rounded-lg hover:bg-surface-container-low text-xs font-medium"
                >
                  {r.label}
                </button>
              ))}
            </div>
          </div>
          <Link
            to="/masuk"
            className="px-3 py-1.5 rounded-lg border border-outline-variant hover:bg-surface-container text-xs font-semibold text-primary transition-all"
          >
            Masuk
          </Link>
          <Link
            to="/masuk?mode=daftar"
            className="hidden sm:inline-flex px-3 py-1.5 rounded-lg bg-primary hover:bg-[#122D42] text-white text-xs font-semibold transition-all shadow-sm"
          >
            Daftar
          </Link>
        </div>
      </div>

      <nav className="md:hidden flex gap-5 overflow-x-auto px-4 pb-2 text-sm font-semibold">
        {NAV_LINKS.map((l) => (
          <NavLink key={l.to} to={l.to} end={l.end} className={linkClass}>
            {l.label}
            {l.to === "/bandingkan" && selectedIds.length > 0 && (
              <span className="rounded-full bg-secondary px-1.5 text-[10px] font-bold text-white">
                {selectedIds.length}
              </span>
            )}
          </NavLink>
        ))}
      </nav>
    </header>
  );
}