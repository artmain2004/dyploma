import { useEffect, useState } from "react";
import { ordersApi } from "../../api/ordersApi";
import Button from "../../components/Button";
import ErrorMessage from "../../components/ErrorMessage";
import type { OrderSummary } from "../../types";

const AdminOrdersPage = () => {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [error, setError] = useState<string | null>(null);

  const load = async (pageNumber = page) => {
    setError(null);
    try {
      const result = await ordersApi.adminGetOrders({ page: pageNumber, pageSize });
      setOrders(result.items);
      setPage(result.page);
      setTotal(result.totalCount);
    } catch {
      setError("Nie udalo sie zaladowac zamowien");
    }
  };

  useEffect(() => {
    load(1);
  }, []);

  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  const handleStatus = async (id: string, status: string) => {
    try {
      await ordersApi.adminUpdateOrderStatus(id, status);
      await load(page);
    } catch {
      setError("Nie udalo sie zaktualizowac statusu");
    }
  };

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Zamowienia</h2>
      {error ? <ErrorMessage message={error} /> : null}
      <div className="space-y-3">
        {orders.map((order) => (
          <div key={order.id} className="rounded-2xl bg-white p-4 shadow-card">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-sm font-semibold text-slate-900">{order.orderNumber}</div>
                <div className="text-xs text-slate-500">{new Date(order.createdAtUtc).toLocaleString()}</div>
              </div>
              <div className="text-right">
                <div className="text-sm font-semibold text-slate-900">${order.totalPrice.toFixed(2)}</div>
                <div className="text-xs text-slate-500">{order.status}</div>
              </div>
            </div>
            <div className="mt-3 flex flex-wrap gap-2">
              {["New", "Completed", "Cancelled"].map((status) => (
                <Button key={status} variant="secondary" onClick={() => handleStatus(order.id, status)}>
                  {status}
                </Button>
              ))}
            </div>
          </div>
        ))}
        {orders.length === 0 ? <div className="rounded-2xl bg-white p-4 text-sm text-slate-500 shadow-card">Brak zamowien</div> : null}
      </div>
      <div className="flex items-center justify-between">
        <div className="text-sm text-slate-500">Strona {page} z {totalPages}</div>
        <div className="flex gap-2">
          <Button variant="secondary" onClick={() => load(Math.max(1, page - 1))} disabled={page <= 1}>Wstecz</Button>
          <Button variant="secondary" onClick={() => load(Math.min(totalPages, page + 1))} disabled={page >= totalPages}>Dalej</Button>
        </div>
      </div>
    </div>
  );
};

export default AdminOrdersPage;
