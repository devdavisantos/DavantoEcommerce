"use client";
import { authenticatedFetch } from "@/lib/auth-client";

import { useEffect, useState } from "react";
import { ChevronDown, CircleCheck, CircleX, Edit3, MapPin, Plus, Save, Trash2 } from "@/components/icons/icons";

import CountrySelect from "@/components/ui/CountrySelect";
import { countries } from "@/data/countries";

type Address = {
  externalId: string;
  name: string | null;
  recipientName: string;
  street: string;
  number: string;
  complement?: string | null;
  district: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  isDefault: boolean;
};

type AddressForm = {
  name: string;
  recipientName: string;
  street: string;
  number: string;
  complement: string;
  district: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  isDefault: boolean;
};

const initialForm: AddressForm = {
  name: "",
  recipientName: "",
  street: "",
  number: "",
  complement: "",
  district: "",
  city: "",
  state: "",
  zipCode: "",
  country: "",
  isDefault: false,
};

export default function AddressPage() {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [form, setForm] = useState<AddressForm>(initialForm);

  const [editingExternalId, setEditingExternalId] = useState<string | null>(null);

  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  const [showValidation, setShowValidation] = useState(false);
  const [isFormOpen, setIsFormOpen] = useState(false);

  const [alert, setAlert] = useState<{
    type: "success" | "error";
    message: string;
  } | null>(null);

  useEffect(() => {
    loadAddresses();
  }, []);

  useEffect(() => {
    if (!alert) return;

    const timeout = setTimeout(() => {
      setAlert(null);
    }, 4000);

    return () => clearTimeout(timeout);
  }, [alert]);

  const loadAddresses = async () => {
    setIsLoading(true);

    try {
      const response = await authenticatedFetch("/api/customer/addresses", {
        method: "GET",
        credentials: "same-origin",
        cache: "no-store",
      });

      if (!response.ok) {
        const error = await response.json().catch(() => null);

        setAlert({
          type: "error",
          message:
            error?.message ??
            "Unable to load addresses.",
        });

        return;
      }

      const data = await response.json();

      setAddresses(
        Array.isArray(data)
          ? data
          : []
      );
    } catch {
      setAlert({
        type: "error",
        message: "Unable to load addresses.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleChange = (
    field: keyof AddressForm,
    value: string
  ) => {
    setForm((current) => ({
      ...current,
      [field]: value,
    }));
  };

  const isFieldInvalid = (
  field: keyof AddressForm
  ) => {
    if (!showValidation) {
      return false;
    }
  
    const value = form[field];
  
    return (
      typeof value === "string" &&
      !value.trim()
    );
  };

  const getCountryName = (
    code: string
  ) => {
    return (
      countries.find(
        (country) =>
          country.code === code
      )?.name ?? code
    );
  };

  const clearForm = () => {
    setForm(initialForm);
    setEditingExternalId(null);
    setShowValidation(false);
    setIsFormOpen(false);
  };

  const handleToggleForm = () => {
    if (isFormOpen) {
      clearForm();
      return;
    }

    setIsFormOpen(true);
  };

  const handleEdit = (
    address: Address
  ) => {
    setEditingExternalId(address.externalId);
    setShowValidation(false);
    setIsFormOpen(true);

    setForm({
      name: address.name ?? "",
      recipientName: address.recipientName ?? "",
      street: address.street ?? "",
      number: address.number ?? "",
      complement: address.complement ?? "",
      district: address.district ?? "",
      city: address.city ?? "",
      state: address.state ?? "",
      zipCode: address.zipCode ?? "",
      country: address.country ?? "",
      isDefault: address.isDefault,
    });
  };

  const handleSave = async () => {
    if (isSaving) return;

    setShowValidation(true);

    if (
      !form.recipientName.trim() ||
      !form.street.trim() ||
      !form.number.trim() ||
      !form.district.trim() ||
      !form.city.trim() ||
      !form.state.trim() ||
      !form.zipCode.trim() ||
      !form.country.trim()
    ) {
      setAlert({
        type: "error",
        message:
          "Fill in all required address fields.",
      });

      return;
    }

    setIsSaving(true);
    setAlert(null);

    try {
      const response = await authenticatedFetch(
        editingExternalId
          ? `/api/customer/addresses/${editingExternalId}`
          : "/api/customer/addresses",
        {
          method: editingExternalId
            ? "PATCH"
            : "POST",

          headers: {
            "Content-Type":
              "application/json",
          },

          credentials:
            "same-origin",

          body: JSON.stringify({
            name: form.name.trim() || null,
            recipientName: form.recipientName.trim(),
            street: form.street.trim(),
            number: form.number.trim(),
            complement: form.complement.trim() || null,
            district: form.district.trim(),
            city: form.city.trim(),
            state: form.state.trim(),
            zipCode: form.zipCode.trim(),
            country: form.country.trim(),
            isDefault: form.isDefault,
          }),
        }
      );

      if (!response.ok) {
        const error =
          await response
            .json()
            .catch(() => null);

        setAlert({
          type: "error",

          message:
            error?.message ??
            (editingExternalId
              ? "Unable to update address."
              : "Unable to create address."),
        });

        return;
      }

      setAlert({
        type: "success",

        message: editingExternalId
          ? "Address updated successfully."
          : "Address created successfully.",
      });

      clearForm();

      await loadAddresses();
    } catch {
      setAlert({
        type: "error",

        message: editingExternalId
          ? "Unable to update address."
          : "Unable to create address.",
      });
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async (externalId: string) => {
    try {
      const response = await authenticatedFetch(
        `/api/customer/addresses/${externalId}`,
        {
          method: "DELETE",
          credentials: "same-origin",
        }
      );

      if (!response.ok) {
        const error = await response
          .json()
          .catch(() => null);

        setAlert({
          type: "error",
          message: error?.message ?? "Unable to delete address.",
        });

        return;
      }

      setAlert({
        type: "success",
        message: "Address deleted successfully.",
      });

      if (editingExternalId === externalId) {
        clearForm();
      }

      await loadAddresses();
    } catch {
      setAlert({
        type: "error",
        message: "Unable to delete address.",
      });
    }
  };

  return (
    <>
      <div className="space-y-8">
        <section>
          <div className="mb-6 flex items-center gap-3">
            <MapPin
              size={22}
              className="text-deep-orange"
            />

            <div>
              <h1 className="text-xl font-semibold text-white">
                Addresses
              </h1>

              <p className="mt-1 text-sm text-gray-400">
                Add and manage your
                delivery addresses.
              </p>
            </div>
          </div>

          {/* ADD / EDIT ADDRESS */}
          <div
            className="relative rounded-xl border border-white/10 bg-white/[0.02]"
          >
            <button
              type="button"
              onClick={handleToggleForm}
              aria-expanded={
                isFormOpen
              }
              className="
                flex w-full
                items-center
                justify-between
                px-6 py-4
                text-left
                transition-colors
                hover:bg-white/[0.03]
                cursor-pointer
              "
            >
              <div className="flex items-center gap-3">
                <Plus
                  size={19}
                  className={`
                    text-deep-orange
                    transition-transform
                    duration-200
                    ${
                      isFormOpen
                        ? "rotate-45"
                        : "rotate-0"
                    }
                  `}
                />

                <span className="font-semibold text-white">
                  {editingExternalId
                    ? "Edit address"
                    : "Add new address"}
                </span>
              </div>

              <ChevronDown
                size={18}
                className={`
                  text-gray-400
                  transition-transform
                  duration-200
                  ${
                    isFormOpen
                      ? "rotate-180"
                      : "rotate-0"
                  }
                `}
              />
            </button>

            {isFormOpen && (
              <div className="border-t border-white/10 p-6">
                <div className="grid grid-cols-1 gap-5 md:grid-cols-2">

                  {/* STREET */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      Street{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>

                    <input
                      type="text"
                      value={form.street}
                      onChange={(e) => handleChange("street", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("street")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* NUMBER */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      Number{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.number}
                      onChange={(e) => handleChange("number", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("number")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* COMPLEMENT */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      Complement
                    </label>
                        
                    <input
                      type="text"
                      value={form.complement}
                      onChange={(e) => handleChange("complement", e.target.value)}
                      className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                    />
                  </div>
                        
                  {/* DISTRICT */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      District{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.district}
                      onChange={(e) => handleChange("district", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("district")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* CITY */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      City{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.city}
                      onChange={(e) => handleChange("city", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("city")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* STATE */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      State{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.state}
                      onChange={(e) => handleChange("state", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("state")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* ZIP CODE */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      ZIP code{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.zipCode}
                      onChange={(e) => handleChange("zipCode", e.target.value)}
                      aria-required="true"
                      className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                        ${isFieldInvalid("zipCode")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                        
                  {/* COUNTRY */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      Country{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <CountrySelect
                      value={form.country}
                      onChange={(value) => handleChange("country", value)}
                      invalid={isFieldInvalid("country")}
                    />
                  </div>
                        
                  {/* RECIPIENT NAME */}
                  <div>
                    <label className="mb-2 block text-sm text-gray-400">
                      Recipient name{" "}
                      <span className="text-red-400">
                        *
                      </span>
                    </label>
                        
                    <input
                      type="text"
                      value={form.recipientName}
                      onChange={(e) => handleChange("recipientName", e.target.value)}
                      aria-required="true"
                      placeholder="Who will receive orders at this address?"
                      className={`w-full rounded-lg border bg-charcoal px-4 py-3 text-white outline-none transition placeholder:text-gray-500
                        ${isFieldInvalid("recipientName")
                          ? "border-red-500 focus:border-red-500"
                          : "border-white/10 focus:border-deep-orange"}
                      `}
                    />
                  </div>
                      
                  <div
                    className={`flex items-center justify-between rounded-lg border bg-white/[0.02] px-4 py-3 transition
                      ${showValidation && form.isDefault === null
                          ? "border-red-500"
                          : "border-white/10"
                      }
                    `}
                  >
                    <div>
                      <div className="flex items-center gap-1">
                        <p className="text-sm font-medium text-white">
                          Default address
                        </p>

                        <span className="text-red-500">
                          *
                        </span>
                      </div>

                      <p className="text-xs text-gray-500">
                        Use this address by default for deliveries.
                      </p>
                    </div>

                    <button
                      type="button"
                      role="switch"
                      aria-checked={form.isDefault === true}
                      onClick={() =>
                        setForm((current) => ({
                          ...current,
                          isDefault:
                            current.isDefault === null
                              ? true
                              : !current.isDefault,
                        }))
                      }
                      className={`
                        relative
                        h-6
                        w-11
                        shrink-0
                        rounded-full
                        transition
                        cursor-pointer
                        ${
                          form.isDefault === true
                            ? "bg-deep-orange"
                            : "bg-gray-600"
                        }
                      `}
                    >
                      <span
                        className={`
                          absolute
                          left-1
                          top-1
                          h-4
                          w-4
                          rounded-full
                          bg-white
                          transition-transform
                          duration-200
                          ${
                            form.isDefault === true
                              ? "translate-x-5"
                              : "translate-x-0"
                          }
                        `}
                      />
                    </button>
                  </div>
                        
                  <div className="space-y-3">
                    <label className="text-sm font-medium text-gray-300">
                      Address label
                    </label>
                        
                    <div className="flex gap-2">
                      {["Home", "Work", "Other"].map((option) => (
                        <button
                          key={option}
                          type="button"
                          onClick={() => setForm((current) => ({...current, name: option, })) }
                          className={`rounded-lg border px-4 py-2 text-sm font-medium transition cursor-pointer
                            ${form.name === option
                                ? "border-deep-orange bg-deep-orange/10 text-deep-orange"
                                : "border-white/10 bg-white/[0.02] text-gray-300 hover:bg-white/5"}
                          `}
                        >
                          {option}
                        </button>
                      ))}
                    </div>
                  </div>

                </div>

                <p className="mt-4 text-xs text-gray-500 font-bold">
                  Fields marked with{" "}
                  <span className="text-red-400">
                    *
                  </span>{" "}
                  are required.
                </p>

                <div
                  className={`
                    mt-6
                    flex
                    items-center
                    ${
                      editingExternalId
                        ? "justify-between"
                        : "justify-end"
                    }
                  `}
                >
                  {editingExternalId && (
                    <button
                      type="button"
                      onClick={
                        clearForm
                      }
                      disabled={
                        isSaving
                      }
                      className="
                        rounded-lg
                        px-4 py-2.5
                        text-sm
                        font-medium
                        text-gray-400
                        transition-colors
                        hover:bg-white/5
                        hover:text-white
                        disabled:cursor-not-allowed
                        disabled:opacity-50
                        cursor-pointer
                      "
                    >
                      Cancel edit
                    </button>
                  )}

                  <button
                    type="button"
                    onClick={
                      handleSave
                    }
                    disabled={
                      isSaving
                    }
                    className="
                      flex
                      items-center
                      gap-2
                      rounded-lg
                      bg-deep-orange
                      px-5 py-2.5
                      font-medium
                      text-white
                      transition
                      hover:opacity-90
                      disabled:cursor-not-allowed
                      disabled:opacity-50
                      cursor-pointer
                    "
                  >
                    {editingExternalId ? (
                      <Save
                        size={18}
                      />
                    ) : (
                      <Plus
                        size={18}
                      />
                    )}

                    {isSaving
                      ? "Saving..."
                      : editingExternalId
                        ? "Save changes"
                        : "Add address"}
                  </button>
                </div>
              </div>
            )}
          </div>
        </section>

        {/* SAVED ADDRESSES */}
        <section>
          <h2 className="mb-4 text-lg font-semibold text-white">
            Saved addresses
          </h2>

          {isLoading ? (
            <div className="py-10 text-center text-sm text-gray-400">
              Loading addresses...
            </div>
          ) : addresses.length ===
            0 ? (
            <div
              className="
                rounded-xl
                border border-dashed
                border-white/10
                p-8
                text-center
              "
            >
              <MapPin
                size={30}
                className="mx-auto mb-3 text-gray-500"
              />

              <p className="text-gray-400">
                You don't have any
                saved addresses yet.
              </p>
            </div>
          ) : (
            <div className="space-y-3">
              {addresses.map(
                (address) => (
                  <div
                    key={
                      address.externalId
                    }
                    className="
                      flex
                      items-start
                      justify-between
                      gap-5
                      rounded-xl
                      border
                      border-white/10
                      bg-white/[0.02]
                      p-5
                    "
                  >
                    <div className="min-w-0">
                      <div className="flex items-center gap-2">
                        <MapPin
                          size={18}
                          className="shrink-0 text-deep-orange"
                        />

                        <p className="font-medium text-white">
                          {
                            address.street
                          }
                          ,{" "}
                          {
                            address.number
                          }
                        </p>

                        {address.isDefault && (
                          <span
                            className="
                              rounded-full
                              bg-deep-orange/15
                              px-2 py-0.5
                              text-xs
                              text-deep-orange
                            "
                          >
                            Default
                          </span>
                        )}
                      </div>

                      <div className="mt-2 pl-6 text-sm leading-6 text-gray-400">
                        {address.complement && (
                          <p>
                            {
                              address.complement
                            }
                          </p>
                        )}

                        <p>
                          {
                            address.district
                          }{" "}
                          -{" "}
                          {
                            address.city
                          }
                          /
                          {
                            address.state
                          }
                        </p>

                        <p>
                          {
                            address.zipCode
                          }{" "}
                          -{" "}
                          {getCountryName(
                            address.country
                          )}
                        </p>
                      </div>
                    </div>

                    <div className="flex shrink-0 items-center gap-2">
                      <button
                        type="button"
                        onClick={() =>
                          handleEdit(
                            address
                          )
                        }
                        title="Edit address"
                        className="
                          flex
                          h-10 w-10
                          items-center
                          justify-center
                          rounded-lg
                          border
                          border-white/10
                          text-gray-300
                          transition
                          hover:border-deep-orange
                          hover:text-deep-orange
                          cursor-pointer
                        "
                      >
                        <Edit3
                          size={17}
                        />
                      </button>

                      <button
                        type="button"
                        onClick={() =>
                          handleDelete(
                            address.externalId
                          )
                        }
                        title="Delete address"
                        className="
                          flex
                          h-10 w-10
                          items-center
                          justify-center
                          rounded-lg
                          border
                          border-white/10
                          text-gray-300
                          transition
                          hover:border-red-500/50
                          hover:text-red-400
                          cursor-pointer
                        "
                      >
                        <Trash2
                          size={17}
                        />
                      </button>
                    </div>
                  </div>
                )
              )}
            </div>
          )}
        </section>
      </div>

      {/* ALERT */}
      {alert && (
        <div
          role="alert"
          className={`
            fixed
            bottom-6
            right-6
            z-[9999]
            flex
            min-w-[320px]
            max-w-[420px]
            items-center
            gap-3
            rounded-lg
            border
            px-4
            py-3
            shadow-xl
            ${
              alert.type ===
              "success"
                ? "border-green-500/30 bg-green-950 text-green-400"
                : "border-red-500/30 bg-red-950 text-red-400"
            }
          `}
        >
          {alert.type ===
          "success" ? (
            <CircleCheck
              size={20}
              className="shrink-0"
            />
          ) : (
            <CircleX
              size={20}
              className="shrink-0"
            />
          )}

          <span className="text-sm font-medium">
            {alert.message}
          </span>
        </div>
      )}
    </>
  );
}