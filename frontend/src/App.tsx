import { useEffect } from "react";
import { useAppDispatch } from "./app/hooks";
import { setToken, setUser } from "./features/auth/authSlice";
import { setItems } from "./features/cart/cartSlice";
import { authApi } from "./api/authApi";
import { cartApi } from "./api/cartApi";
import AppRouter from "./routes/AppRouter";
import Header from "./components/Header";

const App = () => {
  const dispatch = useAppDispatch();

  useEffect(() => {
    const bootstrap = async () => {
      const token = localStorage.getItem("accessToken");
      const rememberMe = localStorage.getItem("rememberMe") === "true";
      if (token) {
        dispatch(setToken(token));
        try {
          const user = await authApi.me();
          dispatch(setUser(user));
          try {
            const cart = await cartApi.getCart();
            dispatch(setItems(cart.items));
          } catch {
            dispatch(setItems([]));
          }
          return;
        } catch {
          localStorage.removeItem("accessToken");
          dispatch(setToken(null));
        }
      }
      try {
        const refreshed = await authApi.refreshToken(rememberMe);
        localStorage.setItem("accessToken", refreshed.accessToken);
        dispatch(setToken(refreshed.accessToken));
        const user = await authApi.me();
        dispatch(setUser(user));
        try {
          const cart = await cartApi.getCart();
          dispatch(setItems(cart.items));
        } catch {
          dispatch(setItems([]));
        }
      } catch {
        dispatch(setToken(null));
        dispatch(setUser(null));
        dispatch(setItems([]));
      }
    };
    void bootstrap();
  }, [dispatch]);

  return (
    <div className="min-h-screen">
      <Header />
      <main className="mx-auto w-full max-w-6xl px-6 py-8">
        <AppRouter />
      </main>
    </div>
  );
};

export default App;
