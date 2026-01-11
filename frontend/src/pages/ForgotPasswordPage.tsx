import { useState } from "react";
import { Link } from "react-router-dom";
import TextField from "../components/TextField";
import Button from "../components/Button";
import { authApi } from "../api/authApi";

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);
    try {
      await authApi.resetPassword(email);
      setSuccess(true);
    } catch {
      setError("Nie udało się wysłać emaila resetującego");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-md rounded-2xl bg-white p-8 shadow-card">
      <h2 className="text-xl font-semibold text-slate-900">Reset hasła</h2>
      <p className="mt-2 text-sm text-slate-500">Podaj email, a wyślemy link do resetu.</p>
      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
        {success ? <div className="rounded-xl bg-emerald-50 px-4 py-3 text-sm text-emerald-700">Email wysłany</div> : null}
        <Button type="submit" disabled={loading}>
          {loading ? "Wysyłanie..." : "Wyślij link"}
        </Button>
      </form>
      <p className="mt-6 text-sm text-slate-500">
        <Link to="/login" className="font-semibold text-slate-700 hover:text-slate-900">Wróć do logowania</Link>
      </p>
    </div>
  );
};

export default ForgotPasswordPage;
