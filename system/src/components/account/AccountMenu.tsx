"use client";

import Link from "next/link";
import { LogOut, Package, UserRound } from "@/components/icons/icons";
import { useAuth } from "@/contexts/AuthContext";

type AccountMenuProps = {};

export default function AccountMenu() {
  const { refreshSession } = useAuth();
  const { logout, isLoggingOut } = useAuth();

  const handleLogout = async () => {
    try {
      await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "same-origin"
      });
    } finally {
      await refreshSession();
    }
  }

  return (
    <div
      className="w-56 rounded-xl border border-white/10 bg-charcoal-gray shadow-xl overflow-hidden"
    >
      <nav className="flex flex-col py-2">
        <Link
          href="/profile"
          className="flex items-center gap-3 px-4 py-3 transition hover:bg-white/15 hover:!text-deep-orange"
        >
          <UserRound size={18} />
          <span>My profile</span>
        </Link>

        <Link
          href="/orders"
          className="flex items-center gap-3 px-4 py-3 transition hover:bg-white/15 hover:!text-deep-orange"
        >
          <Package size={18} />
          <span>Orders</span>
        </Link>

        <div className="my-1 border-t border-white/10" />

        {/* <button
          type="button"
          onClick={handleLogout}
          className="flex w-full items-center gap-3 px-4 py-3 text-left transition cursor-pointer hover:bg-white/5 hover:text-deep-orange"
        >
          <LogOut size={18} />
          <span>Logout</span>
        </button> */}

        <button
          type="button"
          onClick={logout}
          disabled={isLoggingOut}
          className="flex w-full items-center gap-3 px-4 py-3 text-left transition cursor-pointer hover:bg-white/15 hover:text-deep-orange disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isLoggingOut ? (
            <>
              <span className="h-[18px] w-[18px] animate-spin rounded-full border-2 border-current border-t-transparent" />
              <span>Logging out...</span>
            </>
          ) : (
            <>
              <LogOut size={18} />
              <span>Logout</span>
            </>
          )}
        </button>
      </nav>
    </div>
  );
}