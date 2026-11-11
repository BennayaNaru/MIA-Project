import { createContext, useContext, useEffect, useMemo, useState } from 'react'

export const MAX_COMPARE = 3

const CompareContext = createContext(null)

const mapSchool = (school) => ({
  id: school.id,
  name: school.name,
  kota: school.city,
  kec: school.district,
  level: school.level,
  lit: school.literacyScore,
  num: school.numeracyScore,
  composite: school.compositeScore,
  rank: school.rank,
})

const average = (values) =>
  values.length
    ? Number((values.reduce((total, value) => total + value, 0) / values.length).toFixed(1))
    : 0

export function CompareProvider({ children }) {
  const [schools, setSchools] = useState([])
  const [schoolsLoading, setSchoolsLoading] = useState(true)
  const [schoolsError, setSchoolsError] = useState(null)
  const [selectedIds, setSelectedIds] = useState([])
  const [detailId, setDetailId] = useState(null)
  const [message, setMessage] = useState('')

  // The catalog is owned by the API. Fetch it once and derive every view-model
  // field, district filter, and baseline from the same response.
  useEffect(() => {
    const controller = new AbortController()
    setSchoolsLoading(true)
    setSchoolsError(null)

    fetch('/api/schools', {
      headers: { Accept: 'application/json' },
      credentials: 'include',
      signal: controller.signal,
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error('Katalog sekolah tidak dapat dimuat dari server.')
        }
        return response.json()
      })
      .then((data) => setSchools(data.map(mapSchool)))
      .catch((error) => {
        if (error.name !== 'AbortError') setSchoolsError(error.message)
      })
      .finally(() => {
        if (!controller.signal.aborted) setSchoolsLoading(false)
      })

    return () => controller.abort()
  }, [])

  const kecamatanMap = useMemo(() => {
    const map = {}

    for (const school of schools) {
      if (!school.kota || !school.kec) continue
      map[school.kota] ??= []
      if (!map[school.kota].includes(school.kec)) map[school.kota].push(school.kec)
    }

    for (const districts of Object.values(map)) {
      districts.sort((a, b) => a.localeCompare(b))
    }

    return map
  }, [schools])

  const baseline = useMemo(
    () => ({
      lit: average(schools.map((school) => school.lit)),
      num: average(schools.map((school) => school.num)),
    }),
    [schools],
  )

  const benchmark = useMemo(
    () => average(schools.map((school) => school.composite)),
    [schools],
  )

  const toggle = (id) => {
    if (selectedIds.includes(id)) {
      setSelectedIds(selectedIds.filter((x) => x !== id))
      setMessage('')
    } else if (selectedIds.length >= MAX_COMPARE) {
      setMessage('Maksimal 3 sekolah. Hapus satu pilihan sebelum menambahkan sekolah lain.')
    } else {
      setSelectedIds([...selectedIds, id])
      setMessage('')
    }
  }

  const remove = (id) => {
    setSelectedIds(selectedIds.filter((x) => x !== id))
    setMessage('')
  }
  const clear = () => {
    setSelectedIds([])
    setMessage('')
  }
  const loadDefault = () => setSelectedIds(['DKI-01', 'DKI-02', 'DKI-03'])
  const openDetail = (id) => setDetailId(id)
  const closeDetail = () => setDetailId(null)

  const selectedSchools = selectedIds
    .map((id) => schools.find((school) => school.id === id))
    .filter(Boolean)

  return (
    <CompareContext.Provider
      value={{
        schools, schoolsLoading, schoolsError, kecamatanMap, baseline, benchmark,
        selectedIds, selectedSchools, toggle, remove, clear, loadDefault,
        detailId, openDetail, closeDetail, message,
      }}
    >
      {children}
    </CompareContext.Provider>
  )
}

export function useCompare() {
  return useContext(CompareContext)
}
