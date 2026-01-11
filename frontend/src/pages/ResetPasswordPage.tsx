import { useMemo, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import TextField from "../components/TextField";
import Button from "../components/Button";
import { authApi } from "../api/authApi";

const useQuery = () => {
  const location = useLocation();
  return useMemo(() => new URLSearchParams(location.search), [location.search]);
};

const ResetPasswordPage = () => {
  const navigate = useNavigate();
  const query = useQuery();
  const [email, setEmail] = useState(query.get("email") ?? "");
  const [code] = useState(query.get("code") ?? "");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await authApi.confirmResetPassword({ email, resetCode: code, newPassword: password });
      navigate("/login");
    } catch {
      setError("Nie udało się zresetować hasła");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-md rounded-2xl bg-white p-8 shadow-card">
      <h2 className="text-xl font-semibold text-slate-900">Ustaw nowe hasło</h2>
      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <TextField label="Nowe hasło" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
        <Button type="submit" disabled={loading}>
          {loading ? "Zapisywanie..." : "Zapisz hasło"}
        </Button>
      </form>
      <p className="mt-6 text-sm text-slate-500">
        <Link to="/login" className="font-semibold text-slate-700 hover:text-slate-900">Wróć do logowania</Link>
      </p>
    </div>
  );
};

export default ResetPasswordPage;
