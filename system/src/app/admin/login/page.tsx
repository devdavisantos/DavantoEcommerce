"use client";

import { FormEvent, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAdminAuth } from "@/contexts/AdminAuthContext";

type LoginForm = {
  email: string;
  password: string;
};

type ApiError = {
  code?: string;
  message?: string;
};

export default function AdminLoginPage() {
  const router = useRouter();
  const { isAuthenticated, isLoading: isSessionLoading, refreshSession } = useAdminAuth();

  const [form, setForm] = useState<LoginForm>({
    email: "",
    password: ""
  });

  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!isSessionLoading && isAuthenticated)
      router.replace("/admin");
  }, [isAuthenticated, isSessionLoading, router]);

  function handleChange(field: keyof LoginForm, value: string) {
    setForm((current) => ({
      ...current,
      [field]: value
    }));

    if (error)
      setError("");
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isLoading)
      return;

    setError("");
    setIsLoading(true);

    try {
      const response = await fetch("/api/admin/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email: form.email.trim(),
          password: form.password
        })
      });

      if (!response.ok) {
        const data: ApiError | null = await response
          .json()
          .catch(() => null);

        setError(
          data?.message ?? "Unable to sign in."
        );

        return;
      }

      await refreshSession();

      router.replace("/admin");
      router.refresh();
    } catch {
      setError("Unable to connect to the server.");
    } finally {
      setIsLoading(false);
    }
  }

  if (isSessionLoading || isAuthenticated) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-[#0f1115] text-white">
        <p className="text-sm text-gray-400">
          Loading...
        </p>
      </main>
    );
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-[#0f1115] px-4">
      <div className="w-full max-w-md rounded-xl border border-white/10 bg-[#181b21] p-8 shadow-2xl">
        <div className="mb-8">
          {/* <p className="mb-2 text-sm font-medium uppercase tracking-widest text-deep-orange">
            Lastore
          </p> */}

          <h1 className="text-3xl font-semibold text-white">
            Admin sign in
          </h1>

          <p className="mt-2 text-sm text-gray-400">
            Sign in to access the administration panel.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label htmlFor="email" className="mb-2 block text-sm font-medium text-gray-200">
              Email
            </label>

            <input
              id="email"
              type="email"
              value={form.email}
              onChange={(e) => handleChange("email", e.target.value)}
              autoComplete="username"
              disabled={isLoading}
              className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-500 focus:border-deep-orange disabled:cursor-not-allowed disabled:opacity-60"
              placeholder="Your Email"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-2 block text-sm font-medium text-gray-200">
              Password
            </label>

            <input
              id="password"
              type="password"
              value={form.password}
              onChange={(e) => handleChange("password", e.target.value)}
              autoComplete="current-password"
              disabled={isLoading}
              className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 text-white outline-none transition placeholder:text-gray-500 focus:border-deep-orange disabled:cursor-not-allowed disabled:opacity-60"
              placeholder="Enter your password"
            />
          </div>

          {error && (
            <div
              role="alert"
              className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400"
            >
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={isLoading}
            className="w-full rounded-lg bg-deep-orange px-4 py-3 font-semibold text-white transition hover:opacity-90 disabled:cursor-not-allowed disabled:opacity-60 cursor-pointer"
          >
            {isLoading ? "Signing in..." : "Sign in"}
          </button>
        </form>
      </div>
    </main>
  );
}