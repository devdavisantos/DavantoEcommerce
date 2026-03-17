export default function UserLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-screen items-center justify-center">
      <aside>Menu da conta</aside>
      <main>{children}</main>
    </div>
  );
}