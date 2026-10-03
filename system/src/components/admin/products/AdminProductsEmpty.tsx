import Link from "next/link";

export default function AdminProductsEmpty() {
    return (
        <div className="mt-8 flex min-h-[300px] flex-col items-center justify-center rounded-xl border border-dashed border-white/10 bg-white/[0.02] px-6 text-center">
            <h3 className="font-medium text-white">
                No products yet
            </h3>

            <p className="mt-2 max-w-sm text-sm text-gray-500">
                You haven't created any products yet. Create your first product to start selling.
            </p>

            <Link
                href="/admin/products/new"
                className="mt-5 rounded-lg bg-deep-orange px-4 py-2 text-sm font-medium text-white transition hover:opacity-90"
            >
                Create product
            </Link>
        </div>
    );
}