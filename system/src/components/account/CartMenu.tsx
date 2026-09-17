import Link from "next/link";

type CartMenuProps = {
  isAuthenticated: boolean;
};

export default function CartMenu({
  isAuthenticated,
}: CartMenuProps) {
  return (
    <div className="flex h-[220px] w-[290px] flex-col overflow-hidden rounded-xl border border-white/10 bg-charcoal-gray shadow-xl">
      <div className="min-h-0 flex-1">
        {isAuthenticated ? (
          <div className="flex h-full items-center justify-center px-5">
            <p className="text-sm text-gray-400">
              Cart items will appear here.
            </p>
          </div>
        ) : (
          <div className="flex h-full items-center justify-center px-5">
            <div className="text-center">
              <p className="font-medium text-white">
                Your cart is waiting
              </p>

              <p className="mx-auto mt-2 max-w-[220px] whitespace-normal break-words text-sm text-gray-400">
                Sign in to view and manage the items in your cart.
              </p>
            </div>
          </div>
        )}
      </div>

      <div className="shrink-0 border-t border-white/10">
        <Link
          href="/cart"
          className="flex w-full items-center justify-center px-4 py-3 text-center text-sm transition hover:bg-white/5 hover:!text-deep-orange"
        >
          View cart
        </Link>
      </div>
    </div>
  );
}