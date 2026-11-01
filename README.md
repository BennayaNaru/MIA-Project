# OpenSch. – Papan Sekolah Terbuka

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
   git clone -b frontend https://github.com/BennayaNaru/MIA-Project.git
   cd MIA-Project (nama repo)
   ```

2. Jalankan **frontend** (cukup langkah ini untuk melihat seluruh tampilan saat ini):

   ```bash
   cd OpenSch-web/src/Web/ClientApp
   npm install
   npm install @tailwindcss/forms
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


## Tim Doa Ibu Selamanya

| No | Nama |
|---|---|
| 1 | Nathaniel Dennis Y.S. |
| 2 | Bennaya Muhammad Caesar |
| 3 | Hadi Sumanjaya |
