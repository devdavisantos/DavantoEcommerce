export type ProductStatus = "Draft" | "Active" | "Inactive" | "Discontinued";

export type ProductCondition = "New" | "Used" | "Refurbished";

export type ProductListItem = {
    externalId: string;
    title: string;
    sku: string | null;
    price: number;
    discountPrice: number | null;
    stock: number;
    condition: ProductCondition;
    status: ProductStatus;
    category: string | null;
    brand: string | null;
    primaryImageUrl: string | null;
    createdAt: string;
};

export type PagedResult<T> = {
    items: T[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
};