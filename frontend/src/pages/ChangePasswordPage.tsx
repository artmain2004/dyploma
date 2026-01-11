import { useState } from "react";
import TextField from "../components/TextField";
import Button from "../components/Button";
import { authApi } from "../api/authApi";
import { useAppSelector } from "../app/hooks";

const ChangePasswordPage = () => {
  const user = useAppSelector((state) => state.auth.user);
  const [email, setEmail] = useState(user?.email ?? "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);
    try {
      await authApi.changePassword({ email, currentPassword, newPassword });
      setSuccess(true);
      setCurrentPassword("");
      setNewPassword("");
    } catch {
      setError("Nie udało się zmienić hasła");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-md rounded-2xl bg-white p-8 shadow-card">
      <h2 className="text-xl font-semibold text-slate-900">Zmień hasło</h2>
      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <TextField label="Obecne hasło" type="password" value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)} required />
        <TextField label="Nowe hasło" type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required />
        {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
        {success ? <div className="rounded-xl bg-emerald-50 px-4 py-3 text-sm text-emerald-700">Hasło zmienione</div> : null}
        <Button type="submit" disabled={loading}>
          {loading ? "Zapisywanie..." : "Zmień hasło"}
        </Button>
      </form>
    </div>
  );
};

export default ChangePasswordPage;
