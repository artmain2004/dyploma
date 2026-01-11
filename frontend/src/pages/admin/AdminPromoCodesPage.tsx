import { useEffect, useState } from "react";
import { ordersApi } from "../../api/ordersApi";
import Button from "../../components/Button";
import TextField from "../../components/TextField";
import ErrorMessage from "../../components/ErrorMessage";

const AdminPromoCodesPage = () => {
  const [items, setItems] = useState<{ id: string; code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number; timesUsed: number }[]>([]);
  const [form, setForm] = useState({
    id: "",
    code: "",
    type: "Percent",
    value: "",
    isActive: true,
    expiresAtUtc: "",
    usageLimit: ""
  });
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    try {
      const data = await ordersApi.adminGetPromoCodes();
      setItems(data);
    } catch {
      setError("Nie udalo sie zaladowac promocji");
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    const payload = {
      code: form.code,
      type: form.type,
      value: Number(form.value),
      isActive: form.isActive,
      expiresAtUtc: form.expiresAtUtc || undefined,
      usageLimit: form.usageLimit ? Number(form.usageLimit) : undefined
    };
    try {
      if (form.id) {
        await ordersApi.adminUpdatePromoCode(form.id, payload);
      } else {
        await ordersApi.adminCreatePromoCode(payload);
      }
      setForm({ id: "", code: "", type: "Percent", value: "", isActive: true, expiresAtUtc: "", usageLimit: "" });
      await load();
    } catch {
      setError("Nie udalo sie zapisac promocji");
    }
  };

  const handleEdit = (item: { id: string; code: string; type: string; value: number; isActive: boolean; expiresAtUtc?: string; usageLimit?: number }) => {
    setForm({
      id: item.id,
      code: item.code,
      type: item.type,
      value: item.value.toString(),
      isActive: item.isActive,
      expiresAtUtc: item.expiresAtUtc ? item.expiresAtUtc.slice(0, 10) : "",
      usageLimit: item.usageLimit?.toString() ?? ""
    });
  };

  const handleDelete = async (id: string) => {
    try {
      await ordersApi.adminDeletePromoCode(id);
      await load();
    } catch {
      setError("Nie udalo sie usunac promocji");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Promocje</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 rounded-2xl bg-white p-4 shadow-card">
        <TextField label="Kod" value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} required />
        <label className="text-sm font-medium text-slate-700">
          Typ
          <select
            className="mt-2 w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
            value={form.type}
            onChange={(e) => setForm({ ...form, type: e.target.value })}
          >
            <option value="Percent">Percent</option>
            <option value="Fixed">Fixed</option>
          </select>
        </label>
        <TextField label="Wartosc" type="number" value={form.value} onChange={(e) => setForm({ ...form, value: e.target.value })} required />
        <TextField label="Limit uzyc" type="number" value={form.usageLimit} onChange={(e) => setForm({ ...form, usageLimit: e.target.value })} />
        <TextField label="Wygasa" type="date" value={form.expiresAtUtc} onChange={(e) => setForm({ ...form, expiresAtUtc: e.target.value })} />
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} />
          Aktywny
        </label>
        <Button type="submit">{form.id ? "Zapisz" : "Dodaj"}</Button>
      </form>
      {error ? <ErrorMessage message={error} /> : null}
      <div className="space-y-3">
        {items.map((item) => (
          <div key={item.id} className="rounded-2xl bg-white p-4 shadow-card">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm font-semibold text-slate-900">{item.code}</div>
                <div className="text-xs text-slate-500">{item.type} · {item.value}</div>
              </div>
              <div className="flex gap-2">
                <Button variant="secondary" onClick={() => handleEdit(item)}>Edytuj</Button>
                <Button variant="secondary" onClick={() => handleDelete(item.id)}>Usun</Button>
              </div>
            </div>
          </div>
        ))}
        {items.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak promocji</div> : null}
      </div>
    </div>
  );
};

export default AdminPromoCodesPage;
