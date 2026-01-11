import { Link } from "react-router-dom";

const AdminDashboardPage = () => {
  const tiles = [
    { to: "/admin/products", label: "Produkty" },
    { to: "/admin/categories", label: "Kategorie" },
    { to: "/admin/orders", label: "Zamowienia" },
    { to: "/admin/promocodes", label: "Promocje" },
    { to: "/admin/users", label: "Uzytkownicy" }
  ];

  return (
    <div className="grid gap-6 sm:grid-cols-2">
      {tiles.map((tile) => (
        <Link key={tile.to} to={tile.to} className="rounded-2xl bg-white p-6 text-lg font-semibold text-slate-900 shadow-card hover:bg-slate-50">
          {tile.label}
        </Link>
      ))}
    </div>
  );
};

export default AdminDashboardPage;
