"use client";

import { useState } from "react";
import { MapPin, X } from "lucide-react";

export default function LocationModal() {
  const [open, setOpen] = useState(false);

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="flex items-center gap-1 text-left cursor-pointer group"
      >
        <MapPin className="text-deep-orange transition group-hover:scale-110" />

        <div>
          <p className="text-xs text-gray-300 transition group-hover:text-white">
            Deliver to
          </p>
          <span className="font-bold transition group-hover:text-deep-orange">
            Update location
          </span>
        </div>
      </button>

      {open && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
          onClick={() => setOpen(false)} // fecha ao clicar fora
        >
          <div
            className="bg-charcoal-dark border border-gray-700 p-6 rounded-lg w-auto"
            onClick={(e) => e.stopPropagation()} // impede fechar ao clicar dentro
          >
            <div className="flex mb-7 justify-between items-center">
              <h2 className="text-lg font-bold">
                Choose your location
              </h2>

              <X
                className="cursor-pointer hover:opacity-70"
                onClick={() => setOpen(false)}
              />
            </div>

            <div className="flex gap-5">
              <input
                type="text"
                placeholder="Postal Code"
                className="w-2xs border p-2 rounded"
              />

              <button
                className="bg-deep-orange hover:bg-amber-500 text-white px-5 py-2.5 rounded-lg  hover:shadow-l cursor-pointer"
              >
                Confirm
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}