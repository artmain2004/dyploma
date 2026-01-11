import { useEffect, useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { logout, setToken, setUser } from "../features/auth/authSlice";
import { clear } from "../features/cart/cartSlice";
import { authApi } from "../api/authApi";
import { getRolesFromToken } from "../utils/jwt";

const Header = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { token, user } = useAppSelector((state) => state.auth);
  const roles = getRolesFromToken(token);
  const isAdmin = roles.includes("Admin");
  const [theme, setTheme] = useState(() => localStorage.getItem("theme") ?? "ocean");
  const [menuOpen, setMenuOpen] = useState(false);
  const itemCount = useAppSelector((state) => state.cart.items.reduce((sum, i) => sum + i.quantity, 0));

  const handleLogout = () => {
    const rememberMe = localStorage.getItem("rememberMe") === "true";
    if (!rememberMe) {
      authApi.logout().catch(() => null);
      localStorage.removeItem("rememberMe");
      localStorage.removeItem("rememberEmail");
    }
    localStorage.removeItem("accessToken");
    localStorage.removeItem("rememberPassword");
    dispatch(setToken(null));
    dispatch(setUser(null));
    dispatch(logout());
    dispatch(clear());
    navigate("/products");
  };

  const navClass = ({ isActive }: { isActive: boolean }) =>
    `inline-flex items-center gap-2 text-sm font-semibold ${isActive ? "text-slate-900" : "text-slate-500 hover:text-slate-800"}`;

  useEffect(() => {
    const root = document.documentElement;
    root.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  return (
    <header className="border-b border-slate-200 bg-white/90 backdrop-blur">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-4 px-4 py-4 sm:px-6 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex items-center justify-between lg:justify-start lg:gap-8">
          <Link to="/products" className="flex items-center gap-2 text-lg font-bold tracking-tight text-slate-900">
            <img src="/logo.png" alt="Logo" className="h-9 w-auto" />
            <span className="sr-only">Sklep</span>
          </Link>
          <button
            type="button"
            onClick={() => setMenuOpen((prev) => !prev)}
            className="inline-flex items-center justify-center rounded-xl border border-slate-200 bg-white px-3 py-2 text-slate-700 shadow-soft hover:bg-slate-50 lg:hidden"
            aria-label="Menu"
            aria-expanded={menuOpen}
          >
            <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden="true" focusable="false">
              <path fill="currentColor" d="M4 6h16v2H4V6Zm0 5h16v2H4v-2Zm0 5h16v2H4v-2Z" />
            </svg>
          </button>
          <nav className="hidden flex-wrap items-center gap-x-5 gap-y-2 lg:flex">
            <NavLink to="/products" className={navClass}>
              Produkty
            </NavLink>
            <NavLink to="/cart" className="relative inline-flex items-center text-slate-500 hover:text-slate-800">
              <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden="true" focusable="false">
                <path fill="currentColor" d="M7 18a2 2 0 1 0 0 4 2 2 0 0 0 0-4Zm10 0a2 2 0 1 0 0 4 2 2 0 0 0 0-4ZM6.3 6H21l-1.6 8.1a2 2 0 0 1-2 1.6H8a2 2 0 0 1-2-1.6L4.4 4H2V2h3a1 1 0 0 1 1 .8L6.3 6Z" />
              </svg>
              {itemCount > 0 ? (
                <span className="absolute -right-3 -top-2 rounded-full bg-slate-900 px-1.5 py-0.5 text-[10px] font-semibold text-white">
                  {itemCount}
                </span>
              ) : null}
            </NavLink>
            {isAdmin ? (
              <NavLink to="/admin" className={navClass}>
                Admin
              </NavLink>
            ) : null}
          </nav>
        </div>
        <div className="hidden flex-wrap items-center gap-3 lg:flex">
          <button
            onClick={() => setTheme((prev) => (prev === "ocean" ? "sunset" : "ocean"))}
            className="flex items-center gap-2 rounded-xl border border-slate-200 bg-white/70 px-3 py-2 text-xs font-semibold text-slate-700 shadow-soft hover:bg-white"
            aria-label="Zmien motyw"
          >
            {theme === "ocean" ? (
              <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                <path
                  fill="currentColor"
                  d="M12 3a1 1 0 0 1 1 1v1.1a1 1 0 0 1-2 0V4a1 1 0 0 1 1-1Zm0 15a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm8-5a1 1 0 0 1-1 1h-1.1a1 1 0 0 1 0-2H19a1 1 0 0 1 1 1ZM6.1 12a1 1 0 0 1-1 1H4a1 1 0 0 1 0-2h1.1a1 1 0 0 1 1 1Zm12.7-6.7a1 1 0 0 1 0 1.4l-.8.8a1 1 0 1 1-1.4-1.4l.8-.8a1 1 0 0 1 1.4 0ZM7.4 16.5a1 1 0 0 1 0 1.4l-.8.8a1 1 0 0 1-1.4-1.4l.8-.8a1 1 0 0 1 1.4 0Zm0-10a1 1 0 0 1-1.4 0l-.8-.8a1 1 0 1 1 1.4-1.4l.8.8a1 1 0 0 1 0 1.4Zm10.2 10a1 1 0 0 1-1.4 0l-.8-.8a1 1 0 0 1 1.4-1.4l.8.8a1 1 0 0 1 0 1.4Z"
                />
              </svg>
            ) : (
              <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                <path
                  fill="currentColor"
                  d="M21 14.5A8.5 8.5 0 0 1 9.5 3a7 7 0 1 0 11.5 11.5Z"
                />
              </svg>
            )}
            <span>{theme === "ocean" ? "Ocean" : "Sunset"}</span>
          </button>
          {token ? (
            <button
              onClick={handleLogout}
              className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 shadow-soft hover:bg-slate-50"
            >
              Wyloguj
            </button>
          ) : (
            <div className="flex items-center gap-3">
              <NavLink to="/login" className="text-sm font-semibold text-slate-500 hover:text-slate-900">
                Zaloguj
              </NavLink>
              <NavLink to="/register" className="text-sm font-semibold text-slate-500 hover:text-slate-900">
                Rejestracja
              </NavLink>
            </div>
          )}
          {user?.email ? (
            <Link to="/profile" className="text-sm font-semibold text-slate-500 hover:text-slate-800">
              {user.email}
            </Link>
          ) : null}
        </div>
        {menuOpen ? (
          <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-card lg:hidden">
            <nav className="grid justify-items-start gap-2">
              <NavLink to="/products" className={navClass} onClick={() => setMenuOpen(false)}>
                Produkty
              </NavLink>
              <NavLink
                to="/cart"
                className="relative inline-flex w-fit items-center text-slate-500 hover:text-slate-800"
                onClick={() => setMenuOpen(false)}
              >
                <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden="true" focusable="false">
                  <path fill="currentColor" d="M7 18a2 2 0 1 0 0 4 2 2 0 0 0 0-4Zm10 0a2 2 0 1 0 0 4 2 2 0 0 0 0-4ZM6.3 6H21l-1.6 8.1a2 2 0 0 1-2 1.6H8a2 2 0 0 1-2-1.6L4.4 4H2V2h3a1 1 0 0 1 1 .8L6.3 6Z" />
                </svg>
                {itemCount > 0 ? (
                  <span className="absolute -right-2 -top-2 flex h-5 min-w-[20px] items-center justify-center rounded-full bg-slate-900 px-1 text-[10px] font-semibold text-white">
                    {itemCount}
                  </span>
                ) : null}
              </NavLink>
            </nav>
            <div className="mt-4 flex flex-wrap items-center gap-3">
              <button
                onClick={() => setTheme((prev) => (prev === "ocean" ? "sunset" : "ocean"))}
                className="flex items-center gap-2 rounded-xl border border-slate-200 bg-white/70 px-3 py-2 text-xs font-semibold text-slate-700 shadow-soft hover:bg-white"
                aria-label="Zmien motyw"
              >
                {theme === "ocean" ? (
                  <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                    <path
                      fill="currentColor"
                      d="M12 3a1 1 0 0 1 1 1v1.1a1 1 0 0 1-2 0V4a1 1 0 0 1 1-1Zm0 15a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm8-5a1 1 0 0 1-1 1h-1.1a1 1 0 0 1 0-2H19a1 1 0 0 1 1 1ZM6.1 12a1 1 0 0 1-1 1H4a1 1 0 0 1 0-2h1.1a1 1 0 0 1 1 1Zm12.7-6.7a1 1 0 0 1 0 1.4l-.8.8a1 1 0 1 1-1.4-1.4l.8-.8a1 1 0 0 1 1.4 0ZM7.4 16.5a1 1 0 0 1 0 1.4l-.8.8a1 1 0 0 1-1.4-1.4l.8-.8a1 1 0 0 1 1.4 0Zm0-10a1 1 0 0 1-1.4 0l-.8-.8a1 1 0 1 1 1.4-1.4l.8.8a1 1 0 0 1 0 1.4Zm10.2 10a1 1 0 0 1-1.4 0l-.8-.8a1 1 0 0 1 1.4-1.4l.8.8a1 1 0 0 1 0 1.4Z"
                    />
                  </svg>
                ) : (
                  <svg viewBox="0 0 24 24" className="h-4 w-4" aria-hidden="true" focusable="false">
                    <path fill="currentColor" d="M21 14.5A8.5 8.5 0 0 1 9.5 3a7 7 0 1 0 11.5 11.5Z" />
                  </svg>
                )}
                <span>{theme === "ocean" ? "Ocean" : "Sunset"}</span>
              </button>
              {token ? (
                <button
                  onClick={() => {
                    handleLogout();
                    setMenuOpen(false);
                  }}
                  className="rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 shadow-soft hover:bg-slate-50"
                >
                  Wyloguj
                </button>
              ) : (
                <div className="flex items-center gap-3">
                  <NavLink to="/login" className="text-sm font-semibold text-slate-500 hover:text-slate-900" onClick={() => setMenuOpen(false)}>
                    Zaloguj
                  </NavLink>
                  <NavLink to="/register" className="text-sm font-semibold text-slate-500 hover:text-slate-900" onClick={() => setMenuOpen(false)}>
                    Rejestracja
                  </NavLink>
                </div>
              )}
              {user?.email ? (
                <Link to="/profile" className="text-sm font-semibold text-slate-500 hover:text-slate-800" onClick={() => setMenuOpen(false)}>
                  {user.email}
                </Link>
              ) : null}
            </div>
          </div>
        ) : null}
      </div>
    </header>
  );
};

export default Header;
