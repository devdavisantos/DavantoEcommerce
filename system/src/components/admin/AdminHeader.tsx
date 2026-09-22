"use client";

import { Bell, BellOff } from "lucide-react";
import { usePathname } from "next/navigation";
import { useAdminAuth } from "@/contexts/AdminAuthContext";
import Link from "next/link";

function getPageTitle(pathname: string) {
  if (pathname === "/admin") return "Dashboard";
  if (pathname === "/admin/orders") return "Sales / Orders";
  if (pathname === "/admin/products") return "Products";
  if (pathname === "/admin/products/new") return "New Product";
  if (pathname.startsWith("/admin/products/")) return "Edit Product";
  if (pathname === "/admin/customers") return "Customers";
  if (pathname === "/admin/customers/blocked") return "Blocked Customers";
  if (pathname.startsWith("/admin/customers/")) return "Customer Details";
  if (pathname === "/admin/reports") return "Reports";
  if (pathname === "/admin/account") return "My Account";

  return "Control Panel";
}

export default function AdminHeader() {
  const pathname = usePathname();
  const { admin } = useAdminAuth();

  const title = getPageTitle(pathname);

  return (
    <header className="flex h-20 shrink-0 items-center justify-between border-b border-white/10 px-8">
      <div>
        <h1 className="text-xl font-semibold text-white">
          {title}
        </h1>
      </div>

      <div className="flex items-center gap-5">
        <div className="group relative">
          <button
            type="button"
            aria-label="Notifications"
            className="relative flex h-10 w-10 items-center justify-center rounded-lg text-gray-400 transition hover:bg-white/5 hover:text-white cursor-pointer"
          >
            <Bell size={20} />
          </button>

          <div className="invisible absolute right-0 top-full z-50 w-96 pt-2 opacity-0 transition duration-150 group-hover:visible group-hover:opacity-100">
            <div className="overflow-hidden rounded-xl border border-white/10 bg-neutral-950 shadow-2xl">
              <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
                <div>
                  <p className="font-semibold text-white text-sm">
                    Notifications
                  </p>
                  <p className="mt-0.5 text-xs text-gray-500">
                    Latest activity from your store.
                  </p>
                </div>
              </div>

              <div className="flex min-h-48 flex-col items-center justify-center px-6 py-8 text-center">
                <div className="flex h-11 w-11 items-center justify-center rounded-full bg-white/5 text-gray-500">
                  <BellOff size={20} />
                </div>

                <p className="mt-3 text-sm font-medium text-white">
                  No notifications
                </p>

                <p className="mt-1 max-w-64 text-xs leading-5 text-gray-500">
                  You&apos;re all caught up. New store activity will appear here.
                </p>
              </div>
            </div>
          </div>
        </div>

        <div className="h-8 w-px bg-white/10" />

        <Link
          href="/admin/account"
          className="group flex items-center gap-3 rounded-lg px-2 py-1.5 transition hover:bg-white/5"
        >
          <div className="flex h-10 w-10 items-center justify-center rounded-full bg-deep-orange/10 font-semibold text-deep-orange transition group-hover:bg-deep-orange group-hover:text-white">
            {admin?.name?.charAt(0).toUpperCase() ?? "A"}
          </div>

          <div className="min-w-0">
            <p className="max-w-40 truncate text-sm font-semibold text-white">
              {admin?.name ?? "Admin"}
            </p>
            <p className="text-xs text-gray-500 transition group-hover:text-gray-400">
              Administrator
            </p>
          </div>
        </Link>

      </div>
    </header>
  );
}