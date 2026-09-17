"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { UserRound,  MapPin,  Package,  Heart} from "@/components/icons/icons";

const menuItems = [
  {
    label: "Profile",
    href: "/profile",
    icon: UserRound,
  },
  {
    label: "Address",
    href: "/address",
    icon: MapPin,
  },
  {
    label: "Orders",
    href: "/orders",
    icon: Package,
  },
  {
    label: "Wishlist",
    href: "/wishlist",
    icon: Heart,
  },
];

export default function AccountSidebar() {
  const pathname = usePathname();

  return (
    <aside className="w-[250px] shrink-0 border-r border-white/10 bg-charcoal-gray">
      <nav className="flex flex-col py-4">
        {menuItems.map((item) => {
          const Icon = item.icon;

          const isActive = pathname === item.href;

          return (
            <Link
              key={item.href}
              href={item.href}
              className={`
                flex items-center gap-3
                px-5 py-4
                text-sm
                transition-colors
                ${
                  isActive
                    ? "bg-gray-500 text-deep-orange"
                    : "text-white hover:bg-white/15 hover:!text-deep-orange"
                }
              `}
            >
              <Icon size={20} />

              <span className="font-medium">
                {item.label}
              </span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}