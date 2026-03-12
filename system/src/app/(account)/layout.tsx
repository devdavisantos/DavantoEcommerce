export default function UserLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex">
      <aside>Menu da conta</aside>
      <main>{children}</main>
    </div>
  );
}