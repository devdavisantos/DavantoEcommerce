"use client";

import { useCallback, useEffect, useState } from "react";

import AdminProductCard from "@/components/admin/products/AdminProductCard";
import AdminProductsEmpty from "@/components/admin/products/AdminProductsEmpty";
import { getAdminProducts } from "@/services/admin/product";
import type { PagedResult, ProductListItem } from "@/types/product";

export default function AdminProductsPage() {
    const [products, setProducts] = useState<PagedResult<ProductListItem> | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const loadProducts = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);

            const result = await getAdminProducts({
                page: 1,
                pageSize: 20
            });

            setProducts(result);
        } catch {
            setError("Unable to load products.");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadProducts();
    }, [loadProducts]);

    return (
        <div>
            <div>
                <h2 className="text-lg font-semibold text-white">
                    Products
                </h2>

                <p className="mt-1 text-sm text-gray-500">
                    Manage your store products.
                </p>
            </div>

            {loading ? (
                <div className="mt-8 flex min-h-[250px] items-center justify-center">
                    <p className="text-sm text-gray-500">
                        Loading products...
                    </p>
                </div>
            ) : error ? (
                <div className="mt-8 flex min-h-[250px] flex-col items-center justify-center rounded-xl border border-white/10">
                    <p className="text-sm text-gray-400">
                        {error}
                    </p>

                    <button
                        type="button"
                        onClick={loadProducts}
                        className="mt-4 rounded-lg bg-white/5 px-4 py-2 text-sm text-white transition hover:bg-white/10"
                    >
                        Try again
                    </button>
                </div>
            ) : !products || products.items.length === 0 ? (
                <AdminProductsEmpty />
            ) : (
                <div className="mt-8 space-y-3">
                    {products.items.map((product) => (
                        <AdminProductCard
                            key={product.externalId}
                            product={product}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}