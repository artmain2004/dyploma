import { useEffect, useState } from "react";
import { catalogApi } from "../api/catalogApi";
import type { Category, ProductListItem } from "../types";
import ProductCard from "../components/ProductCard";
import Button from "../components/Button";
import TextField from "../components/TextField";
import Loading from "../components/Loading";
import ErrorMessage from "../components/ErrorMessage";

const ProductsPage = () => {
  const [products, setProducts] = useState<ProductListItem[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");
  const [sort, setSort] = useState("newest");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(12);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async (
    pageNumber = page,
    searchValue = search,
    categoryValue = categoryId,
    minValue = minPrice,
    maxValue = maxPrice,
    sortValue = sort
  ) => {
    setLoading(true);
    setError(null);
    try {
      const result = await catalogApi.getProducts({
        search: searchValue || undefined,
        categoryId: categoryValue || undefined,
        minPrice: minValue ? Number(minValue) : undefined,
        maxPrice: maxValue ? Number(maxValue) : undefined,
        sort: sortValue || undefined,
        page: pageNumber,
        pageSize
      });
      setProducts(result.items);
      setTotalCount(result.totalCount);
      setPage(result.page);
    } catch {
      setError("Nie udalo sie zaladowac produktow");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    catalogApi.getCategories().then(setCategories).catch(() => null);
  }, []);

  useEffect(() => {
    const value = search.trim();
    if (value.length > 0 && value.length < 2) {
      return;
    }
    const timer = setTimeout(() => {
      load(1, value, categoryId, minPrice, maxPrice, sort);
    }, 350);
    return () => clearTimeout(timer);
  }, [search, categoryId, minPrice, maxPrice, sort]);

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <div className="space-y-6">
      <div className="rounded-2xl bg-white p-6 shadow-card">
        <div className="grid gap-4 md:grid-cols-[1fr_200px_120px_120px_180px_auto] md:items-end">
          <TextField label="Szukaj" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Nazwa lub opis" />
          <label className="block text-sm font-medium text-slate-700">
            <span className="mb-2 block">Kategoria</span>
            <select
              className="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
            >
              <option value="">Wszystkie kategorie</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
          <TextField label="Cena od" type="number" value={minPrice} onChange={(e) => setMinPrice(e.target.value)} />
          <TextField label="Cena do" type="number" value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} />
          <label className="block text-sm font-medium text-slate-700">
            <span className="mb-2 block">Sortowanie</span>
            <select
              className="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
              value={sort}
              onChange={(e) => setSort(e.target.value)}
            >
              <option value="newest">Najnowsze</option>
              <option value="price_asc">Cena rosnaco</option>
              <option value="price_desc">Cena malejaco</option>
              <option value="rating_desc">Najlepsza ocena</option>
            </select>
          </label>
          <Button onClick={() => load(1, search.trim(), categoryId, minPrice, maxPrice, sort)}>Szukaj</Button>
        </div>
      </div>

      {loading ? <Loading /> : null}
      {error ? <ErrorMessage message={error} /> : null}

      {!loading && !error ? (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      ) : null}

      <div className="flex items-center justify-between">
        <div className="text-sm text-slate-500">
          Strona {page} z {totalPages}
        </div>
        <div className="flex items-center gap-3">
          <Button
            variant="secondary"
            onClick={() => load(Math.max(1, page - 1), search.trim(), categoryId, minPrice, maxPrice, sort)}
            disabled={page <= 1}
          >
            Wstecz
          </Button>
          <Button
            variant="secondary"
            onClick={() => load(Math.min(totalPages, page + 1), search.trim(), categoryId, minPrice, maxPrice, sort)}
            disabled={page >= totalPages}
          >
            Dalej
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ProductsPage;
