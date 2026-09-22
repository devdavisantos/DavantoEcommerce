"use client";

import { ChangeEvent, FormEvent, useEffect, useMemo, useRef, useState } from "react";
import { Eye, ImagePlus, PackagePlus, Star, Trash2, X } from "lucide-react";
import ProductOptionDropdown from "./ProductOptionDropdown";

type ProductImage = {
  id: string;
  file: File;
  preview: string;
  isPrimary: boolean;
};

type ProductForm = {
  title: string;
  description: string;
  category: string;
  brand: string;
  condition: string;
  price: string;
  discountPrice: string;
  sku: string;
  stock: string;
};

const categoryOptions = [
  { value: "", label: "Select category" },
  { value: "electronics", label: "Electronics" },
  { value: "fashion", label: "Fashion" },
  { value: "home", label: "Home" },
  { value: "beauty", label: "Beauty" },
  { value: "sports", label: "Sports" },
  { value: "other", label: "Other" }
];

const conditionOptions = [
  { value: "NEW", label: "New" },
  { value: "USED", label: "Used" },
  { value: "REFURBISHED", label: "Refurbished" }
];

const initialForm: ProductForm = {
  title: "",
  description: "",
  category: "",
  brand: "",
  condition: "NEW",
  price: "",
  discountPrice: "",
  sku: "",
  stock: ""
};

export default function NewProductForm() {
  const inputRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState<ProductForm>(initialForm);
  const [images, setImages] = useState<ProductImage[]>([]);
  const [previewOpen, setPreviewOpen] = useState(false);

  const primaryImage = useMemo(
    () => images.find((image) => image.isPrimary) ?? images[0],
    [images]
  );

  const handleChange = (field: keyof ProductForm, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const handleImages = (event: ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files ?? []);

    if (!files.length) return;

    const newImages = files.map((file, index) => ({
      id: crypto.randomUUID(),
      file,
      preview: URL.createObjectURL(file),
      isPrimary: images.length === 0 && index === 0
    }));

    setImages((current) => [...current, ...newImages]);
    event.target.value = "";
  };

  const removeImage = (id: string) => {
    setImages((current) => {
      const removed = current.find((image) => image.id === id);
      if (removed) URL.revokeObjectURL(removed.preview);

      const remaining = current.filter((image) => image.id !== id);

      if (remaining.length && !remaining.some((image) => image.isPrimary))
        remaining[0] = { ...remaining[0], isPrimary: true };

      return remaining;
    });
  };

  const setPrimaryImage = (id: string) => {
    setImages((current) =>
      current.map((image) => ({
        ...image,
        isPrimary: image.id === id
      }))
    );
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    // API + Cloudflare R2 will be connected here after the Catalog model is defined.
  };

  useEffect(() => {
    if (!previewOpen) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        setPreviewOpen(false);
      }
    };

    document.addEventListener("keydown", handleKeyDown);

    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [previewOpen]);

  return (
    <>
      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-semibold text-white">
              Create Product
            </h2>

            <p className="mt-1 text-sm text-gray-500">
              Create a new product listing for your store.
            </p>
          </div>

          <div className="flex items-center gap-3">
            <button
              type="button"
              onClick={() => setPreviewOpen(true)}
              className="inline-flex items-center gap-2 rounded-lg border border-white/10 px-4 py-2.5 text-sm font-semibold text-gray-300 transition hover:bg-white/5 hover:text-white cursor-pointer"
            >
              <Eye size={17} />
              Preview
            </button>

            <button
              type="submit"
              className="inline-flex items-center gap-2 rounded-lg bg-deep-orange px-4 py-2.5 text-sm font-semibold text-white transition hover:opacity-90 cursor-pointer"
            >
              <PackagePlus size={17} />
              Publish Product
            </button>
          </div>
        </div>

        <section className="rounded-xl border border-white/10 p-6">
          <div>
            <h3 className="font-semibold text-white">
              Product Information
            </h3>

            <p className="mt-1 text-sm text-gray-500">
              Basic information customers will see on the product page.
            </p>
          </div>

          <div className="mt-6 space-y-5">
            <div>
              <label htmlFor="title" className="mb-2 block text-sm font-medium text-gray-300">
                Product title
              </label>

              <input
                id="title"
                type="text"
                value={form.title}
                onChange={(e) => handleChange("title", e.target.value)}
                placeholder="Enter product title"
                className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
              />
            </div>

            <div>
              <label htmlFor="description" className="mb-2 block text-sm font-medium text-gray-300">
                Description
              </label>

              <textarea
                id="description"
                value={form.description}
                onChange={(e) => handleChange("description", e.target.value)}
                placeholder="Describe the product, its features and important details..."
                rows={7}
                className="w-full resize-none rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
              />
            </div>

            <div className="grid grid-cols-3 gap-4">
              <div>
                <label htmlFor="category" className="mb-2 block text-sm font-medium text-gray-300">
                  Category
                </label>

                <ProductOptionDropdown
                  id="category"
                  value={form.category}
                  options={categoryOptions}
                  placeholder="Select category"
                  onChange={(value) => handleChange("category", value)}
                />
              </div>

              <div>
                <label htmlFor="brand" className="mb-2 block text-sm font-medium text-gray-300">
                  Brand
                </label>

                <input
                  id="brand"
                  type="text"
                  value={form.brand}
                  onChange={(e) => handleChange("brand", e.target.value)}
                  placeholder="Product brand"
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
                />
              </div>

              <div>
                <label htmlFor="condition" className="mb-2 block text-sm font-medium text-gray-300">
                  Condition
                </label>

                <ProductOptionDropdown
                  id="condition"
                  value={form.condition}
                  options={conditionOptions}
                  onChange={(value) => handleChange("condition", value)}
                />
              </div>
            </div>
          </div>
        </section>

        <section className="rounded-xl border border-white/10 p-6">
          <div>
            <h3 className="font-semibold text-white">
              Product Images
            </h3>

            <p className="mt-1 text-sm text-gray-500">
              Add product images and choose the main image shown to customers.
            </p>
          </div>

          <input
            ref={inputRef}
            type="file"
            accept="image/png,image/jpeg,image/webp"
            multiple
            onChange={handleImages}
            className="hidden"
          />

          <div className="mt-6 grid grid-cols-5 gap-4">
            {images.map((image) => (
              <div key={image.id} className={`group relative aspect-square overflow-hidden rounded-xl border ${image.isPrimary ? "border-deep-orange" : "border-white/10"}`}>
                <img
                  src={image.preview}
                  alt="Product preview"
                  className="h-full w-full object-cover"
                />

                {image.isPrimary && (
                  <span className="absolute left-2 top-2 rounded-md bg-deep-orange px-2 py-1 text-xs font-semibold text-white">
                    Main
                  </span>
                )}

                <div className="absolute inset-x-0 bottom-0 flex justify-end gap-1 bg-black/60 p-2 opacity-0 transition group-hover:opacity-100">
                  {!image.isPrimary && (
                    <button
                      type="button"
                      onClick={() => setPrimaryImage(image.id)}
                      aria-label="Set as main image"
                      className="flex h-8 w-8 items-center justify-center rounded-md bg-white/10 text-white transition hover:bg-deep-orange"
                    >
                      <Star size={15} />
                    </button>
                  )}

                  <button
                    type="button"
                    onClick={() => removeImage(image.id)}
                    aria-label="Remove image"
                    className="flex h-8 w-8 items-center justify-center rounded-md bg-white/10 text-white transition hover:bg-red-500"
                  >
                    <Trash2 size={15} />
                  </button>
                </div>
              </div>
            ))}

            <button
              type="button"
              onClick={() => inputRef.current?.click()}
              className="flex aspect-square flex-col items-center justify-center rounded-xl border border-dashed border-white/15 text-gray-500 transition hover:border-deep-orange hover:bg-deep-orange/5 hover:text-deep-orange cursor-pointer"
            >
              <ImagePlus size={25} />

              <span className="mt-2 text-sm font-medium">
                Add images
              </span>
            </button>
          </div>

          <p className="mt-4 text-xs text-gray-600">
            PNG, JPG and WEBP images are supported.
          </p>
        </section>

        <div className="grid grid-cols-2 gap-6">
          <section className="rounded-xl border border-white/10 p-6">
            <div>
              <h3 className="font-semibold text-white">
                Pricing
              </h3>

              <p className="mt-1 text-sm text-gray-500">
                Set the regular and promotional price.
              </p>
            </div>

            <div className="mt-6 grid grid-cols-2 gap-4">
              <div>
                <label htmlFor="price" className="mb-2 block text-sm font-medium text-gray-300">
                  Price
                </label>

                <input
                  id="price"
                  type="number"
                  min="0"
                  step="0.01"
                  value={form.price}
                  onChange={(e) => handleChange("price", e.target.value)}
                  placeholder="0.00"
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
                />
              </div>

              <div>
                <label htmlFor="discountPrice" className="mb-2 block text-sm font-medium text-gray-300">
                  Discount price
                </label>

                <input
                  id="discountPrice"
                  type="number"
                  min="0"
                  step="0.01"
                  value={form.discountPrice}
                  onChange={(e) => handleChange("discountPrice", e.target.value)}
                  placeholder="Optional"
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
                />
              </div>
            </div>
          </section>

          <section className="rounded-xl border border-white/10 p-6">
            <div>
              <h3 className="font-semibold text-white">
                Inventory
              </h3>

              <p className="mt-1 text-sm text-gray-500">
                Manage the product SKU and available stock.
              </p>
            </div>

            <div className="mt-6 grid grid-cols-2 gap-4">
              <div>
                <label htmlFor="sku" className="mb-2 block text-sm font-medium text-gray-300">
                  SKU
                </label>

                <input
                  id="sku"
                  type="text"
                  value={form.sku}
                  onChange={(e) => handleChange("sku", e.target.value)}
                  placeholder="SKU-001"
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
                />
              </div>

              <div>
                <label htmlFor="stock" className="mb-2 block text-sm font-medium text-gray-300">
                  Stock
                </label>

                <input
                  id="stock"
                  type="number"
                  min="0"
                  value={form.stock}
                  onChange={(e) => handleChange("stock", e.target.value)}
                  placeholder="0"
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-600 focus:border-deep-orange"
                />
              </div>
            </div>
          </section>
        </div>
      </form>

      {previewOpen && (
        <div
          onClick={() => setPreviewOpen(false)}
          className="fixed inset-0 z-[10000] flex items-center justify-center bg-black/70 p-8 backdrop-blur-sm"
        >
          <div
            onClick={(e) => e.stopPropagation()}
            className="max-h-full w-full max-w-6xl overflow-y-auto rounded-2xl border border-white/10 bg-[#131921] shadow-2xl"
          >
            <div className="flex items-center justify-between border-b border-white/10 px-6 py-4">
              <div>
                <p className="font-semibold text-white">
                  Product Preview
                </p>
            
                <p className="mt-0.5 text-xs text-gray-500">
                  This is how your product information will appear to customers.
                </p>
              </div>
            
              <button
                type="button"
                onClick={() => setPreviewOpen(false)}
                aria-label="Close preview"
                className="flex h-9 w-9 cursor-pointer items-center justify-center rounded-lg text-gray-400 transition hover:bg-white/5 hover:text-white"
              >
                <X size={20} />
              </button>
            </div>
            
            <div className="grid grid-cols-2 gap-10 p-8">
              <div>
                <div className="aspect-square overflow-hidden rounded-xl border border-white/10">
                  {primaryImage ? (
                    <img
                      src={primaryImage.preview}
                      alt={form.title || "Product"}
                      className="h-full w-full object-contain"
                    />
                  ) : (
                    <div className="flex h-full items-center justify-center text-gray-600">
                      <ImagePlus size={40} />
                    </div>
                  )}
                </div>
                
                {images.length > 1 && (
                  <div className="mt-4 flex gap-3 overflow-x-auto">
                    {images.map((image) => (
                      <div
                        key={image.id}
                        className={`h-20 w-20 shrink-0 overflow-hidden rounded-lg border ${
                          image.isPrimary
                            ? "border-deep-orange"
                            : "border-white/10"
                        }`}
                      >
                        <img
                          src={image.preview}
                          alt="Product"
                          className="h-full w-full object-cover"
                        />
                      </div>
                    ))}
                  </div>
                )}
              </div>
              
              <div>
                <p className="text-sm text-gray-500">
                  {form.brand || "Brand"}
                </p>
              
                <h2 className="mt-2 text-2xl font-semibold text-white">
                  {form.title || "Product title"}
                </h2>
              
                <div className="mt-6 flex items-end gap-3">
                  {form.discountPrice ? (
                    <>
                      <p className="text-3xl font-bold text-white">
                        {form.discountPrice}
                      </p>
                  
                      <p className="pb-1 text-sm text-gray-500 line-through">
                        {form.price || "0.00"}
                      </p>
                    </>
                  ) : (
                    <p className="text-3xl font-bold text-white">
                      {form.price || "0.00"}
                    </p>
                  )}
                </div>
                
                <div className="my-6 h-px bg-white/10" />
                
                <p className="whitespace-pre-line text-sm leading-7 text-gray-400">
                  {form.description || "Product description will appear here."}
                </p>
                
                <div className="mt-8 grid grid-cols-2 gap-3">
                  <div className="rounded-lg border border-white/10 p-4">
                    <p className="text-xs text-gray-500">
                      Condition
                    </p>
                
                    <p className="mt-1 text-sm font-medium text-white">
                      {conditionOptions.find(
                        (condition) => condition.value === form.condition
                      )?.label}
                    </p>
                  </div>
                    
                  <div className="rounded-lg border border-white/10 p-4">
                    <p className="text-xs text-gray-500">
                      Available stock
                    </p>
                    
                    <p className="mt-1 text-sm font-medium text-white">
                      {form.stock || "0"}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
}