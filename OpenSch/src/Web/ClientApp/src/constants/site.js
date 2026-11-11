export const SITE = {
  name: "OpenSch",
  tagline: "Simulasi data mutu pendidikan 4 kota administrasi DKI",
};

export const NAV_LINKS = [
  { to: '/', label: 'Beranda' },
  { to: '/cari', label: 'Cari Sekolah' },
  { to: '/peringkat', label: 'Peringkat' },
  { to: '/bandingkan', label: 'Bandingkan' },
  { to: '/tentang-data', label: 'Tentang Data' },
]

export const REGIONS = [
  { label: "DKI 4 Wilayah (Semua)", value: "" },
  { label: "Jakarta Selatan", value: "Jakarta Selatan" },
  { label: "Jakarta Timur", value: "Jakarta Timur" },
  { label: "Jakarta Utara", value: "Jakarta Utara" },
  { label: "Jakarta Barat", value: "Jakarta Barat" },
];

export const HERO = {
  badge: "Eksplorasi mutu 4 kota administrasi DKI Jakarta (SD, SMP, SMA, SMK)",
  title: "Temukan Sekolah di Jakarta, Pahami Kualitasnya.",
  description:
    "Platform independen penyedia metrik literasi dan numerasi berbasis data objektif. Membantu orang tua dan pendidik mengevaluasi profil sekolah jenjang SD, SMP, SMA, dan SMK di 4 wilayah kota administrasi DKI Jakarta tanpa bias komersial.",
  searchPlaceholder: "Ketik nama sekolah atau kecamatan DKI...",
  points: ["Khusus 4 kota DKI", "4 jenjang (SD, SMP, SMA, SMK)", "Skor komposit terbuka"],
};

export const INDICATORS = [
  { label: "Indeks Literasi Wilayah", icon: "menu_book", value: 80.5, min: 65.0, tone: "secondary" },
  { label: "Indeks Numerasi Wilayah", icon: "calculate", value: 81.8, min: 60.0, tone: "primary" },
];

export const STATS = [
  { value: "24", label: "Sekolah Terdata", note: "Jenjang SD, SMP, SMA, SMK" },
  { value: "4", label: "Kota Administrasi DKI", note: "Jaksel, Jaktim, Jakut, Jakbar" },
  { value: "4", label: "Jenjang Pendidikan", note: "SD, SMP, SMA, SMK" },
  { value: "100%", label: "Formula Transparan & Terbuka", note: "Data sintetis untuk uji coba", accent: true },
];

export const STEPS = [
  { title: "Pilih Kota Administrasi", desc: "Gunakan filter 4 kota administrasi DKI (Jakarta Selatan, Timur, Utara, Barat) dan kecamatannya.", hint: "Contoh: Cilandak, Tebet" },
  { title: "Filter Jenjang & Skor", desc: "Saring berdasarkan jenjang (SD, SMP, SMA, SMK) serta batas minimum nilai literasi dan numerasi.", hint: "SD, SMP, SMA, dan SMK" },
  { title: "Bandingkan Berdampingan", desc: "Tandai 2 hingga 3 sekolah. Sistem memberi catatan khusus jika jenjangnya berbeda.", hint: "Aturan perbandingan adil", accent: true },
  { title: "Ringkasan Berbasis Aturan", desc: "Dapatkan ringkasan dari aturan dan template, dengan opsi narasi AI jika diaktifkan.", hint: "Transparan & bebas bias" },
];