import { useEffect, useRef, useState } from "react";

const SCRIPT_ID = "cf-turnstile-script";
const SCRIPT_SRC = "https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit";

let scriptPromise;

function loadTurnstile() {
  if (window.turnstile) {
    return Promise.resolve(window.turnstile);
  }

  if (scriptPromise) {
    return scriptPromise;
  }

  scriptPromise = new Promise((resolve, reject) => {
    const existing = document.getElementById(SCRIPT_ID);
    if (existing) {
      existing.addEventListener("load", () => resolve(window.turnstile));
      existing.addEventListener("error", () => reject(new Error("Turnstile script failed to load.")));
      return;
    }

    const script = document.createElement("script");
    script.id = SCRIPT_ID;
    script.src = SCRIPT_SRC;
    script.async = true;
    script.defer = true;
    script.onload = () => resolve(window.turnstile);
    script.onerror = () => reject(new Error("Turnstile script failed to load."));
    document.head.appendChild(script);
  });

  return scriptPromise;
}

/**
 * Renders the Cloudflare Turnstile widget and reports the token to the parent.
 * The token is single-use; remount the component (for example with a new `key`)
 * after a failed submission.
 */
export default function TurnstileWidget({ siteKey, onToken, onExpire, onError, action = "register" }) {
  const containerRef = useRef(null);
  const widgetIdRef = useRef(null);
  const callbacksRef = useRef({ onToken, onExpire, onError });
  const [failed, setFailed] = useState(false);

  useEffect(() => {
    callbacksRef.current = { onToken, onExpire, onError };
  }, [onToken, onExpire, onError]);

  useEffect(() => {
    let cancelled = false;

    loadTurnstile()
      .then((turnstile) => {
        if (cancelled || !containerRef.current || !turnstile) {
          return;
        }

        widgetIdRef.current = turnstile.render(containerRef.current, {
          sitekey: siteKey,
          action,
          theme: "light",
          callback: (token) => callbacksRef.current.onToken?.(token),
          "expired-callback": () => callbacksRef.current.onExpire?.(),
          "error-callback": () => {
            setFailed(true);
            callbacksRef.current.onError?.();
          },
        });
      })
      .catch(() => {
        if (!cancelled) {
          setFailed(true);
          callbacksRef.current.onError?.();
        }
      });

    return () => {
      cancelled = true;
      const turnstile = window.turnstile;
      if (turnstile && widgetIdRef.current != null) {
        try {
          turnstile.remove(widgetIdRef.current);
        } catch {
          // The widget may already have been removed by the page lifecycle.
        }
      }
      widgetIdRef.current = null;
    };
  }, [siteKey, action]);

  return (
    <div className="space-y-1">
      <div ref={containerRef} className="min-h-[65px]" />
      {failed && (
        <p className="text-[11px] text-error">
          Verifikasi keamanan gagal dimuat. Muat ulang halaman atau nonaktifkan pemblokir.
        </p>
      )}
    </div>
  );
}
