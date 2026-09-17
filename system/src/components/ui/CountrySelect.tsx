"use client";

import { useEffect, useRef, useState } from "react";
import { ChevronDown } from "@/components/icons/icons";
import { countries } from "@/data/countries";

type CountrySelectProps = {
  value: string;
  onChange: (value: string) => void;
  invalid?: boolean;
};

export default function CountrySelect({
  value,
  onChange,
  invalid = false,
}: CountrySelectProps) {
  const [isOpen, setIsOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const selectedCountry = countries.find(
    (country) => country.code === value
  );

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const handleSelect = (code: string) => {
    onChange(code);
    setIsOpen(false);
  };

  return (
    <div
      ref={containerRef}
      className={`relative ${isOpen} ? "z-50" : "z-0"}`}
    >
      <button
        type="button"
        onClick={() => setIsOpen((current) => !current)}
        className={`
          flex w-full items-center justify-between
          rounded-lg border bg-steel-gray
          px-4 py-3
          text-left
          outline-none transition
          cursor-pointer
          ${
            invalid
              ? "border-red-500"
              : isOpen
                ? "border-deep-orange"
                : "border-white/10"
          }
        `}
      >
        <span
          className={
            selectedCountry
              ? "text-white"
              : "text-gray-400"
          }
        >
          {selectedCountry?.name ?? "Select a country"}
        </span>

        <ChevronDown
          size={18}
          className={`
            ml-4 shrink-0 text-gray-400
            transition-transform duration-200
            ${isOpen ? "rotate-180" : "rotate-0"}
          `}
        />
      </button>

      {isOpen && (
        <div
          className="absolute left-0 top-full z-50 mt-2 max-h-64 w-full overflow-y-auto rounded-lg border border-white/10 bg-charcoal-dark shadow-xl"
        >
          {countries.map((country) => (
            <button
              key={country.code}
              type="button"
              onClick={() => handleSelect(country.code)}
              className={`flex w-full items-center px-4 py-3 text-left text-sm transition cursor-pointer hover:bg-white/5
                ${value === country.code
                    ? "bg-white/5 text-deep-orange"
                    : "text-gray-200"}
              `}
            >
              {country.name}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}