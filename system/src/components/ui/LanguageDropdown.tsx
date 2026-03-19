"use client";

import { useState } from "react";
import { ChevronDown } from "lucide-react";
import LanguageOption from "@/components/icons/flags";

export default function LanguageDropdown() {
  const [open, setOpen] = useState(false);

  return (
    <div className="relative">

      {/* BOTÃO */}
      <button
        onClick={() => setOpen(!open)}
        className="flex items-center gap-1 text-white hover:text-[var(--orangecolor)] cursor-pointer"
      >
        <LanguageOption code="US" label="EN" />

        <ChevronDown
          size={17}
          className={`transition ${open ? "rotate-180" : ""}`}
        />
      </button>

      {/* DROPDOWN */}
      {open && (
        <div className="absolute right-0 mt-2 w-32 bg-[var(--headerground)] border border-gray-700 rounded shadow-[0_6px_14px_rgba(0,0,0,0.35)] z-50">
          <ul className="text-sm">

            <li
              onClick={() => setOpen(false)}
              className="px-3 py-2 hover:bg-gray-700 cursor-pointer"
            >
              <LanguageOption code="US" label="EN" />
            </li>

            <li
              onClick={() => setOpen(false)}
              className="px-3 py-2 hover:bg-gray-700 cursor-pointer"
            >
              <LanguageOption code="BR" label="PT" />
            </li>

            <li
              onClick={() => setOpen(false)}
              className="px-3 py-2 hover:bg-gray-700 cursor-pointer"
            >
              <LanguageOption code="ES" label="ES" />
            </li>

          </ul>
        </div>
      )}
    </div>
  );
}