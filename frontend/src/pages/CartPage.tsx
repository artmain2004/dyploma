import { Link, useNavigate } from "react-router-dom";
import CartItemRow from "../components/CartItemRow";
import Button from "../components/Button";
import { useAppSelector } from "../app/hooks";

const CartPage = () => {
  const navigate = useNavigate();
  const items = useAppSelector((state) => state.cart.items);
  const total = items.reduce((sum, i) => sum + i.unitPrice * i.quantity, 0);

  if (items.length === 0) {
    return (
      <div className="rounded-2xl bg-white p-8 text-center shadow-card">
        <h2 className="text-xl font-semibold text-slate-900">Koszyk jest pusty</h2>
        <p className="mt-2 text-sm text-slate-500">Dodaj produkty z katalogu.</p>
        <div className="mt-6">
          <Link to="/products" className="text-sm font-semibold text-slate-700 hover:text-slate-900">
            Przejdź do produktów
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="space-y-4">
        {items.map((item) => (
          <CartItemRow key={item.productId} item={item} />
        ))}
      </div>
      <div className="flex flex-col items-end gap-4 rounded-2xl bg-white p-6 shadow-card">
        <div className="text-lg font-semibold text-slate-900">Razem: ${total.toFixed(2)}</div>
        <Button onClick={() => navigate("/checkout")}>Przejdź do zamówienia</Button>
      </div>
    </div>
  );
};

export default CartPage;
