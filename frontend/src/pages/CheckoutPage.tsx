import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Button from "../components/Button";
import TextField from "../components/TextField";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { ordersApi } from "../api/ordersApi";
import { authApi } from "../api/authApi";
import { cartApi } from "../api/cartApi";
import { clear } from "../features/cart/cartSlice";

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
};

const CheckoutPage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const items = useAppSelector((state) => state.cart.items);
  const user = useAppSelector((state) => state.auth.user);
  const isLoggedIn = Boolean(user?.userId || user?.email);

  const [email, setEmail] = useState(user?.email ?? "");
  const [name, setName] = useState(user?.name ?? "");
  const [phone, setPhone] = useState("");
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [selectedAddressId, setSelectedAddressId] = useState("");
  const [promoCode, setPromoCode] = useState("");
  const [promoResult, setPromoResult] = useState<{ discountAmount: number; totalAfterDiscount: number } | null>(null);
  const [promoError, setPromoError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showAddressModal, setShowAddressModal] = useState(false);
  const [addressError, setAddressError] = useState<string | null>(null);
  const [savingAddress, setSavingAddress] = useState(false);
  const [newAddress, setNewAddress] = useState<{
    label: string;
    line1: string;
    line2: string;
    city: string;
    region: string;
    postalCode: string;
    country: string;
    phone: string;
    isDefault: boolean;
  }>({
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
  const [guestAddress, setGuestAddress] = useState({
    line1: "",
    line2: "",
    city: "",
    region: "",
    postalCode: "",
    country: ""
  });

  const total = useMemo(() => items.reduce((sum, i) => sum + i.unitPrice * i.quantity, 0), [items]);
  const totalAfterDiscount = promoResult ? promoResult.totalAfterDiscount : total;

  const formatAddress = (a: Address) => {
    const parts = [a.line1, a.line2, a.city, a.region, a.postalCode, a.country]
      .filter((p) => p && p.trim().length > 0)
      .join(", ");
    const phonePart = a.phone ? ` (${a.phone})` : "";
    return `${parts}${phonePart}`;
  };

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }
    authApi
      .getAddresses()
      .then((list) => {
        setAddresses(list);
        const def = list.find((a) => a.isDefault);
        if (def) {
          setSelectedAddressId(def.id);
        }
      })
      .catch(() => null);
  }, [isLoggedIn]);

  const buildGuestAddress = () =>
    [guestAddress.line1, guestAddress.line2, guestAddress.city, guestAddress.region, guestAddress.postalCode, guestAddress.country]
      .filter((p) => p && p.trim().length > 0)
      .join(", ");

  const handleApplyPromo = async () => {
    if (!promoCode) {
      setPromoError("Wpisz kod promocyjny");
      return;
    }
    setPromoError(null);
    try {
      const result = await ordersApi.validatePromoCode({ code: promoCode, total });
      setPromoResult({ discountAmount: result.discountAmount, totalAfterDiscount: result.totalAfterDiscount });
    } catch {
      setPromoResult(null);
      setPromoError("Kod jest nieprawidlowy");
    }
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!email) {
      setError("Email jest wymagany");
      return;
    }
    if (items.length === 0) {
      setError("Koszyk jest pusty");
      return;
    }
    setAddressError(null);
    const selectedAddress = isLoggedIn ? addresses.find((a) => a.id === selectedAddressId) : null;
    const shippingAddressValue = isLoggedIn ? (selectedAddress ? formatAddress(selectedAddress) : "") : buildGuestAddress();
    if (!shippingAddressValue) {
      setAddressError("Adres dostawy jest wymagany");
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      const response = await ordersApi.createOrder({
        customerEmail: email,
        customerName: name || undefined,
        customerPhone: phone || undefined,
        shippingAddress: shippingAddressValue || undefined,
        promoCode: promoCode || undefined,
        items: items.map((i) => ({
          productId: i.productId,
          productName: i.productName,
          unitPrice: i.unitPrice,
          quantity: i.quantity
        }))
      });
      if (isLoggedIn) {
        try {
          await cartApi.clear();
        } catch {
        }
      }
      dispatch(clear());
      navigate("/orders/success", { state: { orderNumber: response.orderNumber } });
    } catch {
      setError("Nie udalo sie zlozyc zamowienia");
    } finally {
      setSubmitting(false);
    }
  };

  const handleSaveAddress = async () => {
    if (!newAddress.line1 || !newAddress.city || !newAddress.postalCode || !newAddress.country) {
      setAddressError("Uzupelnij wymagane pola adresu");
      return;
    }
    setSavingAddress(true);
    setAddressError(null);
    try {
      const created = await authApi.createAddress({
        label: newAddress.label || undefined,
        line1: newAddress.line1,
        line2: newAddress.line2 || undefined,
        city: newAddress.city,
        region: newAddress.region || undefined,
        postalCode: newAddress.postalCode,
        country: newAddress.country,
        phone: newAddress.phone || undefined,
        isDefault: newAddress.isDefault
      });
      setAddresses((prev) => [created, ...prev]);
      setSelectedAddressId(created.id);
      setShowAddressModal(false);
      setNewAddress({
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
    } catch {
      setAddressError("Nie udalo sie zapisac adresu");
    } finally {
      setSavingAddress(false);
    }
  };

  if (items.length === 0) {
    return (
      <div className="rounded-2xl bg-white p-8 text-center shadow-card">
        <h2 className="text-xl font-semibold text-slate-900">Koszyk jest pusty</h2>
        <p className="mt-2 text-sm text-slate-500">Dodaj produkty, aby zlozyc zamowienie.</p>
        <div className="mt-6">
          <Link to="/products" className="text-sm font-semibold text-slate-700 hover:text-slate-900">
            Przejdz do produktow
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
      <form onSubmit={handleSubmit} className="space-y-4 rounded-2xl bg-white p-6 shadow-card">
        <h2 className="text-xl font-semibold text-slate-900">Zamowienie</h2>
        <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <TextField label="Imie" value={name} onChange={(e) => setName(e.target.value)} />
        <TextField label="Telefon" value={phone} onChange={(e) => setPhone(e.target.value)} />

        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <span className="text-sm font-semibold text-slate-700">Adres dostawy</span>
            {isLoggedIn ? (
              <button
                type="button"
                onClick={() => setShowAddressModal(true)}
                className="rounded-xl border border-slate-200 px-3 py-1 text-xs font-semibold text-slate-700 shadow-soft hover:bg-slate-50"
              >
                + Dodaj adres
              </button>
            ) : null}
          </div>
          {isLoggedIn ? (
            <div className="flex flex-col gap-3">
              {addresses.length === 0 ? (
                <div className="rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">
                  Nie masz jeszcze zapisanych adresow. Dodaj nowy adres.
                </div>
              ) : (
                addresses.map((a, index) => (
                  <div
                    key={a.id}
                    className={`flex items-center justify-between rounded-2xl border px-4 py-3 text-left text-sm shadow-soft transition ${
                      selectedAddressId === a.id ? "border-slate-900 bg-slate-50" : "border-slate-200 bg-white"
                    }`}
                  >
                    <div>
                      <div className="text-xs text-slate-500">Adres #{index + 1}</div>
                      <div className="mt-1 font-semibold text-slate-900">{a.label || "Adres"}</div>
                      <div className="mt-1 text-xs text-slate-500">{formatAddress(a)}</div>
                      {a.isDefault ? <div className="mt-2 text-[11px] text-emerald-700">Domyslny</div> : null}
                      {selectedAddressId === a.id ? <div className="mt-2 text-[11px] text-slate-700">Wybrany</div> : null}
                    </div>
                    <Button
                      type="button"
                      variant="secondary"
                      onClick={() => {
                        setSelectedAddressId(a.id);
                      }}
                    >
                      Wybierz
                    </Button>
                  </div>
                ))
              )}
            </div>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2">
              <TextField label="Ulica i numer" value={guestAddress.line1} onChange={(e) => setGuestAddress((prev) => ({ ...prev, line1: e.target.value }))} required />
              <TextField label="Mieszkanie (opcjonalnie)" value={guestAddress.line2} onChange={(e) => setGuestAddress((prev) => ({ ...prev, line2: e.target.value }))} />
              <TextField label="Miasto" value={guestAddress.city} onChange={(e) => setGuestAddress((prev) => ({ ...prev, city: e.target.value }))} required />
              <TextField label="Region (opcjonalnie)" value={guestAddress.region} onChange={(e) => setGuestAddress((prev) => ({ ...prev, region: e.target.value }))} />
              <TextField label="Kod pocztowy" value={guestAddress.postalCode} onChange={(e) => setGuestAddress((prev) => ({ ...prev, postalCode: e.target.value }))} required />
              <TextField label="Kraj" value={guestAddress.country} onChange={(e) => setGuestAddress((prev) => ({ ...prev, country: e.target.value }))} required />
            </div>
          )}
        </div>

        <div className="grid gap-3 rounded-xl bg-slate-50 p-4">
          <TextField label="Kod promocyjny" value={promoCode} onChange={(e) => setPromoCode(e.target.value)} />
          <Button type="button" variant="secondary" onClick={handleApplyPromo}>
            Zastosuj kod
          </Button>
          {promoError ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{promoError}</div> : null}
          {promoResult ? (
            <div className="text-sm text-emerald-700">
              Znizka: ${promoResult.discountAmount.toFixed(2)}
            </div>
          ) : null}
        </div>
        {addressError ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{addressError}</div> : null}
        {error ? <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{error}</div> : null}
        <Button type="submit" disabled={submitting}>
          {submitting ? "Wysylanie..." : "Zloz zamowienie"}
        </Button>
      </form>
      <div className="space-y-4 rounded-2xl bg-white p-6 shadow-card">
        <h3 className="text-lg font-semibold text-slate-900">Podsumowanie</h3>
        <div className="space-y-3">
          {items.map((item) => (
            <div key={item.productId} className="flex items-center justify-between text-sm">
              <div className="text-slate-700">
                {item.productName} x{item.quantity}
              </div>
              <div className="font-semibold text-slate-900">${(item.unitPrice * item.quantity).toFixed(2)}</div>
            </div>
          ))}
        </div>
        <div className="border-t border-slate-200 pt-4 text-lg font-semibold text-slate-900">
          Razem: ${totalAfterDiscount.toFixed(2)}
        </div>
      </div>
      {showAddressModal ? (
        <div className="fixed inset-0 z-40 flex items-center justify-center bg-slate-900/50 px-4 py-6">
          <div className="w-full max-w-2xl rounded-2xl bg-white p-6 shadow-card">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-semibold text-slate-900">Nowy adres</h3>
              <button type="button" className="text-sm font-semibold text-slate-500 hover:text-slate-700" onClick={() => setShowAddressModal(false)}>
                Zamknij
              </button>
            </div>
            <div className="mt-4 grid gap-3 sm:grid-cols-2">
              <TextField label="Etykieta (opcjonalnie)" value={newAddress.label} onChange={(e) => setNewAddress((prev) => ({ ...prev, label: e.target.value }))} />
              <TextField label="Telefon (opcjonalnie)" value={newAddress.phone} onChange={(e) => setNewAddress((prev) => ({ ...prev, phone: e.target.value }))} />
              <TextField label="Ulica i numer" value={newAddress.line1} onChange={(e) => setNewAddress((prev) => ({ ...prev, line1: e.target.value }))} required />
              <TextField label="Mieszkanie (opcjonalnie)" value={newAddress.line2} onChange={(e) => setNewAddress((prev) => ({ ...prev, line2: e.target.value }))} />
              <TextField label="Miasto" value={newAddress.city} onChange={(e) => setNewAddress((prev) => ({ ...prev, city: e.target.value }))} required />
              <TextField label="Region (opcjonalnie)" value={newAddress.region} onChange={(e) => setNewAddress((prev) => ({ ...prev, region: e.target.value }))} />
              <TextField label="Kod pocztowy" value={newAddress.postalCode} onChange={(e) => setNewAddress((prev) => ({ ...prev, postalCode: e.target.value }))} required />
              <TextField label="Kraj" value={newAddress.country} onChange={(e) => setNewAddress((prev) => ({ ...prev, country: e.target.value }))} required />
              <label className="flex items-center gap-2 text-sm text-slate-700">
                <input
                  type="checkbox"
                  checked={newAddress.isDefault}
                  onChange={(e) => setNewAddress((prev) => ({ ...prev, isDefault: e.target.checked }))}
                />
                Ustaw jako domyslny
              </label>
            </div>
            {addressError ? <div className="mt-4 rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{addressError}</div> : null}
            <div className="mt-4 flex justify-end gap-3">
              <Button type="button" variant="secondary" onClick={() => setShowAddressModal(false)}>
                Anuluj
              </Button>
              <Button type="button" disabled={savingAddress} onClick={handleSaveAddress}>
                {savingAddress ? "Zapisywanie..." : "Zapisz adres"}
              </Button>
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
};

export default CheckoutPage;
