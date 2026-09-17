import Link from "next/link";

type WishlistMenuProps = {
  isAuthenticated: boolean;
};

export default function WishlistMenu({
  isAuthenticated,
}: WishlistMenuProps) {
  if (!isAuthenticated) {
    return (
      <div className="flex h-[220px] w-[290px] flex-col overflow-hidden rounded-xl border border-white/10 bg-charcoal-gray shadow-xl">
        <div className="flex flex-1 items-center justify-center px-5">
          <div className="text-center">
            <p className="font-medium text-white">
              Your wishlist is waiting
            </p>

            <p className="mx-auto mt-2 max-w-[220px] whitespace-normal break-words text-sm text-gray-400">
              Sign in to view and save your favorite items.
            </p>
          </div>
        </div>

        <div className="border-t border-white/10">
          <Link
            href="/wishlist"
            className="flex w-full items-center justify-center px-4 py-3 text-center text-sm transition hover:bg-white/5 hover:!text-deep-orange"
          >
            See all favorites and wishlists
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="flex h-[220px] w-[290px] flex-col overflow-hidden rounded-xl border border-white/10 bg-charcoal-gray shadow-xl">

      {/* Área que terá os últimos produtos */}
      <div className="min-h-0 flex-1 overflow-y-auto">
        {/* Últimos 6 itens da wishlist */}
      </div>

      {/* Rodapé fixo */}
      <div className="shrink-0 border-t border-white/10">
        <Link
          href="/wishlist"
          className="flex w-full items-center justify-center px-4 py-3 text-center text-sm transition hover:bg-white/5 hover:!text-deep-orange"
        >
          See all favorites and wishlists
        </Link>
      </div>

    </div>
  );
}