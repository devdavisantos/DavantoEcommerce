import type { ProductListItem } from "@/types/product";

type AdminProductCardProps = {
    product: ProductListItem;
};

export default function AdminProductCard({ product }: AdminProductCardProps) {
    const price = product.discountPrice ?? product.price;

    return (
        <div className="flex items-center gap-4 rounded-xl border border-white/10 bg-charcoal-gray p-4 transition hover:border-white/20">
            <div className="flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-white/5">
                {product.primaryImageKey ? (
                    <img
                        src={product.primaryImageKey}
                        alt={product.title}
                        className="h-full w-full object-cover"
                    />
                ) : (
                    <span className="text-xs text-gray-500">
                        No image
                    </span>
                )}
            </div>

            <div className="min-w-0 flex-1">
                <div className="flex items-start justify-between gap-4">
                    <div className="min-w-0">
                        <h3 className="truncate font-medium text-white">
                            {product.title}
                        </h3>

                        <p className="mt-1 text-xs text-gray-500">
                            SKU: {product.sku}
                        </p>
                    </div>

                    <span className="shrink-0 rounded-md bg-white/5 px-2 py-1 text-xs text-gray-400">
                        {product.status}
                    </span>
                </div>

                <div className="mt-4 flex items-end justify-between gap-4">
                    <div className="flex gap-4 text-sm text-gray-400">
                        {product.brand && <span>{product.brand}</span>}
                        {product.category && <span>{product.category}</span>}

                        <span>
                            Stock: {product.stock}
                        </span>
                    </div>

                    <div className="text-right">
                        {product.discountPrice !== null && (
                            <p className="text-xs text-gray-500 line-through">
                                ${product.price.toFixed(2)}
                            </p>
                        )}

                        <p className="font-semibold text-deep-orange">
                            ${price.toFixed(2)}
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}