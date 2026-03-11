export default function UserLayout({ children }) {
  return (
    <div className="flex">
      <aside>Menu da conta</aside>
      <main>{children}</main>
    </div>
  );
}