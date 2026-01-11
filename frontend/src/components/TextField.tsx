import { InputHTMLAttributes } from "react";

type TextFieldProps = InputHTMLAttributes<HTMLInputElement> & {
  label?: string;
};

const TextField = ({ label, className, ...props }: TextFieldProps) => {
  return (
    <label className="block text-sm font-medium text-slate-700">
      {label ? <span className="mb-2 block">{label}</span> : null}
      <input
        className={`w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-slate-900 shadow-soft focus:border-slate-400 focus:outline-none ${
          className ?? ""
        }`.trim()}
        {...props}
      />
    </label>
  );
};

export default TextField;
