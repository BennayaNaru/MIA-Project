import { useState } from "react";
import { Link, useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "../components/api-authorization/AuthContext";
import AuthShowcase from "../components/auth/AuthShowcase";
import AuthField from "../components/auth/AuthField";
import TurnstileWidget from "../components/auth/TurnstileWidget";
import Icon from "../components/common/Icon";
import { usePublicConfig } from "../hooks/usePublicConfig";

const parseAuthError = (error) => {
  const fallback = error?.message || "Gagal memproses permintaan. Coba lagi.";

  try {
    const body = JSON.parse(error.message);
    const validationMessage = body.errors ? Object.values(body.errors).flat()[0] : null;
    return body.detail || validationMessage || body.title || fallback;
  } catch {
    return fallback;
  }
};


const tabClass = (active) =>
  "flex-1 py-2.5 px-4 rounded-lg text-sm text-center transition-all " +
  (active
    ? "bg-white text-primary font-semibold shadow-sm"
    : "text-on-surface-variant hover:text-on-surface");

export default function LoginPage() {
  const [params, setParams] = useSearchParams();
  const navigate = useNavigate();
  const location = useLocation();
  const mode = params.get("mode") === "daftar" ? "daftar" : "masuk";
  const { turnstileEnabled, turnstileSiteKey, ready: configReady } = usePublicConfig();

  const [form, setForm] = useState({ nama: "", email: "", password: "", konfirmasi: "", ingat: true, setuju: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [turnstileToken, setTurnstileToken] = useState("");
  const [turnstileReset, setTurnstileReset] = useState(0);
  const { login, register } = useAuth();

  const turnstileRequired = mode === "daftar" && turnstileEnabled && Boolean(turnstileSiteKey);

  const set = (key) => (e) =>
    setForm((f) => ({ ...f, [key]: e.target.type === "checkbox" ? e.target.checked : e.target.value }));

  const switchMode = (m) => {
    setError("");
    setTurnstileToken("");
    setTurnstileReset((value) => value + 1);
    setParams(m === "daftar" ? { mode: "daftar" } : {});
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");
    if (mode === "daftar" && form.password !== form.konfirmasi) {
      setError("Kata sandi dan konfirmasinya belum sama.");
      return;
    }
    if (turnstileRequired && !turnstileToken) {
      setError("Selesaikan verifikasi keamanan terlebih dahulu.");
      return;
    }
    setLoading(true);
    try {
      if (mode === "daftar") {
        await register(form.email, form.password, turnstileToken);
      }
      await login(form.email, form.password, form.ingat);
      navigate(location.state?.returnUrl || "/", { replace: true });
    } catch (err) {
      // Turnstile tokens are single-use, so request a fresh one after any failure.
      setTurnstileToken("");
      setTurnstileReset((value) => value + 1);
      setError(parseAuthError(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="min-h-screen w-full flex flex-col lg:flex-row">
      <AuthShowcase />

      <div className="w-full lg:w-1/2 min-h-screen bg-white flex flex-col justify-between p-6 sm:p-8 lg:p-12 xl:p-16">
        <div className="flex justify-end mb-8">
          <Link
            to="/cari"
            className="inline-flex items-center gap-2 px-3.5 py-1.5 rounded-lg border border-outline-variant text-primary hover:bg-surface-container text-sm font-semibold transition-all"
          >
            <Icon name="arrow_back" className="text-[18px]" />
            <span>Kembali ke Direktori Sekolah</span>
          </Link>
        </div>

        <div className="w-full max-w-md mx-auto my-auto">
          <div role="tablist" className="p-1 rounded-xl bg-surface-container border border-outline-variant flex mb-8">
            <button type="button" role="tab" aria-selected={mode === "masuk"} onClick={() => switchMode("masuk")} className={tabClass(mode === "masuk")}>
              Masuk
            </button>
            <button type="button" role="tab" aria-selected={mode === "daftar"} onClick={() => switchMode("daftar")} className={tabClass(mode === "daftar")}>
              Daftar Akun
            </button>
          </div>

          <h2 className="text-3xl font-display font-bold text-primary tracking-tight">
            {mode === "masuk" ? "Selamat Datang di OpenSch" : "Buat Akun OpenSch"}
          </h2>
          <p className="text-sm text-on-surface-variant mt-1.5 mb-6">
            {mode === "masuk"
              ? "Masukkan email dan kata sandi Anda."
              : "Daftar untuk menyimpan pilihan perbandingan dan memakai ringkasan AI."}
          </p>



          <form onSubmit={onSubmit} className="space-y-4">
            {mode === "daftar" && (
              <AuthField id="nama" label="Nama Lengkap" icon="person" required placeholder="Nama lengkap Anda" value={form.nama} onChange={set("nama")} />
            )}
            <AuthField id="email" label="Alamat Email" icon="alternate_email" type="email" required placeholder="nama@domain.com" value={form.email} onChange={set("email")} />
            <AuthField
              id="password"
              label="Kata Sandi"
              icon="lock"
              type="password"
              required
              minLength={mode === "daftar" ? 8 : undefined}
              placeholder={mode === "daftar" ? "Minimal 8 karakter" : "Kata sandi"}
              value={form.password}
              onChange={set("password")}
            />
            {mode === "daftar" && (
              <AuthField id="konfirmasi" label="Konfirmasi Kata Sandi" icon="check_circle" type="password" required placeholder="Ulangi kata sandi" value={form.konfirmasi} onChange={set("konfirmasi")} />
            )}

            {mode === "masuk" ? (
              <label className="flex items-center gap-2 text-sm text-on-surface-variant">
                <input type="checkbox" checked={form.ingat} onChange={set("ingat")} className="rounded border-outline-variant text-secondary focus:ring-secondary" />
                Ingat saya selama 30 hari
              </label>
            ) : (
              <label className="flex items-start gap-2 text-sm text-on-surface-variant">
                <input type="checkbox" required checked={form.setuju} onChange={set("setuju")} className="mt-0.5 rounded border-outline-variant text-secondary focus:ring-secondary" />
                <span>Saya menyetujui syarat penggunaan layanan demo ini. Data yang tampil adalah data sintetis.</span>
              </label>
            )}

            {turnstileRequired && (
              <div className="rounded-lg border border-outline-variant/60 px-3 py-2">
                <TurnstileWidget
                  key={turnstileReset}
                  siteKey={turnstileSiteKey}
                  action="register"
                  onToken={setTurnstileToken}
                  onExpire={() => setTurnstileToken("")}
                  onError={() => setTurnstileToken("")}
                />
                <p className="text-[11px] text-outline">
                  Dilindungi Cloudflare Turnstile. Tidak perlu memecahkan teka-teki.
                </p>
              </div>
            )}

            {error && (
              <p role="alert" className="flex items-center gap-2 text-sm text-error bg-error-container/40 rounded-lg px-3 py-2">
                <Icon name="error" className="text-[18px]" /> {error}
              </p>
            )}

            <button
              type="submit"
              disabled={loading || (mode === "daftar" && !configReady)}
              className="w-full py-3 rounded-xl bg-primary hover:bg-[#122D42] disabled:opacity-60 text-white text-sm font-bold flex items-center justify-center gap-2 shadow-sm transition-all"
            >
              <span>{loading ? "Memproses..." : mode === "masuk" ? "Masuk ke Akun OpenSch" : "Daftar Sekarang"}</span>
              {!loading && <Icon name={mode === "masuk" ? "login" : "how_to_reg"} className="text-[20px]" />}
            </button>
          </form>
        </div>

        <p className="mt-8 pt-4 border-t border-outline-variant/60 text-xs text-outline">
          © {new Date().getFullYear()} OpenSch. Akun dan data di halaman ini hanya untuk demo.
        </p>
      </div>
    </main>
  );
}