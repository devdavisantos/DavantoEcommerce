"use client";

import Link from "next/link";
import { createContext, ReactNode, useCallback, useContext, useEffect, useRef, useState } from "react";
import { anonymousSession, getSession, REFRESH_BEFORE_MS, Session, withSessionLock } from "@/lib/auth-client";
import { Clock3 } from "@/components/icons/icons";

type AuthContextType = {
  user: Session["user"];
  isAuthenticated: boolean;
  isLoading: boolean;
  isLoggingOut: boolean;
  accessTokenExpiresAt: string | null;
  refreshTokenExpiresAt: string | null;
  refreshSession: () => Promise<void>;
  clearSessionExpired: () => void;
  logout: () => Promise<void>;
};

const EXPIRATION_KEY = "lastore-session-expires-at";

// Only expiry metadata is stored here, never credentials or tokens.
function savedExpiration(): number {
  try { return Date.parse(sessionStorage.getItem(EXPIRATION_KEY) ?? ""); }
  catch { return NaN; }
}
function saveExpiration(value: string | null) {
  try {
    if (value) sessionStorage.setItem(EXPIRATION_KEY, value);
    else sessionStorage.removeItem(EXPIRATION_KEY);
  } catch { /* Session persistence itself uses HttpOnly cookies. */ }
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [sessionExpired, setSessionExpired] = useState(false);
  const [session, setSession] = useState<Session>(anonymousSession);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [nextCheck, setNextCheck] = useState(0);
  const loggingOut = useRef(false);
  const generation = useRef(0);
  const channel = useRef<BroadcastChannel | null>(null);

  const refreshSession = useCallback(async () => {
    if (loggingOut.current) return;
    const current = generation.current;
    try {
      const updated = await getSession();
      if (current !== generation.current) return;
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
        Number.isFinite(access) ? access - REFRESH_BEFORE_MS : now + 60_000,
        Number.isFinite(refresh) ? refresh : now + 60_000,
      );
      // Bounded timers also support sessions longer than the browser's 24.8-day limit.
      setNextCheck(now + Math.max(5_000, Math.min(60_000, deadline - now)));
    } catch {
      // Keep the last confirmed session; a network outage is not a logout.
      if (current === generation.current) setNextCheck(Date.now() + 15_000);
    }
  }, []);

  const logout = useCallback(async () => {
    if (loggingOut.current) return;
    loggingOut.current = true;
    generation.current++;
    setIsLoggingOut(true);
    try {
      await withSessionLock(async () => {
        const response = await fetch("/api/auth/logout", {
          method: "POST", credentials: "same-origin", cache: "no-store",
        });
        if (!response.ok) throw new Error("Unable to logout.");
      });
      saveExpiration(null);
      setSessionExpired(false);
      setSession(anonymousSession);
      setIsLoading(false);
      channel.current?.postMessage("logout");
    } catch (error) {
      console.error("[Logout]", error);
    } finally {
      loggingOut.current = false;
      setIsLoggingOut(false);
      setNextCheck(Date.now() + 15_000);
    }
  }, []);

  useEffect(() => {
    void refreshSession();
    const resume = () => {
      if (document.visibilityState === "visible") void refreshSession();
    };
    window.addEventListener("focus", resume);
    window.addEventListener("online", resume);
    document.addEventListener("visibilitychange", resume);
    if (typeof BroadcastChannel !== "undefined") {
      const broadcast = new BroadcastChannel("lastore-auth");
      channel.current = broadcast;
      broadcast.onmessage = (event) => {
        if (event.data === "logout") {
          saveExpiration(null);
          setSessionExpired(false);
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
    if (!nextCheck) return;
    const timer = window.setTimeout(() => void refreshSession(),
      Math.max(0, Math.min(60_000, nextCheck - Date.now())));
    return () => window.clearTimeout(timer);
  }, [nextCheck, refreshSession]);

  useEffect(() => {
    const expiresAt = Date.parse(session.refreshTokenExpiresAt ?? "");
    if (!Number.isFinite(expiresAt)) return;
    let timer: ReturnType<typeof setTimeout>;
    const check = () => {
      const remaining = expiresAt - Date.now();
      if (remaining <= 0) {
        generation.current++;
        saveExpiration(null);
        setSessionExpired(true);
        setSession(anonymousSession);
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
    if (!sessionExpired) return;

    const timer = setTimeout(() => {
      setSessionExpired(false);
    }, 10000);

    return () => clearTimeout(timer);
  }, [sessionExpired]);

  const clearSessionExpired = useCallback(() => {
    setSessionExpired(false);
  }, []);

  return (
    <AuthContext.Provider value={{
      user: session.user, isAuthenticated: session.authenticated,
      isLoading, isLoggingOut, accessTokenExpiresAt: session.accessTokenExpiresAt,
      refreshTokenExpiresAt: session.refreshTokenExpiresAt, refreshSession, clearSessionExpired, logout,
    }}>
      {sessionExpired && (
        <div role="alert" className="fixed right-6 top-6 z-[10000] w-full max-w-md overflow-hidden rounded-xl border border-white/10 bg-[#131921] shadow-2xl">
          <div className="flex items-start gap-4 p-5">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-deep-orange/10 text-deep-orange">
              <Clock3 size={20} />
            </div>

            <div className="min-w-0 flex-1">
              <p className="font-semibold text-white">Session expired.</p>
              <p className="mt-1 text-sm leading-5 text-gray-400">Your session has expired. Sign in again to continue where you left off.</p>

              <Link
                href="/login"
                onClick={clearSessionExpired}
                className="mt-4 inline-flex rounded-lg bg-deep-orange px-4 py-2 text-sm font-semibold text-white transition hover:opacity-90">
                  Sign in again
              </Link>
            </div>

            <button
              type="button"
              aria-label="Close notice"
              onClick={clearSessionExpired}
              className="text-xl leading-none text-gray-500 transition hover:text-white"
            >
              ×
            </button>
          </div>
        </div>
      )}
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) throw new Error("useAuth must be used within an AuthProvider.");
  
  return context;
}





