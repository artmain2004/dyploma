import { useEffect, useState } from "react";
import { catalogApi } from "../../api/catalogApi";
import Button from "../../components/Button";
import TextField from "../../components/TextField";
import ErrorMessage from "../../components/ErrorMessage";

const AdminCategoriesPage = () => {
  const [items, setItems] = useState<{ id: string; name: string }[]>([]);
  const [name, setName] = useState("");
  const [editId, setEditId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await catalogApi.adminGetCategories();
      setItems(data);
    } catch {
      setError("Nie udalo sie zaladowac kategorii");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!name) {
      return;
    }
    try {
      if (editId) {
        await catalogApi.adminUpdateCategory(editId, { name });
      } else {
        await catalogApi.adminCreateCategory({ name });
      }
      setName("");
      setEditId(null);
      await load();
    } catch {
      setError("Nie udalo sie zapisac kategorii");
    }
  };

  const handleEdit = (id: string, currentName: string) => {
    setEditId(id);
    setName(currentName);
  };

  const handleDelete = async (id: string) => {
    try {
      await catalogApi.adminDeleteCategory(id);
      await load();
    } catch {
      setError("Nie udalo sie usunac kategorii");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Kategorie</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 rounded-2xl bg-white p-4 shadow-card sm:grid-cols-[1fr_auto]">
        <TextField label="Nazwa" value={name} onChange={(e) => setName(e.target.value)} required />
        <Button type="submit">{editId ? "Zapisz" : "Dodaj"}</Button>
      </form>
      {error ? <ErrorMessage message={error} /> : null}
      {loading ? <div className="rounded-2xl bg-white p-4 shadow-card">Ladowanie...</div> : null}
      <div className="space-y-3">
        {items.map((item) => (
          <div key={item.id} className="flex items-center justify-between rounded-2xl bg-white p-4 shadow-card">
            <span className="text-sm font-semibold text-slate-900">{item.name}</span>
            <div className="flex gap-2">
              <Button variant="secondary" onClick={() => handleEdit(item.id, item.name)}>Edytuj</Button>
              <Button variant="secondary" onClick={() => handleDelete(item.id)}>Usun</Button>
            </div>
          </div>
        ))}
        {items.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak kategorii</div> : null}
      </div>
    </div>
  );
};

export default AdminCategoriesPage;
