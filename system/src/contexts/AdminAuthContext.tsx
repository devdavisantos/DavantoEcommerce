"use client";

import Link from "next/link";
import { createContext, ReactNode, useCallback, useContext, useEffect, useRef, useState } from "react";
import {
  AdminSession,
  ADMIN_REFRESH_BEFORE_MS,
  anonymousAdminSession,
  getAdminSession,
  withAdminSessionLock
} from "@/lib/admin-auth-client";

type AdminAuthContextType = {
  admin: AdminSession["user"];
  isAuthenticated: boolean;
  isLoading: boolean;
  isLoggingOut: boolean;
  accessTokenExpiresAt: string | null;
  refreshTokenExpiresAt: string | null;
  refreshSession: () => Promise<void>;
  clearSessionExpired: () => void;
  logout: () => Promise<void>;
};

const EXPIRATION_KEY = "admin-session-expires-at";

function savedExpiration(): number {
  try {
    return Date.parse(sessionStorage.getItem(EXPIRATION_KEY) ?? "");
  } catch {
    return NaN;
  }
}

function saveExpiration(value: string | null) {
  try {
    if (value)
      sessionStorage.setItem(EXPIRATION_KEY, value);
    else
      sessionStorage.removeItem(EXPIRATION_KEY);
  } catch {
    /* Session persistence itself uses HttpOnly cookies. */
  }
}

const AdminAuthContext = createContext<AdminAuthContextType | undefined>(undefined);

export function AdminAuthProvider({ children }: { children: ReactNode }) {
  const [sessionExpired, setSessionExpired] = useState(false);
  const [session, setSession] = useState<AdminSession>(anonymousAdminSession);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [nextCheck, setNextCheck] = useState(0);

  const loggingOut = useRef(false);
  const generation = useRef(0);
  const channel = useRef<BroadcastChannel | null>(null);

  const clearSessionExpired = useCallback(() => {
    setSessionExpired(false);
  }, []);

  const refreshSession = useCallback(async () => {
    if (loggingOut.current)
      return;

    const current = generation.current;

    try {
      const updated = await getAdminSession();

      if (current !== generation.current)
        return;

      if (updated.authenticated) {
        saveExpiration(updated.refreshTokenExpiresAt);
        setSessionExpired(false);
      } else if (savedExpiration() <= Date.now()) {
        saveExpiration(null);
        setSessionExpired(true);
      }

      setSession(updated);
      setIsLoading(false);

      const now = Date.now();
      const access = Date.parse(updated.accessTokenExpiresAt ?? "");
      const refresh = Date.parse(updated.refreshTokenExpiresAt ?? "");

      const deadline = Math.min(
        Number.isFinite(access) ? access - ADMIN_REFRESH_BEFORE_MS : now + 60_000,
        Number.isFinite(refresh) ? refresh : now + 60_000
      );

      setNextCheck(
        now + Math.max(5_000, Math.min(60_000, deadline - now))
      );
    } catch {
      if (current === generation.current)
        setNextCheck(Date.now() + 15_000);
    }
  }, []);

  const logout = useCallback(async () => {
    if (loggingOut.current)
      return;

    loggingOut.current = true;
    generation.current++;
    setIsLoggingOut(true);

    try {
      await withAdminSessionLock(async () => {
        const response = await fetch("/api/admin/auth/logout", {
          method: "POST", credentials: "same-origin", cache: "no-store"
        });

        if (!response.ok)
          throw new Error("Unable to logout.");
      });

      saveExpiration(null);
      setSessionExpired(false);
      setSession(anonymousAdminSession);
      setIsLoading(false);
      channel.current?.postMessage("logout");
    } catch (error) {
      console.error("[Admin Logout]", error);
    } finally {
      loggingOut.current = false;
      setIsLoggingOut(false);
      setNextCheck(Date.now() + 15_000);
    }
  }, []);

  useEffect(() => {
    void refreshSession();

    const resume = () => {
      if (document.visibilityState === "visible")
        void refreshSession();
    };

    window.addEventListener("focus", resume);
    window.addEventListener("online", resume);
    document.addEventListener("visibilitychange", resume);

    if (typeof BroadcastChannel !== "undefined") {
      const broadcast = new BroadcastChannel("lastore-admin-auth");

      channel.current = broadcast;

      broadcast.onmessage = (event) => {
        if (event.data === "logout") {
          saveExpiration(null);
          setSessionExpired(false);
          setSession(anonymousAdminSession);
          setIsLoading(false);
        }
        generation.current++;
        void refreshSession();
      };
    }

    return () => {
      window.removeEventListener("focus", resume);
      window.removeEventListener("online", resume);
      document.removeEventListener("visibilitychange", resume);
      channel.current?.close();
      channel.current = null;
    };
  }, [refreshSession]);

  useEffect(() => {
    if (!nextCheck)
      return;

    const timer = window.setTimeout(
      () => void refreshSession(),
      Math.max(0, Math.min(60_000, nextCheck - Date.now()))
    );

    return () => window.clearTimeout(timer);
  }, [nextCheck, refreshSession]);

  useEffect(() => {
    const expiresAt = Date.parse(session.refreshTokenExpiresAt ?? "");

    if (!Number.isFinite(expiresAt))
      return;

    let timer: ReturnType<typeof setTimeout>;

    const check = () => {
      const remaining = expiresAt - Date.now();

      if (remaining <= 0) {
        generation.current++;
        saveExpiration(null);
        setSessionExpired(true);
        setSession(anonymousAdminSession);
        setIsLoading(false);
        void refreshSession();
      } else {
        timer = setTimeout(check, Math.min(remaining, 60_000));
      }
    };

    check();

    return () => clearTimeout(timer);
  }, [session.refreshTokenExpiresAt, refreshSession]);

  useEffect(() => {
    if (!sessionExpired)
      return;
  
    const timer = window.setTimeout(() => {
      clearSessionExpired();
    }, 10_000);
  
    return () => window.clearTimeout(timer);
  }, [sessionExpired, clearSessionExpired]);

  return (
    <AdminAuthContext.Provider value={{
      admin: session.user,
      isAuthenticated: session.authenticated,
      isLoading,
      isLoggingOut,
      accessTokenExpiresAt: session.accessTokenExpiresAt,
      refreshTokenExpiresAt: session.refreshTokenExpiresAt,
      refreshSession,
      clearSessionExpired,
      logout
    }}>
      {children}

      {sessionExpired && (
        <div
          role="alert"
          className="fixed bottom-6 right-6 z-50 w-full max-w-sm overflow-hidden rounded-xl border border-white/10 bg-[#17191f] shadow-2xl"
        >
          <div className="p-5">
            <div className="flex items-start gap-4">
              <div className="flex-1">
                <p className="text-base font-semibold text-white">
                  Session expired
                </p>

                <p className="mt-1 text-sm leading-5 text-gray-400">
                  Your admin session has expired. Please sign in again to continue.
                </p>

                <p
                  className="mt-4 inline-flex items-center px-4 py-2 text-sm font-semibold !text-deep-orange transition"
                >
                  Sign in again
                </p>
              </div>

              <button
                type="button"
                onClick={clearSessionExpired}
                className="flex h-7 w-7 items-center justify-center rounded-md text-lg text-gray-400 transition hover:bg-white/10 hover:text-white"
                aria-label="Close"
              >
                ×
              </button>
            </div>
          </div>

          <div className="h-1 w-full bg-white/5">
            <div className="h-full w-full origin-left animate-[session-progress_10s_linear_forwards] bg-deep-orange" />
          </div>
        </div>
      )}
    </AdminAuthContext.Provider>
  );
}

export function useAdminAuth() {
  const context = useContext(AdminAuthContext);

  if (!context)
    throw new Error("useAdminAuth must be used within an AdminAuthProvider.");

  return context;
}
