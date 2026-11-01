export const KECAMATAN_MAP = {
  'Jakarta Selatan': ['Kebayoran Baru', 'Cilandak', 'Pasar Minggu', 'Tebet', 'Jagakarsa'],
  'Jakarta Timur': ['Jatinegara', 'Duren Sawit', 'Cakung', 'Kramat Jati', 'Ciracas'],
  'Jakarta Utara': ['Kelapa Gading', 'Tanjung Priok', 'Penjaringan', 'Koja', 'Pademangan'],
  'Jakarta Barat': ['Kebon Jeruk', 'Grogol Petamburan', 'Kembangan', 'Cengkareng', 'Palmerah'],
}

export const BENCHMARK = 81.1


const RAW = [
  { id: 'DKI-01', name: 'SD Negeri Menteng Atas 01 Sintetis', kota: 'Jakarta Selatan', kec: 'Tebet', level: 'SD', lit: 84.5, num: 81.2 },
  { id: 'DKI-02', name: 'SMP Cendekia Jaya Mandiri', kota: 'Jakarta Selatan', kec: 'Cilandak', level: 'SMP', lit: 88.0, num: 86.5 },
  { id: 'DKI-03', name: 'SMK Teknologi Bangsa Pratama', kota: 'Jakarta Selatan', kec: 'Kebayoran Baru', level: 'SMK', lit: 79.4, num: 85.0 },
  { id: 'DKI-04', name: 'SMP Teladan Jagakarsa Mandiri', kota: 'Jakarta Selatan', kec: 'Jagakarsa', level: 'SMP', lit: 83.0, num: 81.5 },
  { id: 'DKI-05', name: 'SD Harapan Bangsa Cakung', kota: 'Jakarta Timur', kec: 'Cakung', level: 'SD', lit: 80.2, num: 76.8 },
  { id: 'DKI-06', name: 'SMP Negeri Rawamangun Maju', kota: 'Jakarta Timur', kec: 'Duren Sawit', level: 'SMP', lit: 82.6, num: 79.5 },
  { id: 'DKI-07', name: 'SMK Karya Mandiri Otomotif', kota: 'Jakarta Timur', kec: 'Jatinegara', level: 'SMK', lit: 76.5, num: 88.2 },
  { id: 'DKI-08', name: 'SMK Insan Mulia Ciracas', kota: 'Jakarta Timur', kec: 'Ciracas', level: 'SMK', lit: 78.2, num: 84.0 },
  { id: 'DKI-09', name: 'SD Pesisir Bintang Bahari', kota: 'Jakarta Utara', kec: 'Tanjung Priok', level: 'SD', lit: 78.0, num: 74.2 },
  { id: 'DKI-10', name: 'SMP Samudera Indah Gading', kota: 'Jakarta Utara', kec: 'Kelapa Gading', level: 'SMP', lit: 86.4, num: 84.0 },
  { id: 'DKI-11', name: 'SMK Maritim & Logistik Perkasa', kota: 'Jakarta Utara', kec: 'Penjaringan', level: 'SMK', lit: 75.8, num: 83.5 },
  { id: 'DKI-12', name: 'SMP Citra Bahari Pademangan', kota: 'Jakarta Utara', kec: 'Pademangan', level: 'SMP', lit: 81.5, num: 78.0 },
  { id: 'DKI-13', name: 'SD Citra Melati Kembangan', kota: 'Jakarta Barat', kec: 'Kembangan', level: 'SD', lit: 81.5, num: 78.0 },
  { id: 'DKI-14', name: 'SMP Prestasi Cemerlang Grogol', kota: 'Jakarta Barat', kec: 'Grogol Petamburan', level: 'SMP', lit: 83.2, num: 80.4 },
  { id: 'DKI-15', name: 'SMK Rekayasa Digital Informatika', kota: 'Jakarta Barat', kec: 'Kebon Jeruk', level: 'SMK', lit: 82.0, num: 89.6 },
  { id: 'DKI-16', name: 'SMK Grafika Cengkareng Unggul', kota: 'Jakarta Barat', kec: 'Cengkareng', level: 'SMK', lit: 77.0, num: 82.5 },
]

export const ALL_SCHOOLS = RAW.map((s) => ({
  ...s,
  composite: Number(((s.lit + s.num) / 2).toFixed(1)),
}))
  .sort((a, b) => b.composite - a.composite || b.lit - a.lit)
  .map((s, i) => ({ ...s, rank: i + 1 }))

export const BASELINE = { lit: 80.5, num: 81.8 }