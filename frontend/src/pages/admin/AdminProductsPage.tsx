import { useEffect, useState } from "react";
import { catalogApi } from "../../api/catalogApi";
import Button from "../../components/Button";
import TextField from "../../components/TextField";
import ErrorMessage from "../../components/ErrorMessage";

const AdminProductsPage = () => {
  const [products, setProducts] = useState<{ id: string; name: string; description?: string; price: number; imageUrl?: string; isFeatured: boolean; categoryId?: string }[]>([]);
  const [categories, setCategories] = useState<{ id: string; name: string }[]>([]);
  const [form, setForm] = useState({
    id: "",
    name: "",
    description: "",
    price: "",
    imageFile: null as File | null,
    isFeatured: false,
    categoryId: ""
  });
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    try {
      const [cats, prods] = await Promise.all([
        catalogApi.adminGetCategories(),
        catalogApi.adminGetProducts({})
      ]);
      setCategories(cats);
      setProducts(prods);
    } catch {
      setError("Nie udalo sie zaladowac danych");
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    const payload = {
      name: form.name,
      description: form.description || undefined,
      price: Number(form.price),
      imageFile: form.imageFile,
      isFeatured: form.isFeatured,
      categoryId: form.categoryId || undefined
    };
    try {
      if (form.id) {
        await catalogApi.adminUpdateProduct(form.id, payload);
      } else {
        await catalogApi.adminCreateProduct(payload);
      }
      setForm({ id: "", name: "", description: "", price: "", imageFile: null, isFeatured: false, categoryId: "" });
      await load();
    } catch {
      setError("Nie udalo sie zapisac produktu");
    }
  };

  const handleEdit = (product: { id: string; name: string; description?: string; price: number; imageUrl?: string; isFeatured: boolean; categoryId?: string }) => {
    setForm({
      id: product.id,
      name: product.name,
      description: product.description ?? "",
      price: product.price.toString(),
      imageFile: null,
      isFeatured: product.isFeatured,
      categoryId: product.categoryId ?? ""
    });
  };

  const handleDelete = async (id: string) => {
    try {
      await catalogApi.adminDeleteProduct(id);
      await load();
    } catch {
      setError("Nie udalo sie usunac produktu");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Produkty</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 rounded-2xl bg-white p-4 shadow-card">
        <TextField label="Nazwa" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        <TextField label="Opis" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        <TextField label="Cena" type="number" value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} required />
        <label htmlFor="admin-product-image" className="text-sm font-medium text-slate-700">
          Zdjecie produktu
          <div className="mt-2 flex items-center justify-between gap-3 rounded-xl border border-dashed border-slate-300 bg-slate-50 px-4 py-3 text-sm text-slate-600">
            <span className="truncate">{form.imageFile?.name ?? "Nie wybrano pliku"}</span>
            <span className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white">Wybierz</span>
          </div>
          <input
            type="file"
            accept="image/*"
            className="sr-only"
            id="admin-product-image"
            onChange={(e) => setForm({ ...form, imageFile: e.target.files?.[0] ?? null })}
          />
        </label>
        <label className="text-sm font-medium text-slate-700">
          Kategoria
          <select
            className="mt-2 w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
            value={form.categoryId}
            onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
          >
            <option value="">Brak</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </label>
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input type="checkbox" checked={form.isFeatured} onChange={(e) => setForm({ ...form, isFeatured: e.target.checked })} />
          Wyrozniony
        </label>
        <Button type="submit">{form.id ? "Zapisz" : "Dodaj"}</Button>
      </form>
      {error ? <ErrorMessage message={error} /> : null}
      <div className="space-y-3">
        {products.map((product) => (
          <div key={product.id} className="rounded-2xl bg-white p-4 shadow-card">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm font-semibold text-slate-900">{product.name}</div>
                <div className="text-xs text-slate-500">${product.price.toFixed(2)}</div>
              </div>
              <div className="flex gap-2">
                <Button variant="secondary" onClick={() => handleEdit(product)}>Edytuj</Button>
                <Button variant="secondary" onClick={() => handleDelete(product.id)}>Usun</Button>
              </div>
            </div>
          </div>
        ))}
        {products.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak produktow</div> : null}
      </div>
    </div>
  );
};

export default AdminProductsPage;
