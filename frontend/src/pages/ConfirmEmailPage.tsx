import { useEffect, useMemo, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { authApi } from "../api/authApi";
import Button from "../components/Button";

const ConfirmEmailPage = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState<"loading" | "success" | "error">("loading");
  const [message, setMessage] = useState("Potwierdzanie adresu e-mail...");
  const [subtitle, setSubtitle] = useState("To może potrwać kilka sekund.");
  const navigate = useNavigate();

  const payload = useMemo(() => {
    const userId = searchParams.get("userId") ?? "";
    const token = searchParams.get("token") ?? "";
    return { userId, token };
  }, [searchParams]);

  useEffect(() => {
    let timer: number | undefined;
    const run = async () => {
      if (!payload.userId || !payload.token) {
        setStatus("error");
        setMessage("Nieprawidłowy link potwierdzający.");
        setSubtitle("Sprawdź, czy adres URL został skopiowany w całości.");
        return;
      }

      try {
        await authApi.confirmEmail(payload);
        setStatus("success");
        setMessage("Adres e-mail został potwierdzony.");
        setSubtitle("Za chwilę nastąpi przekierowanie do sklepu.");
        timer = window.setTimeout(() => navigate("/products"), 3000);
      } catch {
        setStatus("error");
        setMessage("Nie udało się potwierdzić adresu e-mail.");
        setSubtitle("Możesz spróbować ponownie lub wrócić do sklepu.");
      }
    };

    run();
    return () => {
      if (timer) {
        window.clearTimeout(timer);
      }
    };
  }, [payload, navigate]);

  return (
    <div className="min-h-[70vh] flex items-center justify-center px-4">
      <div className="relative w-full max-w-xl overflow-hidden rounded-3xl bg-white p-10 shadow-soft">
        <div className="absolute inset-x-0 top-0 h-24 bg-gradient-to-r from-slate-900 via-slate-800 to-slate-700 opacity-10" />
        <div className="relative">
          <div className="inline-flex items-center rounded-full border border-slate-200 bg-slate-50 px-4 py-2 text-xs font-semibold uppercase tracking-wide text-slate-600">
            {status === "success" ? "Potwierdzenie zakończone" : status === "error" ? "Błąd potwierdzenia" : "Weryfikacja"}
          </div>
          <h1 className="mt-6 text-2xl font-semibold text-slate-900">{message}</h1>
          <p className="mt-2 text-sm text-slate-500">{subtitle}</p>
          <div className="mt-8 flex flex-col gap-3 sm:flex-row">
            <Button onClick={() => navigate("/products")} variant="secondary">
              Wróć do produktów
            </Button>
            {status !== "loading" && (
              <Button onClick={() => window.location.reload()}>
                Spróbuj ponownie
              </Button>
            )}
          </div>
          {status === "success" && (
            <div className="mt-6 text-xs text-slate-400">
              Jeśli przekierowanie nie nastąpi, kliknij przycisk powyżej.
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ConfirmEmailPage;
