import { THEME } from '@/constants/app'
import { ref } from 'vue'

export type SchemePreference = (typeof THEME.schemes)[number]

const scheme = ref<SchemePreference>(getInitialScheme())
const systemDarkQuery = window.matchMedia('(prefers-color-scheme: dark)')
systemDarkQuery.addEventListener('change', () => applyTheme())

function getInitialScheme(): SchemePreference {
  const stored = localStorage.getItem(THEME.schemeStorageKey)
  if (!stored) return 'system'
  const schemePreference = stored as SchemePreference
  return THEME.schemes.includes(schemePreference) ? schemePreference : 'system'
}

function applyTheme() {
  let isDark = false
  if (scheme.value === 'system') {
    isDark = systemDarkQuery.matches
  } else {
    isDark = scheme.value === 'dark'
  }

  document.documentElement.classList.toggle(THEME.darkSelector, isDark)
}

export function useThemeManager() {
  function initialize() {
    applyTheme()
  }

  function change(nextScheme?: SchemePreference) {
    if (nextScheme) scheme.value = nextScheme
    else {
      const nextSchemeIndex = (THEME.schemes.indexOf(scheme.value) + 1) % THEME.schemes.length
      scheme.value = THEME.schemes[nextSchemeIndex] as SchemePreference
    }
    localStorage.setItem(THEME.schemeStorageKey, scheme.value)
    applyTheme()
  }

  return {
    scheme,
    initialize,
    change,
  }
}
