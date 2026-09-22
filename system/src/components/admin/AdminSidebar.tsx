"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";
import {
  ChevronDown,
  CircleUserRound,
  ClipboardList,
  LayoutDashboard,
  LogOut,
  Package,
  ShoppingCart,
  UsersRound
} from "lucide-react";
import { useAdminAuth } from "@/contexts/AdminAuthContext";

export default function AdminSidebar() {
  const pathname = usePathname();
  const { logout } = useAdminAuth();

  const [productsOpen, setProductsOpen] = useState(pathname.startsWith("/admin/products"));
  const [customersOpen, setCustomersOpen] = useState(pathname.startsWith("/admin/customers"));

  const isDashboard = pathname === "/admin";
  const isOrders = pathname.startsWith("/admin/orders");
  const isProducts = pathname.startsWith("/admin/products");
  const isCustomers = pathname.startsWith("/admin/customers");
  const isReports = pathname.startsWith("/admin/reports");
  const isAccount = pathname.startsWith("/admin/account");

  const itemClass = (active: boolean) =>
    `relative flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition ${
      active
        ? "bg-deep-orange/10 !text-deep-orange"
        : "text-gray-400 hover:bg-white/5 hover:text-white"
    }`;

  const subItemClass = (active: boolean) =>
    `relative block rounded-lg px-3 py-2 text-sm transition ${
      active
        ? "bg-white/5 font-medium !text-deep-orange"
        : "text-gray-500 hover:bg-white/5 hover:text-white"
    }`;

  const handleLogout = async () => {
    await logout();
    window.location.href = "/admin/login";
  };

  return (
    <aside className="flex h-screen w-64 shrink-0 flex-col border-r border-white/10">
      <div className="flex h-20 items-center border-b border-white/10 px-6">
        <div>
          <p className="text-lg font-semibold text-white">
            Control Panel
          </p>
          <p className="text-xs text-gray-500">
            Lastore Administration
          </p>
        </div>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-5">
        <div className="space-y-1">
          <Link href="/admin" className={itemClass(isDashboard)}>
            {isDashboard && (
              <span className="absolute left-0 h-5 w-1 rounded-r-full bg-deep-orange" />
            )}
            <LayoutDashboard size={19} />
            Dashboard
          </Link>

          <Link href="/admin/orders" className={itemClass(isOrders)}>
            {isOrders && (
              <span className="absolute left-0 h-5 w-1 rounded-r-full bg-deep-orange" />
            )}
            <ShoppingCart size={19} />
            Sales / Orders
          </Link>

          <div>
            <button
              type="button"
              onClick={() => setProductsOpen(!productsOpen)}
              className={itemClass(isProducts)}
            >
              {isProducts && (
                <span className="absolute left-0 h-5 w-1 rounded-r-full bg-deep-orange" />
              )}
              <Package size={19} />
              <span className="flex-1 text-left">Products</span>
              <ChevronDown
                size={16}
                className={`transition-transform ${productsOpen ? "rotate-180" : ""}`}
              />
            </button>

            {productsOpen && (
              <div className="ml-7 mt-1 space-y-1 border-l border-white/10 pl-3">
                <Link
                  href="/admin/products"
                  className={subItemClass(pathname === "/admin/products")}
                >
                  All Products
                </Link>

                <Link
                  href="/admin/products/new"
                  className={subItemClass(pathname === "/admin/products/new")}
                >
                  New Product
                </Link>
              </div>
            )}
          </div>

          <div>
            <button
              type="button"
              onClick={() => setCustomersOpen(!customersOpen)}
              className={itemClass(isCustomers)}
            >
              {isCustomers && (
                <span className="absolute left-0 h-5 w-1 rounded-r-full bg-deep-orange" />
              )}
              <UsersRound size={19} />
              <span className="flex-1 text-left">Customers</span>
              <ChevronDown
                size={16}
                className={`transition-transform ${customersOpen ? "rotate-180" : ""}`}
              />
            </button>

            {customersOpen && (
              <div className="ml-7 mt-1 space-y-1 border-l border-white/10 pl-3">
                <Link
                  href="/admin/customers"
                  className={subItemClass(pathname === "/admin/customers")}
                >
                  All Customers
                </Link>

                <Link
                  href="/admin/customers/blocked"
                  className={subItemClass(pathname === "/admin/customers/blocked")}
                >
                  Blocked Customers
                </Link>
              </div>
            )}
          </div>

          <Link href="/admin/reports" className={itemClass(isReports)}>
            {isReports && (
                <span className="absolute left-0 h-5 w-1 rounded-r-full bg-deep-orange" />
            )}
            <ClipboardList size={19} />
            Reports
          </Link>
        </div>
      </nav>

      <div className="border-t border-white/10 p-3">
        <div className="space-y-1">
          <Link href="/admin/account" className={itemClass(isAccount)}>
            <CircleUserRound size={19} />
            My Account
          </Link>

          <button
            type="button"
            onClick={handleLogout}
            className={itemClass(false)}
          >
            <LogOut size={19} />
            Logout
          </button>
        </div>
      </div>
    </aside>
  );
}