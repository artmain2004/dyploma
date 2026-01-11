import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import TextField from "../components/TextField";
import Button from "../components/Button";
import { authApi } from "../api/authApi";
import { ordersApi } from "../api/ordersApi";
import { catalogApi } from "../api/catalogApi";
import type { OrderSummary, ProductListItem } from "../types";

const ProfilePage = () => {
  const [email, setEmail] = useState("");
  const [firstname, setFirstname] = useState("");
  const [lastname, setLastname] = useState("");
  const [age, setAge] = useState("");
  const [loadingProfile, setLoadingProfile] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [favorites, setFavorites] = useState<ProductListItem[]>([]);
  const [loadingOrders, setLoadingOrders] = useState(false);
  const [loadingFavorites, setLoadingFavorites] = useState(false);

  useEffect(() => {
    setLoadingProfile(true);
    authApi
      .getProfile()
      .then((profile) => {
        setEmail(profile.email ?? "");
        setFirstname(profile.firstname ?? "");
        setLastname(profile.lastname ?? "");
        setAge(profile.age.toString());
      })
      .catch(() => setError("Nie udalo sie zaladowac profilu"))
      .finally(() => setLoadingProfile(false));

    setLoadingOrders(true);
    ordersApi
      .getMyOrders()
      .then(setOrders)
      .catch(() => null)
      .finally(() => setLoadingOrders(false));

    setLoadingFavorites(true);
    catalogApi
      .getFavorites()
      .then(setFavorites)
      .catch(() => null)
      .finally(() => setLoadingFavorites(false));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setSuccess(false);
    const ageValue = Number(age);
    if (Number.isNaN(ageValue)) {
      setError("Podaj poprawny wiek");
      setSaving(false);
      return;
    }
    try {
      const updated = await authApi.updateProfile({ firstname, lastname, age: ageValue });
      setEmail(updated.email ?? "");
      setFirstname(updated.firstname ?? "");
      setLastname(updated.lastname ?? "");
      setAge(updated.age.toString());
      setSuccess(true);
    } catch {
      setError("Nie udalo sie zapisac profilu");
    } finally {
      setSaving(false);
    }
  };

  const handleRemoveFavorite = async (id: string) => {
    try {
      await catalogApi.removeFavorite(id);
      setFavorites((prev) => prev.filter((p) => p.id !== id));
    } catch {
      setError("Nie udalo sie usunac z ulubionych");
    }
  };

  if (loadingProfile) {
    return <div className="rounded-2xl bg-white p-6 shadow-card">Ladowanie...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
        <div className="rounded-2xl bg-white p-6 shadow-card">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-semibold text-slate-900">Profil</h2>
            {success ? <span className="rounded-full bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700">Zapisano</span> : null}
          </div>
          <form onSubmit={handleSubmit} className="mt-6 space-y-4">
            <TextField label="Email" value={email} readOnly />
            <div className="grid gap-4 sm:grid-cols-2">
              <TextField label="Imie" value={firstname} onChange={(e) => setFirstname(e.target.value)} required />
              <TextField label="Nazwisko" value={lastname} onChange={(e) => setLastname(e.target.value)} required />
            </div>
            <TextField label="Wiek" type="number" value={age} onChange={(e) => setAge(e.target.value)} required />
            {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
            <Button type="submit" disabled={saving}>
              {saving ? "Zapisywanie..." : "Zapisz"}
            </Button>
          </form>
        </div>
        <div className="space-y-4 rounded-2xl bg-white p-6 shadow-card">
          <h3 className="text-lg font-semibold text-slate-900">Szybkie akcje</h3>
          <div className="grid gap-3">
            <Link to="/change-password" className="rounded-xl border border-slate-200 px-4 py-3 text-sm font-semibold text-slate-700 shadow-soft hover:bg-slate-50">
              Zmien haslo
            </Link>
            <Link to="/profile/addresses" className="rounded-xl border border-slate-200 px-4 py-3 text-sm font-semibold text-slate-700 shadow-soft hover:bg-slate-50">
              Adresy dostawy
            </Link>
          </div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-2xl bg-white p-6 shadow-card">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-slate-900">Moje zamowienia</h3>
            <Link to="/my-orders" className="text-xs font-semibold text-slate-600 hover:text-slate-900">Pelna lista</Link>
          </div>
          {loadingOrders ? (
            <div className="mt-4 text-sm text-slate-500">Ladowanie...</div>
          ) : orders.length === 0 ? (
            <div className="mt-4 rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">Brak zamowien.</div>
          ) : (
            <div className="mt-4 overflow-hidden rounded-xl border border-slate-200">
              <div className="grid grid-cols-4 gap-2 bg-slate-50 px-4 py-2 text-xs font-semibold text-slate-500">
                <span>Numer</span>
                <span>Status</span>
                <span>Suma</span>
                <span>Data</span>
              </div>
              <div className="divide-y divide-slate-200">
                {orders.slice(0, 5).map((order) => (
                  <Link
                    key={order.id}
                    to={`/orders/${order.id}`}
                    className="grid grid-cols-4 gap-2 px-4 py-3 text-sm text-slate-700 hover:bg-slate-50"
                  >
                    <span className="font-semibold text-slate-900">{order.orderNumber}</span>
                    <span>{order.status}</span>
                    <span>${order.totalPrice.toFixed(2)}</span>
                    <span>{new Date(order.createdAtUtc).toLocaleDateString()}</span>
                  </Link>
                ))}
              </div>
            </div>
          )}
        </div>

        <div className="rounded-2xl bg-white p-6 shadow-card">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-slate-900">Ulubione produkty</h3>
            <Link to="/favorites" className="text-xs font-semibold text-slate-600 hover:text-slate-900">Pelna lista</Link>
          </div>
          {loadingFavorites ? (
            <div className="mt-4 text-sm text-slate-500">Ladowanie...</div>
          ) : favorites.length === 0 ? (
            <div className="mt-4 rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">Brak ulubionych.</div>
          ) : (
            <div className="mt-4 overflow-hidden rounded-xl border border-slate-200">
              <div className="grid grid-cols-[1.4fr_0.6fr_0.6fr] gap-2 bg-slate-50 px-4 py-2 text-xs font-semibold text-slate-500">
                <span>Produkt</span>
                <span>Cena</span>
                <span>Akcje</span>
              </div>
              <div className="divide-y divide-slate-200">
                {favorites.slice(0, 5).map((product) => (
                  <div key={product.id} className="grid grid-cols-[1.4fr_0.6fr_0.6fr] gap-2 px-4 py-3 text-sm text-slate-700">
                    <Link to={`/products/${product.id}`} className="font-semibold text-slate-900 hover:text-slate-700">
                      {product.name}
                    </Link>
                    <span>${product.price.toFixed(2)}</span>
                    <div className="flex gap-2">
                      <Link to={`/products/${product.id}`} className="text-xs font-semibold text-slate-600 hover:text-slate-900">
                        Podglad
                      </Link>
                      <button
                        onClick={() => handleRemoveFavorite(product.id)}
                        className="text-xs font-semibold text-rose-600 hover:text-rose-700"
                      >
                        Usun
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
