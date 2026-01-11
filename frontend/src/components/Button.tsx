import { ButtonHTMLAttributes } from "react";

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary";
};

const Button = ({ variant = "primary", className, ...props }: ButtonProps) => {
  const base = "inline-flex items-center justify-center rounded-xl px-5 py-2.5 text-sm font-semibold transition";
  const style =
    variant === "primary"
      ? "bg-[color:var(--brand)] text-white shadow-soft hover:bg-[color:var(--brand-soft)]"
      : "bg-[color:var(--surface)] text-[color:var(--text)] shadow-soft ring-1 ring-[color:var(--ring)] hover:bg-[color:var(--surface-muted)]";
  return <button className={`${base} ${style} ${className ?? ""}`.trim()} {...props} />;
};

export default Button;
