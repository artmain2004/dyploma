import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { ordersApi } from "../api/ordersApi";
import type { OrderSummary } from "../types";
import Loading from "../components/Loading";
import ErrorMessage from "../components/ErrorMessage";

const MyOrdersPage = () => {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    setError(null);
    ordersApi
      .getMyOrders()
      .then(setOrders)
      .catch(() => setError("Nie udało się załadować zamówień"))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Moje zamówienia</h2>
      {loading ? <Loading /> : null}
      {error ? <ErrorMessage message={error} /> : null}
      {!loading && !error ? (
        <div className="space-y-4">
          {orders.length === 0 ? (
            <div className="rounded-2xl bg-white p-6 text-sm text-slate-500 shadow-card">Brak zamówień</div>
          ) : (
            orders.map((order) => (
              <Link
                key={order.id}
                to={`/orders/${order.id}`}
                className="flex items-center justify-between rounded-2xl bg-white p-4 shadow-card hover:bg-slate-50"
              >
                <div>
                  <div className="text-sm font-semibold text-slate-900">{order.orderNumber}</div>
                  <div className="text-xs text-slate-500">{new Date(order.createdAtUtc).toLocaleString()}</div>
                </div>
                <div className="text-right">
                  <div className="text-sm font-semibold text-slate-900">${order.totalPrice.toFixed(2)}</div>
                  <div className="text-xs text-slate-500">{order.status}</div>
                </div>
              </Link>
            ))
          )}
        </div>
      ) : null}
    </div>
  );
};

export default MyOrdersPage;
