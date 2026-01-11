interface ImportMetaEnv {
  readonly VITE_AUTH_URL: string;
  readonly VITE_CATALOG_URL: string;
  readonly VITE_ORDERS_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
