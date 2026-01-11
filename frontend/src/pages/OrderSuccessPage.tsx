import { Link, useLocation } from "react-router-dom";
import { useAppSelector } from "../app/hooks";

const OrderSuccessPage = () => {
  const location = useLocation();
  const token = useAppSelector((state) => state.auth.token);
  const orderNumber = (location.state as { orderNumber?: string } | null)?.orderNumber;

  return (
    <div className="rounded-2xl bg-white p-8 text-center shadow-card">
      <h2 className="text-2xl font-semibold text-slate-900">Zamówienie złożone</h2>
      {orderNumber ? <p className="mt-2 text-sm text-slate-500">Numer zamówienia: {orderNumber}</p> : null}
      <div className="mt-6 flex flex-wrap justify-center gap-3">
        <Link to="/products" className="rounded-xl bg-slate-900 px-5 py-2.5 text-sm font-semibold text-white shadow-soft hover:bg-slate-800">
          Wróć do produktów
        </Link>
        {token ? (
          <Link to="/my-orders" className="rounded-xl border border-slate-200 px-5 py-2.5 text-sm font-semibold text-slate-700 shadow-soft hover:bg-slate-50">
            Przejdź do zamówień
          </Link>
        ) : null}
      </div>
    </div>
  );
};

export default OrderSuccessPage;
