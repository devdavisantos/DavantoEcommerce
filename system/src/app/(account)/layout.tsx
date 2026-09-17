import AccountSidebar from "@/components/account/AccountSidebar";
import ProtectedRoute from "@/components/auth/ProtectedRoute";
import Header from "@/components/layout/Header";
import Footer from "@/components/layout/Footer";

export default function UserLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ProtectedRoute>
      <div className="min-h-screen bg-deep-dark">
        <Header fixed={false} />

        <main className="px-6 py-10">
          <div
            className="mx-auto flex h-[calc(100vh-10rem)] max-w-7xl overflow-hidden rounded-2xl border border-white/10 bg-charcoal-dark shadow-xl"
          >
            <AccountSidebar />

            <section className="account-scroll min-w-0 flex-1 overflow-y-auto p-8">
              {children}
            </section>
          </div>
        </main>
      </div>

      <Footer />
    </ProtectedRoute>
  );
}