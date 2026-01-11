import { useState } from "react";
import Button from "./Button";
import { Link } from "react-router-dom";
import type { ProductListItem } from "../types";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { addItem, setItems } from "../features/cart/cartSlice";
import { catalogApi } from "../api/catalogApi";
import { cartApi } from "../api/cartApi";

const ProductCard = ({ product }: { product: ProductListItem }) => {
  const dispatch = useAppDispatch();
  const token = useAppSelector((state) => state.auth.token);
  const [favLoading, setFavLoading] = useState(false);

  const handleAdd = async () => {
    const payload = {
      productId: product.id,
      productName: product.name,
      unitPrice: product.price,
      quantity: 1,
      imageUrl: product.imageUrl
    };
    if (!token) {
      dispatch(addItem(payload));
      return;
    }
    try {
      const cart = await cartApi.addItem(payload);
      dispatch(setItems(cart.items));
    } catch {
      dispatch(addItem(payload));
    }
  };

  const handleFavorite = async () => {
    setFavLoading(true);
    try {
      await catalogApi.addFavorite(product.id);
    } finally {
      setFavLoading(false);
    }
  };

  return (
    <div className="flex h-full flex-col rounded-2xl bg-white p-4 shadow-card">
      <div className="aspect-[4/3] w-full overflow-hidden rounded-xl bg-slate-50">
        {product.imageUrl ? (
          <img src={product.imageUrl} alt={product.name} className="h-full w-full object-contain p-3" />
        ) : (
          <div className="flex h-full items-center justify-center text-sm text-slate-400">Brak zdjecia</div>
        )}
      </div>
      <div className="mt-4 flex flex-1 flex-col">
        <Link to={`/products/${product.id}`} className="text-base font-semibold text-slate-900 hover:text-slate-700">
          {product.name}
        </Link>
        <div className="mt-2 text-lg font-bold text-slate-900">${product.price.toFixed(2)}</div>
        <div className="mt-1 text-xs text-slate-500">Ocena {product.averageRating.toFixed(1)} · {product.reviewsCount}</div>
        <div className="mt-auto pt-4">
          <div className="flex flex-wrap gap-3">
            <Button onClick={handleAdd}>Dodaj do koszyka</Button>
            {token ? (
              <Button variant="secondary" onClick={handleFavorite} disabled={favLoading}>
                {favLoading ? "Dodawanie..." : "Ulubione"}
              </Button>
            ) : null}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;
