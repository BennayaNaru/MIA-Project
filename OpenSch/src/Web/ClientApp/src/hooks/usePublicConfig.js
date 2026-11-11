import { useEffect, useState } from "react";

const FALLBACK = {
  turnstileEnabled: false,
  turnstileSiteKey: "",
};

/**
 * Reads runtime client configuration from the API. Keeping this out of the build means
 * the Turnstile site key can be changed without rebuilding the frontend.
 */
export function usePublicConfig() {
  const [config, setConfig] = useState({ ...FALLBACK, ready: false });

  useEffect(() => {
    let cancelled = false;
    const controller = new AbortController();

    fetch("/api/config/public", {
      signal: controller.signal,
      headers: { Accept: "application/json" },
    })
      .then((response) => (response.ok ? response.json() : FALLBACK))
      .then((data) => {
        if (!cancelled) {
          setConfig({ ...FALLBACK, ...data, ready: true });
        }
      })
      .catch(() => {
        if (!cancelled) {
          setConfig((current) => ({ ...current, ready: true }));
        }
      });

    return () => {
      cancelled = true;
      controller.abort();
    };
  }, []);

  return config;
}
