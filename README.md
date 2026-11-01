# OpenSch – Papan Sekolah Terbuka

Aplikasi web yang membantu orang tua mencari sekolah dan membandingkan 2–3 sekolah lewat tabel angka dan penjelasan singkat, serta menampilkan peringkat literasi dan numerasi dengan konteks yang jelas. Seluruh data adalah **data demo (sintetis), bukan data resmi**.

## Tech Stack

- **Frontend:** React (Vite), React Router, Tailwind CSS
- **Backend:** ASP.NET Core (.NET 10), Clean Architecture
- **Database:** SQLite (sementara untuk pengembangan)

## Prasyarat

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20.19 atau lebih baru (disarankan LTS terbaru)
- Git

## Cara Menjalankan

1. Clone repository:

   ```bash
   git clone https://github.com/<username>/<nama-repo>.git
   cd <nama-repo>
   ```

2. Jalankan **frontend** (cukup langkah ini untuk melihat seluruh tampilan saat ini):

   ```bash
   cd src/Web/ClientApp
   npm install
   npm run dev
   ```

   Buka http://localhost:5173

## Progress Saat Ini

- [x] Landing page (Beranda)
- [x] Cari Sekolah (filter, tampilan kartu/tabel, modal detail)
- [x] Keranjang perbandingan dan halaman Bandingkan
- [x] Tentang Data (metodologi dan batasan data)
- [x] Halaman Masuk/Daftar
- [ ] Integrasi frontend dengan API backend (`/api/schools`, `/api/compare`, `/api/rankings`)
- [ ] Halaman Peringkat dan ringkasan otomatis dari backend

Catatan: saat ini halaman frontend memakai data sintetis yang disimpan di `src/Web/ClientApp/src/data/schools.js`.

## Tim

| Nama | Peran |
|---|---|
| <Nathaniel Dennis Yosafat Sidauruk> | <Peran> |
| <Bennaya Muhammad Caesar> | <Peran> |
| <Hadi Sumanjaya> | <Peran> |
