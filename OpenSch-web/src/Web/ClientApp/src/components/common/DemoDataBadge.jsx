import Icon from "./Icon";

export default function DemoDataBadge({ className = "" }) {
  return (
    <span
      className={
        "inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-surface-container text-primary border border-outline-variant text-[11px] font-semibold " +
        className
      }
    >
      <Icon name="science" className="text-[14px]" />
      Data demo (sintetis), bukan data resmi
    </span>
  );
}