import { useEffect, useState } from "react";
import TextField from "../components/TextField";
import Button from "../components/Button";
import ErrorMessage from "../components/ErrorMessage";
import { authApi } from "../api/authApi";

type Address = {
  id: string;
  label?: string;
  line1: string;
  line2?: string;
  city: string;
  region?: string;
  postalCode: string;
  country: string;
  phone?: string;
  isDefault: boolean;
  createdAtUtc: string;
};

const AddressesPage = () => {
  const [items, setItems] = useState<Address[]>([]);
  const [form, setForm] = useState({
    id: "",
    label: "",
    line1: "",
    line2: "",
    city: "",
    region: "",
    postalCode: "",
    country: "",
    phone: "",
    isDefault: false
  });
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setError(null);
    try {
      const list = await authApi.getAddresses();
      setItems(list);
    } catch {
      setError("Nie udalo sie zaladowac adresow");
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    const payload = {
      label: form.label || undefined,
      line1: form.line1,
      line2: form.line2 || undefined,
      city: form.city,
      region: form.region || undefined,
      postalCode: form.postalCode,
      country: form.country,
      phone: form.phone || undefined,
      isDefault: form.isDefault
    };
    try {
      if (form.id) {
        await authApi.updateAddress(form.id, payload);
      } else {
        await authApi.createAddress(payload);
      }
      setForm({ id: "", label: "", line1: "", line2: "", city: "", region: "", postalCode: "", country: "", phone: "", isDefault: false });
      await load();
    } catch {
      setError("Nie udalo sie zapisac adresu");
    }
  };

  const handleEdit = (item: Address) => {
    setForm({
      id: item.id,
      label: item.label ?? "",
      line1: item.line1,
      line2: item.line2 ?? "",
      city: item.city,
      region: item.region ?? "",
      postalCode: item.postalCode,
      country: item.country,
      phone: item.phone ?? "",
      isDefault: item.isDefault
    });
  };

  const handleDelete = async (id: string) => {
    setError(null);
    try {
      await authApi.deleteAddress(id);
      await load();
    } catch {
      setError("Nie udalo sie usunac adresu");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Adresy dostawy</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 rounded-2xl bg-white p-4 shadow-card">
        <TextField label="Etykieta" value={form.label} onChange={(e) => setForm({ ...form, label: e.target.value })} />
        <TextField label="Ulica i numer" value={form.line1} onChange={(e) => setForm({ ...form, line1: e.target.value })} required />
        <TextField label="Dodatkowy adres" value={form.line2} onChange={(e) => setForm({ ...form, line2: e.target.value })} />
        <TextField label="Miasto" value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} required />
        <TextField label="Region" value={form.region} onChange={(e) => setForm({ ...form, region: e.target.value })} />
        <TextField label="Kod pocztowy" value={form.postalCode} onChange={(e) => setForm({ ...form, postalCode: e.target.value })} required />
        <TextField label="Kraj" value={form.country} onChange={(e) => setForm({ ...form, country: e.target.value })} required />
        <TextField label="Telefon" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input type="checkbox" checked={form.isDefault} onChange={(e) => setForm({ ...form, isDefault: e.target.checked })} />
          Domyslny adres
        </label>
        <Button type="submit">{form.id ? "Zapisz" : "Dodaj"}</Button>
      </form>
      {error ? <ErrorMessage message={error} /> : null}
      <div className="space-y-3">
        {items.map((item) => (
          <div key={item.id} className="rounded-2xl bg-white p-4 shadow-card">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm font-semibold text-slate-900">{item.label || "Adres"}</div>
                <div className="text-xs text-slate-500">{item.line1}{item.line2 ? ", " + item.line2 : ""}</div>
                <div className="text-xs text-slate-500">{item.postalCode} {item.city}, {item.country}</div>
                <div className="text-xs text-slate-500">{item.isDefault ? "Domyslny" : ""}</div>
              </div>
              <div className="flex gap-2">
                <Button variant="secondary" onClick={() => handleEdit(item)}>Edytuj</Button>
                <Button variant="secondary" onClick={() => handleDelete(item.id)}>Usun</Button>
              </div>
            </div>
          </div>
        ))}
        {items.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak adresow</div> : null}
      </div>
    </div>
  );
};

export default AddressesPage;
