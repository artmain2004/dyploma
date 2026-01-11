import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { catalogApi } from "../api/catalogApi";
import type { ProductDetails, Review } from "../types";
import Button from "../components/Button";
import Loading from "../components/Loading";
import ErrorMessage from "../components/ErrorMessage";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { addItem, setItems } from "../features/cart/cartSlice";
import { cartApi } from "../api/cartApi";

const ProductDetailsPage = () => {
  const { id } = useParams();
  const dispatch = useAppDispatch();
  const token = useAppSelector((state) => state.auth.token);
  const [product, setProduct] = useState<ProductDetails | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reviewsError, setReviewsError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [favLoading, setFavLoading] = useState(false);

  useEffect(() => {
    if (!id) {
      return;
    }
    setLoading(true);
    setError(null);
    catalogApi
      .getProduct(id)
      .then(setProduct)
      .catch(() => setError("Nie udalo sie zaladowac produktu"))
      .finally(() => setLoading(false));
    catalogApi
      .getReviews(id)
      .then(setReviews)
      .catch(() => setReviewsError("Nie udalo sie zaladowac opinii"));
  }, [id]);

  const handleAdd = async () => {
    if (!product) {
      return;
    }
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
    if (!id) {
      return;
    }
    setFavLoading(true);
    try {
      await catalogApi.addFavorite(id);
    } finally {
      setFavLoading(false);
    }
  };

  const handleReviewSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!id) {
      return;
    }
    setSubmitting(true);
    setReviewsError(null);
    try {
      const newReview = await catalogApi.addReview(id, { rating, comment: comment || undefined });
      setReviews((prev) => [newReview, ...prev]);
      setComment("");
      setRating(5);
    } catch {
      setReviewsError("Nie udalo sie dodac opinii");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      <Link to="/products" className="text-sm font-semibold text-slate-500 hover:text-slate-900">
        Wroc do katalogu
      </Link>
      {loading ? <Loading /> : null}
      {error ? <ErrorMessage message={error} /> : null}
      {product ? (
        <div className="grid gap-8 rounded-2xl bg-white p-6 shadow-card md:grid-cols-[1.2fr_1fr]">
          <div className="aspect-[4/3] overflow-hidden rounded-xl bg-slate-50">
            {product.imageUrl ? (
              <img src={product.imageUrl} alt={product.name} className="h-full w-full object-contain p-4" />
            ) : (
              <div className="flex h-full items-center justify-center text-sm text-slate-400">Brak zdjecia</div>
            )}
          </div>
          <div className="space-y-4">
            <h1 className="text-2xl font-bold text-slate-900">{product.name}</h1>
            {product.description ? <p className="text-slate-600">{product.description}</p> : null}
            <div className="text-2xl font-bold text-slate-900">${product.price.toFixed(2)}</div>
            <div className="text-sm text-slate-500">
              Ocena: {product.averageRating.toFixed(1)} ({product.reviewsCount})
            </div>
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
      ) : null}
      {product ? (
        <div className="grid gap-6 rounded-2xl bg-white p-6 shadow-card">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-slate-900">Opinie</h3>
            <span className="text-sm text-slate-500">Lacznie: {product.reviewsCount}</span>
          </div>
          {token ? (
            <form onSubmit={handleReviewSubmit} className="grid gap-4 rounded-xl bg-slate-50 p-4">
              <label className="text-sm font-medium text-slate-700">
                Ocena
                <select
                  className="mt-2 w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
                  value={rating}
                  onChange={(e) => setRating(Number(e.target.value))}
                >
                  {[5, 4, 3, 2, 1].map((value) => (
                    <option key={value} value={value}>
                      {value}
                    </option>
                  ))}
                </select>
              </label>
              <label className="text-sm font-medium text-slate-700">
                Komentarz
                <textarea
                  className="mt-2 w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft"
                  rows={3}
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                />
              </label>
              {reviewsError ? <ErrorMessage message={reviewsError} /> : null}
              <Button type="submit" disabled={submitting}>
                {submitting ? "Dodawanie..." : "Dodaj opinie"}
              </Button>
            </form>
          ) : (
            <div className="rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">
              Zaloguj sie, aby dodac opinie.
            </div>
          )}
          <div className="space-y-4">
            {reviews.map((review) => (
              <div key={review.id} className="rounded-xl border border-slate-200 bg-white p-4">
                <div className="flex items-center justify-between text-sm text-slate-600">
                  <span>{review.userName ?? "Uzytkownik"}</span>
                  <span>{new Date(review.createdAtUtc).toLocaleString()}</span>
                </div>
                <div className="mt-2 text-sm font-semibold text-slate-900">Ocena: {review.rating}</div>
                {review.comment ? <p className="mt-2 text-sm text-slate-600">{review.comment}</p> : null}
              </div>
            ))}
            {reviews.length === 0 ? (
              <div className="rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">Brak opinii</div>
            ) : null}
          </div>
        </div>
      ) : null}
    </div>
  );
};

export default ProductDetailsPage;
