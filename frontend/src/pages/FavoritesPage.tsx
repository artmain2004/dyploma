import { useEffect, useState } from "react";
import { catalogApi } from "../api/catalogApi";
import type { ProductListItem } from "../types";
import ProductCard from "../components/ProductCard";
import Loading from "../components/Loading";
import ErrorMessage from "../components/ErrorMessage";

const FavoritesPage = () => {
  const [items, setItems] = useState<ProductListItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    setError(null);
    catalogApi
      .getFavorites()
      .then(setItems)
      .catch(() => setError("Nie udalo sie zaladowac ulubionych"))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-900">Ulubione</h2>
      {loading ? <Loading /> : null}
      {error ? <ErrorMessage message={error} /> : null}
      {!loading && !error ? (
        items.length === 0 ? (
          <div className="rounded-2xl bg-white p-6 text-sm text-slate-500 shadow-card">Brak ulubionych</div>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {items.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )
      ) : null}
    </div>
  );
};

export default FavoritesPage;
