"use client";

import { ChevronDown } from "lucide-react";
import { useEffect, useRef, useState } from "react";

type ProductOption = {
  value: string;
  label: string;
};

type ProductOptionDropdownProps = {
  id: string;
  value: string;
  options: ProductOption[];
  placeholder?: string;
  onChange: (value: string) => void;
};

export default function ProductOptionDropdown({ id, value, options, placeholder = "Select", onChange }: ProductOptionDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const selectedOption = options.find((option) => option.value === value);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelect = (selectedValue: string) => {
    onChange(selectedValue);
    setIsOpen(false);
  };

  return (
    <div ref={dropdownRef} className="relative">
      <button
        id={id}
        type="button"
        onClick={() => setIsOpen((current) => !current)}
        className="flex w-full cursor-pointer items-center justify-between rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-left outline-none transition focus:border-deep-orange"
      >
        <span className={selectedOption ? "text-white" : "text-gray-500"}>
          {selectedOption?.label || placeholder}
        </span>

        <ChevronDown
          size={17}
          className={`text-gray-500 transition-transform ${isOpen ? "rotate-180" : ""}`}
        />
      </button>

      {isOpen && (
        <div className="absolute left-0 top-full z-50 mt-2 max-h-64 w-full overflow-y-auto rounded-lg border border-white/10 bg-charcoal-dark shadow-xl">
          {options.map((option) => (
            <button
              key={option.value}
              type="button"
              onClick={() => handleSelect(option.value)}
              className={`flex w-full cursor-pointer items-center px-4 py-3 text-left text-sm transition hover:bg-white/5 ${
                value === option.value
                  ? "bg-white/5 text-deep-orange"
                  : "text-gray-200"
              }`}
            >
              {option.label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}