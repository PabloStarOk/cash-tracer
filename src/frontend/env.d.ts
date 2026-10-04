/// <reference types="vite/client" />

interface ViteTypeOptions {
  strictImportMetaEnv: unknown
}

interface ImportMetaEnv {
  VITE_API_URL: string
  VITE_API_REQUEST_TIMEOUT_MS: number
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
