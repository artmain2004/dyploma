import Button from "./Button";
import { CartItem } from "../types";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { decrement, increment, removeItem, setItems } from "../features/cart/cartSlice";
import { cartApi } from "../api/cartApi";

const CartItemRow = ({ item }: { item: CartItem }) => {
  const dispatch = useAppDispatch();
  const token = useAppSelector((state) => state.auth.token);
  const subtotal = item.unitPrice * item.quantity;

  return (
    <div className="grid grid-cols-1 gap-4 rounded-2xl bg-white p-4 shadow-card md:grid-cols-[1.5fr_0.6fr_0.6fr_0.4fr] md:items-center">
      <div className="flex items-center gap-4">
        <div className="h-16 w-16 overflow-hidden rounded-xl bg-slate-100">
          {item.imageUrl ? (
            <img src={item.imageUrl} alt={item.productName} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full items-center justify-center text-xs text-slate-400">Brak zdjecia</div>
          )}
        </div>
        <div>
          <div className="text-sm font-semibold text-slate-900">{item.productName}</div>
          <div className="text-sm text-slate-500">${item.unitPrice.toFixed(2)}</div>
        </div>
      </div>
      <div className="flex items-center gap-2">
        <Button
          variant="secondary"
          onClick={async () => {
            if (!token) {
              dispatch(decrement(item.productId));
              return;
            }
            const next = Math.max(1, item.quantity - 1);
            try {
              const cart = await cartApi.updateQuantity(item.productId, next);
              dispatch(setItems(cart.items));
            } catch {
              dispatch(decrement(item.productId));
            }
          }}
        >
          -
        </Button>
        <span className="min-w-[32px] text-center text-sm font-semibold">{item.quantity}</span>
        <Button
          variant="secondary"
          onClick={async () => {
            if (!token) {
              dispatch(increment(item.productId));
              return;
            }
            const next = item.quantity + 1;
            try {
              const cart = await cartApi.updateQuantity(item.productId, next);
              dispatch(setItems(cart.items));
            } catch {
              dispatch(increment(item.productId));
            }
          }}
        >
          +
        </Button>
      </div>
      <div className="text-sm font-semibold text-slate-900">${subtotal.toFixed(2)}</div>
      <Button
        variant="secondary"
        onClick={async () => {
          if (!token) {
            dispatch(removeItem(item.productId));
            return;
          }
          try {
            const cart = await cartApi.removeItem(item.productId);
            dispatch(setItems(cart.items));
          } catch {
            dispatch(removeItem(item.productId));
          }
        }}
      >
        Usun
      </Button>
    </div>
  );
};

export default CartItemRow;
