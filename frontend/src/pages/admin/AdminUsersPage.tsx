import { useEffect, useState } from "react";
import TextField from "../../components/TextField";
import Button from "../../components/Button";
import ErrorMessage from "../../components/ErrorMessage";
import { authApi } from "../../api/authApi";

type AdminUser = { id: string; email: string; firstname: string; lastname: string; age: number; roles: string[] };

const AdminUsersPage = () => {
  const [email, setEmail] = useState("");
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  const load = async () => {
    setError(null);
    try {
      const list = await authApi.adminGetUsers();
      setUsers(list);
    } catch {
      setError("Nie udalo sie zaladowac uzytkownikow");
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setSuccess(false);
    if (!email) {
      setError("Wpisz email");
      return;
    }
    setLoading(true);
    try {
      await authApi.promoteUser(email);
      setSuccess(true);
      setEmail("");
      await load();
    } catch {
      setError("Nie udalo sie nadac roli admin");
    } finally {
      setLoading(false);
    }
  };

  const handleDemote = async (userEmail: string) => {
    setError(null);
    try {
      await authApi.demoteUser(userEmail);
      await load();
    } catch {
      setError("Nie udalo sie odebrac roli admin");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Uzytkownicy</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 rounded-2xl bg-white p-4 shadow-card sm:grid-cols-[1fr_auto]">
        <TextField label="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
        <Button type="submit" disabled={loading}>{loading ? "Zapisywanie..." : "Nadaj Admin"}</Button>
      </form>
      {error ? <ErrorMessage message={error} /> : null}
      {success ? <div className="rounded-2xl bg-emerald-50 px-4 py-3 text-sm text-emerald-700">Rola nadana</div> : null}
      <div className="space-y-3">
        {users.map((user) => (
          <div key={user.id} className="rounded-2xl bg-white p-4 shadow-card">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm font-semibold text-slate-900">{user.email}</div>
                <div className="text-xs text-slate-500">
                  {user.firstname} {user.lastname} · {user.age}
                </div>
                <div className="text-xs text-slate-500">Role: {user.roles.join(", ") || "brak"}</div>
              </div>
              {user.roles.includes("Admin") ? (
                <Button variant="secondary" onClick={() => handleDemote(user.email)}>Odbierz Admin</Button>
              ) : null}
            </div>
          </div>
        ))}
        {users.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak uzytkownikow</div> : null}
      </div>
    </div>
  );
};

export default AdminUsersPage;
