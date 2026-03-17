import Header from "@/components/layout/Header";
import Footer from "@/components/layout/Footer";
import BackToTop from "@/components/layout/BackToTop";

export default function StoreLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <Header />
        <main className="pt-20 min-h-screen">
          {children}
        </main>
      <Footer />
      <BackToTop/>
    </>
  );
}