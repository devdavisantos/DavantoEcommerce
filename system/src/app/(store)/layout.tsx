import Header from "@/components/layout/Header";
import Footer from "@/components/layout/Footer";
import BackToTop from "@/components/ui/BackToTop";

export default function StoreLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <Header />
        <main className="pt-20 min-h-screen bg-deep-dark">
            {children}
        </main>
      <Footer />
      <BackToTop/>
    </>
  );
}