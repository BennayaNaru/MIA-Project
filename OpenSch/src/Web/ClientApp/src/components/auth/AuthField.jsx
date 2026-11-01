import { useState } from "react";
import Icon from "../common/Icon";

export default function AuthField({ id, label, icon, type = "text", right, ...props }) {
  const [show, setShow] = useState(false);
  const isPassword = type === "password";

  return (
    <div>
      <div className="flex items-center justify-between mb-1.5">
        <label htmlFor={id} className="text-sm font-semibold text-on-surface">{label}</label>
        {right}
      </div>
      <div className="relative">
        <Icon name={icon} className="absolute left-3 top-1/2 -translate-y-1/2 text-outline text-[20px]" />
        <input
          id={id}
          type={isPassword && show ? "text" : type}
          className="w-full pl-10 pr-10 py-2.5 rounded-xl border border-outline-variant bg-white text-sm text-on-surface placeholder:text-outline focus:border-secondary focus:ring-secondary"
          {...props}
        />
        {isPassword && (
          <button
            type="button"
            onClick={() => setShow((s) => !s)}
            aria-label={show ? "Sembunyikan kata sandi" : "Tampilkan kata sandi"}
            className="absolute right-3 top-1/2 -translate-y-1/2 text-outline hover:text-primary"
          >
            <Icon name={show ? "visibility_off" : "visibility"} className="text-[20px]" />
          </button>
        )}
      </div>
    </div>
  );
}