import Link from "next/link";

type WishlistItem = {
    id: number;
    name: string;
};

type WishlistMenuProps = {
    isAuthenticated: boolean;
    items?: WishlistItem[];
};

export default function WishlistMenu({ isAuthenticated, items = [] }: WishlistMenuProps) {
    const hasItems = items.length > 0;

    return (
        <div className="flex h-[220px] w-[290px] flex-col overflow-hidden rounded-xl border border-white/10 bg-charcoal-gray shadow-xl">
            <div className="min-h-0 flex flex-1 flex-col overflow-y-auto">
                {!isAuthenticated ? (
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
                ) : !hasItems ? (
                    <div className="flex flex-1 items-center justify-center px-5">
                        <div className="text-center">
                            <p className="font-medium text-white">
                                No saved items yet
                            </p>

                            <p className="mx-auto mt-2 max-w-[220px] whitespace-normal break-words text-sm text-gray-400">
                                Items you save to your wishlist will appear here.
                            </p>
                        </div>
                    </div>
                ) : (
                    <div className="p-3">
                        {items.slice(0, 6).map((item) => (
                            <div key={item.id}>
                                {item.name}
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <div className="shrink-0 border-t border-white/10">
                <Link
                    href={isAuthenticated ? "/wishlist" : "/login"}
                    className="flex w-full items-center justify-center px-4 py-3 text-center text-sm transition hover:bg-white/5 hover:!text-deep-orange"
                >
                    See all favorites and wishlists
                </Link>
            </div>
        </div>
    );
}