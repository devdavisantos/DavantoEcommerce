import type { PagedResult, ProductListItem, ProductStatus } from "@/types/product";

type GetProductsParams = {
    page?: number;
    pageSize?: number;
    search?: string;
    status?: ProductStatus;
};

export async function getAdminProducts({
    page = 1,
    pageSize = 20,
    search,
    status
}: GetProductsParams = {}): Promise<PagedResult<ProductListItem>> {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString()
    });

    if (search) params.set("search", search);
    if (status !== undefined) params.set("status", status.toString());

    const response = await fetch(`/api/admin/products?${params.toString()}`, {
        credentials: "same-origin",
        cache: "no-store"
    });

    if (!response.ok)
        throw new Error("Failed to load products.");

    return response.json();
}