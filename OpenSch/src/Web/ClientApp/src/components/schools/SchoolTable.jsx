export default function SchoolTable({ schools, selectedIds, onToggle, onDetail }) {
  return (
    <div className="overflow-hidden rounded-2xl border border-outline-variant bg-white shadow-sm">
      <div className="custom-scroll overflow-x-auto">
        <table className="w-full border-collapse text-left text-xs">
          <thead>
            <tr className="border-b border-outline-variant bg-surface-container-low font-semibold text-outline">
              <th className="w-10 p-3 text-center">Bandingkan</th>
              <th className="p-3">Peringkat Demo</th>
              <th className="p-3">Nama Sekolah</th>
              <th className="p-3">Kota Administrasi</th>
              <th className="p-3">Kecamatan</th>
              <th className="p-3">Jenjang</th>
              <th className="p-3">Literasi</th>
              <th className="p-3">Numerasi</th>
              <th className="p-3">Skor Komposit</th>
              <th className="p-3 text-right">Aksi</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-outline-variant/40">
            {schools.map((s) => {
              const selected = selectedIds.includes(s.id)
              return (
                <tr key={s.id} className={`transition-colors hover:bg-surface-container-low ${selected ? 'bg-secondary/5' : ''}`}>
                  <td className="p-3 text-center">
                    <input
                      type="checkbox"
                      checked={selected}
                      onChange={() => onToggle(s.id)}
                      className="h-4 w-4 rounded border-outline text-secondary focus:ring-secondary"
                    />
                  </td>
                  <td className="p-3 font-bold text-primary">#{s.rank}</td>
                  <td onClick={() => onDetail(s.id)} className="cursor-pointer p-3 font-semibold text-primary hover:underline">
                    {s.name}
                  </td>
                  <td className="p-3">{s.kota}</td>
                  <td className="p-3 text-outline">Kec. {s.kec}</td>
                  <td className="p-3">
                    <span className="rounded bg-primary px-2 py-0.5 text-[10px] font-bold text-white">{s.level}</span>
                  </td>
                  <td className="p-3 font-bold text-secondary">{s.lit}</td>
                  <td className="p-3 font-bold text-primary">{s.num}</td>
                  <td className="p-3 font-extrabold text-primary">{s.composite}</td>
                  <td className="p-3 text-right">
                    <button
                      onClick={() => onDetail(s.id)}
                      className="rounded bg-surface-container px-2 py-1 font-semibold text-primary hover:bg-surface-container-high"
                    >
                      Detail
                    </button>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}