import { createContext, useContext, useState } from 'react'
import { ALL_SCHOOLS } from '../data/schools'

export const MAX_COMPARE = 3

const CompareContext = createContext(null)

export function CompareProvider({ children }) {
  const [selectedIds, setSelectedIds] = useState(['DKI-02', 'DKI-03'])
  const [detailId, setDetailId] = useState(null)

  const toggle = (id) => {
    if (selectedIds.includes(id)) {
      setSelectedIds(selectedIds.filter((x) => x !== id))
    } else if (selectedIds.length >= MAX_COMPARE) {
      alert('Batas Maksimal: Anda dapat membandingkan maksimal 3 sekolah sekaligus. Silakan hapus salah satu terlebih dahulu.')
    } else {
      setSelectedIds([...selectedIds, id])
    }
  }

  const remove = (id) => setSelectedIds(selectedIds.filter((x) => x !== id))
  const clear = () => setSelectedIds([])
  const loadDefault = () => setSelectedIds(['DKI-01', 'DKI-02', 'DKI-03'])
  const openDetail = (id) => setDetailId(id)
  const closeDetail = () => setDetailId(null)

  const selectedSchools = selectedIds
    .map((id) => ALL_SCHOOLS.find((s) => s.id === id))
    .filter(Boolean)

  return (
    <CompareContext.Provider
      value={{
        selectedIds, selectedSchools, toggle, remove, clear, loadDefault,
        detailId, openDetail, closeDetail,
      }}
    >
      {children}
    </CompareContext.Provider>
  )
}

export function useCompare() {
  return useContext(CompareContext)
}