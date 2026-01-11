import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { ordersApi } from "../api/ordersApi";
import type { OrderDetails } from "../types";
import Loading from "../components/Loading";
import ErrorMessage from "../components/ErrorMessage";

const OrderDetailsPage = () => {
  const { id } = useParams();
  const [order, setOrder] = useState<OrderDetails | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      return;
    }
    setLoading(true);
    setError(null);
    ordersApi
      .getOrderById(id)
      .then(setOrder)
      .catch(() => setError("Nie udało się załadować zamówienia"))
      .finally(() => setLoading(false));
  }, [id]);

  return (
    <div className="space-y-6">
      <Link to="/my-orders" className="text-sm font-semibold text-slate-500 hover:text-slate-900">
        Wróć do zamówień
      </Link>
      {loading ? <Loading /> : null}
      {error ? <ErrorMessage message={error} /> : null}
      {order ? (
        <div className="space-y-4 rounded-2xl bg-white p-6 shadow-card">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-xl font-semibold text-slate-900">{order.orderNumber}</h2>
              <div className="text-sm text-slate-500">{new Date(order.createdAtUtc).toLocaleString()}</div>
            </div>
            <div className="text-right">
              <div className="text-lg font-semibold text-slate-900">${order.totalPrice.toFixed(2)}</div>
              <div className="text-sm text-slate-500">{order.status}</div>
            </div>
          </div>
          <div className="rounded-xl bg-slate-50 p-4 text-sm text-slate-600">
            <div className="font-semibold text-slate-700">Email: {order.customerEmail}</div>
            {order.shippingAddress ? <div>Adres: {order.shippingAddress}</div> : null}
          </div>
          <div className="space-y-3">
            {order.items.map((item) => (
              <div key={item.id} className="flex items-center justify-between text-sm">
                <div className="text-slate-700">
                  {item.productName} x{item.quantity}
                </div>
                <div className="font-semibold text-slate-900">${(item.unitPrice * item.quantity).toFixed(2)}</div>
              </div>
            ))}
          </div>
        </div>
      ) : null}
    </div>
  );
};

export default OrderDetailsPage;
