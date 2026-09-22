import { ReactNode } from "react";
import { AdminAuthProvider } from "@/contexts/AdminAuthContext";

export default function AdminLayout({ children }: { children: ReactNode }) {
  return (
    <AdminAuthProvider>
      {children}
    </AdminAuthProvider>
  );
}