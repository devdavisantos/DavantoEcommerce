"use client";

import { FormEvent, useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/contexts/AuthContext";

type LoginFormProps = {
  showReturnHome?: boolean;
  onLoginSuccess?: () => void | Promise<void>;
  successMessage?: string;
};

type LoginErrorResponse = {
  code?: string;
  message?: string;
};

export default function LoginForm({
  showReturnHome = false,
  onLoginSuccess,
  successMessage,
}: LoginFormProps) {
  const { refreshSession, clearSessionExpired } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [isSubmitting, setIsSubmitting] =
    useState(false);

  const [errorMessage, setErrorMessage] =
    useState<string | null>(null);

  const errorTimeoutRef =
    useRef<ReturnType<typeof setTimeout> | null>(
      null
    );

  useEffect(() => {
    return () => {
      if (errorTimeoutRef.current) {
        clearTimeout(errorTimeoutRef.current);
      }
    };
  }, []);

  const showError = (message: string) => {
    setErrorMessage(message);

    if (errorTimeoutRef.current) {
      clearTimeout(errorTimeoutRef.current);
    }

    errorTimeoutRef.current = setTimeout(() => {
      setErrorMessage(null);
    }, 5000);
  };

  const router = useRouter();

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (isSubmitting) {
      return;
    }

    setErrorMessage(null);
    setIsSubmitting(true);

    try {
      const response = await fetch(
        "/api/auth/login",
        {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
          },

          body: JSON.stringify({
            email,
            password,
          }),
        }
      );

      if (!response.ok) {
        let error: LoginErrorResponse | null = null;

        try {
          error =
            (await response.json()) as LoginErrorResponse;
        } catch {
          error = null;
        }

        showError(
          error?.message ??
            "Unable to sign in."
        );

        return;
      }

      setPassword("");
      await refreshSession();
      clearSessionExpired();

      if (onLoginSuccess) {
        await onLoginSuccess();
        return;
      }

      router.replace("/");
      router.refresh();
      } catch {
        setErrorMessage("Unable to sign in.");
      } finally {
        setIsSubmitting(false);
      }
  };

  return (
    <div className="w-full">
      <h1 className="mb-6 text-center text-2xl font-bold text-white">
        Sign In
      </h1>

      <form
        className="space-y-4"
        onSubmit={handleSubmit}
      >
        {successMessage && (
          <div
            role="status"
            className="rounded-md border border-green-500/30 bg-green-500/10 px-4 py-3 text-center text-sm text-green-400"
          >
            {successMessage}
          </div>
        )}

        <input
          type="email"
          name="email"
          autoComplete="email"
          placeholder="Email"
          value={email}
          disabled={isSubmitting}
          onChange={(event) =>
            setEmail(event.target.value)
          }
          className="w-full rounded-md border-2 border-white bg-gray-300 px-4 py-2 text-black outline-none ring-1 focus:border-white focus:ring-white disabled:cursor-not-allowed disabled:opacity-70"
        />

        <input
          type="password"
          name="password"
          autoComplete="current-password"
          placeholder="Password"
          value={password}
          disabled={isSubmitting}
          onChange={(event) =>
            setPassword(event.target.value)
          }
          className="w-full rounded-md border-2 border-white bg-gray-300 px-4 py-2 text-black outline-none ring-1 focus:border-white focus:ring-white disabled:cursor-not-allowed disabled:opacity-70"
        />

        {errorMessage && (
          <div
            role="alert"
            className="rounded-md border border-red-500/40 bg-red-500/10 px-3 py-2 text-center text-sm text-red-400"
          >
            {errorMessage}
          </div>
        )}

        <button
          type="submit"
          disabled={isSubmitting}
          className="flex h-10 w-full cursor-pointer items-center justify-center rounded-md bg-amber-primary font-semibold hover:bg-amber-secondary disabled:cursor-not-allowed disabled:opacity-80"
        >
          {isSubmitting ? (
            <span
              className="h-5 w-5 animate-spin rounded-full border-2 border-white/40 border-t-white"
              aria-label="Signing in"
            />
          ) : (
            "Sign In"
          )}
        </button>
      </form>

      <div className="mt-4 text-center text-sm">
        <Link
          href="/forgotpass"
          className="!text-deep-orange hover:!text-amber-500"
        >
          Forgot your password?
        </Link>
      </div>

      <div className="mt-2 text-center text-sm text-gray-300">
        Don&apos;t have an account?{" "}
        <Link
          href="/register"
          className="!text-deep-orange hover:!text-amber-500"
        >
          Create one
        </Link>
      </div>

      {showReturnHome && (
        <div className="mt-2 text-center text-sm !text-gray-400">
          <Link
            href="/"
            className="inline-flex items-center justify-center hover:!text-white"
          >
            Return to Home
          </Link>
        </div>
      )}
    </div>
  );
}
