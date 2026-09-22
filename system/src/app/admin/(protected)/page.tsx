import {
  ArrowDownRight,
  ArrowUpRight,
  CircleDollarSign,
  Package,
  ShoppingBag,
  UsersRound
} from "lucide-react";
import Link from "next/link";

const stats = [
  {
    title: "Total Sales",
    value: "$24,780.00",
    change: "+12.5%",
    positive: true,
    icon: CircleDollarSign
  },
  {
    title: "Total Orders",
    value: "1,248",
    change: "+8.2%",
    positive: true,
    icon: ShoppingBag
  },
  {
    title: "Customers",
    value: "3,642",
    change: "+5.7%",
    positive: true,
    icon: UsersRound
  },
  {
    title: "Products",
    value: "486",
    change: "-1.4%",
    positive: false,
    icon: Package
  }
];

const recentOrders = [
  { order: "#LS-1048", customer: "John Carter", total: "$249.90", status: "Processing" },
  { order: "#LS-1047", customer: "Michael Scott", total: "$89.50", status: "Delivered" },
  { order: "#LS-1046", customer: "Sarah Miller", total: "$415.00", status: "Pending" },
  { order: "#LS-1045", customer: "Robert Davis", total: "$179.90", status: "Shipped" }
];

const orderStatus = [
  { label: "Pending", value: 18 },
  { label: "Processing", value: 32 },
  { label: "Shipped", value: 24 },
  { label: "Delivered", value: 174 }
];

function getStatusClass(status: string) {
  switch (status) {
    case "Delivered":
      return "bg-green-500/10 text-green-400";
    case "Shipped":
      return "bg-blue-500/10 text-blue-400";
    case "Processing":
      return "bg-deep-orange/10 text-deep-orange";
    default:
      return "bg-yellow-500/10 text-yellow-400";
  }
}

export default function AdminDashboardPage() {
  return (
    <div className="space-y-8">
      <section>
        <div>
          <h2 className="text-lg font-semibold text-white">
            Overview
          </h2>
          <p className="mt-1 text-sm text-gray-500">
            Here&apos;s an overview of your store performance.
          </p>
        </div>

        <div className="mt-5 grid grid-cols-4 gap-5">
          {stats.map((stat) => {
            const Icon = stat.icon;
            const ChangeIcon = stat.positive ? ArrowUpRight : ArrowDownRight;

            return (
              <div key={stat.title} className="rounded-xl border border-white/10 p-5">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-sm text-gray-500">
                      {stat.title}
                    </p>

                    <p className="mt-2 text-2xl font-semibold text-white">
                      {stat.value}
                    </p>
                  </div>

                  <div className="flex h-11 w-11 items-center justify-center rounded-lg bg-deep-orange/10 text-deep-orange">
                    <Icon size={21} />
                  </div>
                </div>

                <div className="mt-5 flex items-center gap-2 text-xs">
                  <span className={`flex items-center gap-1 font-medium ${stat.positive ? "text-green-400" : "text-red-400"}`}>
                    <ChangeIcon size={14} />
                    {stat.change}
                  </span>

                  <span className="text-gray-600">
                    from last month
                  </span>
                </div>
              </div>
            );
          })}
        </div>
      </section>

      <section className="grid grid-cols-[minmax(0,2fr)_minmax(280px,1fr)] gap-5">
        <div className="rounded-xl border border-white/10 p-5">
          <div>
            <h3 className="font-semibold text-white">
              Sales Overview
            </h3>
            <p className="mt-1 text-sm text-gray-500">
              Sales performance over the last 7 days.
            </p>
          </div>

          <div className="mt-8 flex h-64 items-end gap-4">
            {[42, 68, 53, 82, 61, 92, 74].map((height, index) => (
              <div key={index} className="flex h-full flex-1 flex-col justify-end gap-3">
                <div className="relative flex-1">
                  <div
                    className="absolute bottom-0 w-full rounded-t-lg bg-deep-orange/80 transition hover:bg-deep-orange"
                    style={{ height: `${height}%` }}
                  />
                </div>

                <span className="text-center text-xs text-gray-500">
                  {["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"][index]}
                </span>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-xl border border-white/10 p-5">
          <div>
            <h3 className="font-semibold text-white">
              Order Status
            </h3>
            <p className="mt-1 text-sm text-gray-500">
              Current order distribution.
            </p>
          </div>

          <div className="mt-6 space-y-3">
            {orderStatus.map((item) => (
              <div key={item.label} className="flex items-center justify-between rounded-lg border border-white/5 px-4 py-3">
                <div className="flex items-center gap-3">
                  <span className="h-2 w-2 rounded-full bg-deep-orange" />
                  <span className="text-sm text-gray-400">
                    {item.label}
                  </span>
                </div>

                <span className="text-sm font-semibold text-white">
                  {item.value}
                </span>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="rounded-xl border border-white/10">
        <div className="flex items-center justify-between border-b border-white/10 p-5">
          <div>
            <h3 className="font-semibold text-white">
              Recent Orders
            </h3>
            <p className="mt-1 text-sm text-gray-500">
              Latest orders received by your store.
            </p>
          </div>

          <Link href="/admin/orders" className="text-sm font-medium !text-deep-orange transition hover:opacity-80">
            View all orders
          </Link>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="border-b border-white/10 text-xs uppercase tracking-wide text-gray-500">
                <th className="px-5 py-4 font-medium">Order</th>
                <th className="px-5 py-4 font-medium">Customer</th>
                <th className="px-5 py-4 font-medium">Total</th>
                <th className="px-5 py-4 font-medium">Status</th>
              </tr>
            </thead>

            <tbody>
              {recentOrders.map((order) => (
                <tr key={order.order} className="border-b border-white/5 last:border-0">
                  <td className="px-5 py-4 text-sm font-medium text-white">
                    {order.order}
                  </td>
                  <td className="px-5 py-4 text-sm text-gray-400">
                    {order.customer}
                  </td>
                  <td className="px-5 py-4 text-sm text-gray-300">
                    {order.total}
                  </td>
                  <td className="px-5 py-4">
                    <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${getStatusClass(order.status)}`}>
                      {order.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}