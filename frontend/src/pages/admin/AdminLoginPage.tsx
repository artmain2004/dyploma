import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import TextField from "../../components/TextField";
import Button from "../../components/Button";
import { authApi } from "../../api/authApi";
import { useAppDispatch, useAppSelector } from "../../app/hooks";
import { setToken, setUser } from "../../features/auth/authSlice";
import { getRolesFromToken } from "../../utils/jwt";

const AdminLoginPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!token) {
      return;
    }
    const roles = getRolesFromToken(token);
    if (roles.includes("Admin")) {
      navigate("/admin");
    }
  }, [token, navigate]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const response = await authApi.login({ email, password });
      const roles = getRolesFromToken(response.accessToken);
      if (!roles.includes("Admin")) {
        setError("Brak uprawnien admina");
        setLoading(false);
        return;
      }
      localStorage.setItem("accessToken", response.accessToken);
            dispatch(setToken(response.accessToken));
      const user = await authApi.me();
      dispatch(setUser(user));
      navigate("/admin");
    } catch {
      setError("Nieprawidlowy login lub haslo");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-md rounded-2xl bg-white p-8 shadow-card">
      <h2 className="text-xl font-semibold text-slate-900">Logowanie admina</h2>
      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <TextField label="Email" type="email" autoComplete="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <label className="block text-sm font-medium text-slate-700">
          <span className="mb-2 block">Haslo</span>
          <div className="relative">
            <input
              type={showPassword ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              autoComplete="current-password"
              className="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 pr-12 text-slate-900 shadow-soft focus:border-slate-400 focus:outline-none"
            />
            <button
              type="button"
              onClick={() => setShowPassword((prev) => !prev)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-xs font-semibold text-slate-500 hover:text-slate-700"
            >
              {showPassword ? (
                <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                  <path
                    fill="currentColor"
                    d="M3.5 5.3L4.7 4.1l15.2 15.2-1.2 1.2-3.2-3.2c-1.1.5-2.3.7-3.5.7-4.4 0-8.1-2.9-10-6.9.7-1.6 1.7-3 3-4.1L3.5 5.3Zm5.3 5.3a3.2 3.2 0 0 0 4.5 4.5l-4.5-4.5Zm3.2-6.6c4.4 0 8.1 2.9 10 6.9a11.8 11.8 0 0 1-3.6 4.5l-2.1-2.1a5.5 5.5 0 0 0-7.6-7.6L6 4.6c1.8-1 3.8-1.6 6-1.6Z"
                  />
                </svg>
              ) : (
                <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                  <path
                    fill="currentColor"
                    d="M12 5c4.4 0 8.1 2.9 10 6.9-1.9 4-5.6 6.9-10 6.9s-8.1-2.9-10-6.9C3.9 7.9 7.6 5 12 5Zm0 3.2a3.8 3.8 0 1 0 0 7.6 3.8 3.8 0 0 0 0-7.6Z"
                  />
                </svg>
              )}
            </button>
          </div>
        </label>
        {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
        <Button type="submit" disabled={loading}>
          {loading ? "Logowanie..." : "Zaloguj"}
        </Button>
      </form>
      <p className="mt-6 text-sm text-slate-500">
        <Link to="/login" className="font-semibold text-slate-700 hover:text-slate-900">Logowanie uzytkownika</Link>
      </p>
    </div>
  );
};

export default AdminLoginPage;

