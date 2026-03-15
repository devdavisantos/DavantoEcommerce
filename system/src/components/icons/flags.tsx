import ReactCountryFlag from "react-country-flag";

type LanguageOptionProps = {
  code: string;
  label: string;
};

export default function LanguageOption({ code, label }: LanguageOptionProps) {
  return (
    <div className="flex items-center gap-2">
      <ReactCountryFlag
        countryCode={code}
        svg
        style={{ width: "20px", height: "20px" }}
      />
      <span>{label}</span>
    </div>
  );
}