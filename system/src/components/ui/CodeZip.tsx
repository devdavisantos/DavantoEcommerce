"use client";

import { useEffect, useRef, useState } from "react";
import { MapPin, X } from "lucide-react";

type DeliveryLocation = {
  street: string;
  district: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  countryCode: string;
};

const LOCATION_KEY = "lastore-delivery-location";

export default function LocationModal() {
  const [open, setOpen] = useState(false);
  const [postalCode, setPostalCode] = useState("");
  const [location, setLocation] = useState("Update location");
  const [results, setResults] = useState<DeliveryLocation[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const controllerRef = useRef<AbortController | null>(null);

  const formatHeaderLocation = (location: DeliveryLocation) => {
    const maxStreetLength = 14;
    
    const street = location.street.length > maxStreetLength
      ? `${location.street.slice(0, maxStreetLength)}...`
      : location.street;
    
    return [
      street,
      location.state,
      location.countryCode,
    ].filter(Boolean).join(" · ");
  };

  useEffect(() => {
    try {
      const saved = localStorage.getItem(LOCATION_KEY);

      if (!saved) return;

      const savedLocation = JSON.parse(saved) as DeliveryLocation;

      setLocation(formatHeaderLocation(savedLocation));
    } catch {
      localStorage.removeItem(LOCATION_KEY);
    }
  }, []);

  useEffect(() => {
    const query = postalCode.trim();

    if (!open || query.length < 3) {
      controllerRef.current?.abort();
      setResults([]);
      setErrorMessage(null);
      setIsLoading(false);
      return;
    }

    const timer = setTimeout(async () => {
      controllerRef.current?.abort();

      const controller = new AbortController();
      controllerRef.current = controller;

      setIsLoading(true);
      setErrorMessage(null);

      try {
        const response = await fetch(`/api/location/search?q=${encodeURIComponent(query)}`, {
          signal: controller.signal,
          cache: "no-store",
        });

        if (!response.ok) {
          setResults([]);
          setErrorMessage("Unable to search this location.");
          return;
        }

        const locations = (await response.json()) as DeliveryLocation[];

        setResults(locations);

        if (locations.length === 0) {
          setErrorMessage("No locations found.");
        }
      } catch (error) {
        if (error instanceof Error && error.name === "AbortError") return;

        setResults([]);
        setErrorMessage("Unable to search this location.");
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false);
        }
      }
    }, 500);

    return () => {
      clearTimeout(timer);
      controllerRef.current?.abort();
    };
  }, [postalCode, open]);

  const handleSelectLocation = (selectedLocation: DeliveryLocation) => {
    localStorage.setItem(LOCATION_KEY, JSON.stringify(selectedLocation));

    setLocation(formatHeaderLocation(selectedLocation));
    setPostalCode("");
    setResults([]);
    setErrorMessage(null);
    setOpen(false);
  };

  const handleClose = () => {
    controllerRef.current?.abort();

    setPostalCode("");
    setResults([]);
    setErrorMessage(null);
    setIsLoading(false);
    setOpen(false);
  };

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="group flex items-center gap-1 text-left px-2 py-2 rounded-lg transition-colors cursor-pointer hover:bg-white/5"
      >
        <MapPin className="text-deep-orange transition group-hover:scale-110" />

        <div>
          <p className="text-xs text-gray-300 transition group-hover:text-white">
            Deliver to
          </p>

          <span className="font-bold transition group-hover:text-deep-orange text-sm">
            {location}
          </span>
        </div>
      </button>

      {open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50" onClick={handleClose}>
          <div className="bg-charcoal-dark border border-gray-700 p-6 rounded-lg w-[500px]" onClick={(e) => e.stopPropagation()}>
            <div className="flex mb-7 justify-between items-center">
              <h2 className="text-lg font-bold">
                Choose your location
              </h2>

              <X className="cursor-pointer hover:opacity-70" onClick={handleClose} />
            </div>

            <input
              type="text"
              value={postalCode}
              onChange={(e) => setPostalCode(e.target.value)}
              placeholder="Postal Code or address"
              autoComplete="postal-code"
              autoFocus
              className="w-full border p-2 rounded"
            />

            {isLoading && (
              <div className="flex justify-center py-5">
                <span className="h-5 w-5 animate-spin rounded-full border-2 border-white/30 border-t-deep-orange" />
              </div>
            )}

            {!isLoading && results.length > 0 && (
              <div className="mt-4 overflow-hidden rounded-lg border border-gray-700">
                {results.map((result, index) => {
                  const primaryLocation = [
                    result.street,
                    result.postalCode,
                  ].filter(Boolean).join(", ");

                  const secondaryLocation = [
                    result.district,
                    result.city,
                    result.state,
                    result.country,
                  ].filter(Boolean).join(", ");

                  return (
                    <button
                      key={`${result.street}-${result.postalCode}-${result.city}-${index}`}
                      type="button"
                      onClick={() => handleSelectLocation(result)}
                      className="w-full border-b border-gray-700 px-4 py-3 text-left transition last:border-b-0 hover:bg-white/5 cursor-pointer"
                    >
                      {primaryLocation && (
                        <p className="font-medium text-white">
                          {primaryLocation}
                        </p>
                      )}

                      {secondaryLocation && (
                        <p className="mt-1 text-sm text-gray-400">
                          {secondaryLocation}
                        </p>
                      )}
                    </button>
                  );
                })}
              </div>
            )}

            {!isLoading && errorMessage && (
              <p className="mt-3 text-sm text-red-400">
                {errorMessage}
              </p>
            )}
          </div>
        </div>
      )}
    </>
  );
}