"use client";

import { useState } from "react";
import { MapPin } from "lucide-react";

export default function LocationModal() {
  const [open, setOpen] = useState(false);

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="flex items-center gap-1 text-left cursor-pointer group"
      >
        <MapPin className="text-[var(--orangecolor)] transition group-hover:scale-110" />

        <div>
          <p className="text-xs text-gray-300 transition group-hover:text-white">Deliver to</p>
          <span className="font-bold transition group-hover:text-[var(--orangecolor)]">Update location</span>
        </div>
      </button>

      {open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-[350px]">
            <h2 className="text-lg font-bold mb-4">
              Enter your ZIP Code
            </h2>

            <input
              type="text"
              placeholder="ZIP Code"
              className="w-full border p-2 rounded mb-4"
            />

            <div className="flex justify-end gap-3">
              <button
                onClick={() => setOpen(false)}
                className="text-gray-500 cursor-pointer"
              >
                Cancel
              </button>

              <button className="bg-[var(--orangecolor)] text-white px-4 py-2 rounded cursor-pointer">
                Save
              </button>
            </div>

          </div>
        </div>
      )}
    </>
  );
}