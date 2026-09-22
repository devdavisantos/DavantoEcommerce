export type AuthUser = { name?: string; email?: string; [key: string]: unknown };
export type Session = {
  authenticated: boolean;
  canRefresh: boolean;
  user: AuthUser | null;
  accessTokenExpiresAt: string | null;
  refreshTokenExpiresAt: string | null;
};

export const REFRESH_BEFORE_MS = 50_000;
export const anonymousSession: Session = {
  authenticated: false, canRefresh: false, user: null,
  accessTokenExpiresAt: null, refreshTokenExpiresAt: null,
};

let pending: Promise<Session> | null = null;

export async function withSessionLock<T>(action: () => Promise<T>): Promise<T> {
  if (typeof navigator !== "undefined" && navigator.locks) {
    return navigator.locks.request("customer-session", action);
  }
  return action();
}

async function readSession(): Promise<Session> {
  const response = await fetch("/api/auth/session", {
    credentials: "same-origin", cache: "no-store",
  });
  if (!response.ok) throw new Error("Unable to check session.");
  return response.json();
}

function isFresh(session: Session): boolean {
  return session.authenticated &&
    Date.parse(session.accessTokenExpiresAt ?? "") > Date.now() + REFRESH_BEFORE_MS;
}

// Re-read cookies after acquiring the cross-tab lock: another tab may have rotated them.
export function getSession(forceRefresh = false): Promise<Session> {
  if (pending) return pending;
  pending = withSessionLock(async () => {
    let session = await readSession();
    if (session.refreshTokenExpiresAt &&
        Date.parse(session.refreshTokenExpiresAt) <= Date.now()) return anonymousSession;
    if (!forceRefresh && isFresh(session)) return session;
    if (!session.canRefresh) return session.authenticated ? session : anonymousSession;

    for (let attempt = 0; attempt < 2; attempt++) {
      const response = await fetch("/api/auth/refresh", {
        method: "POST", credentials: "same-origin", cache: "no-store",
      });
      if (response.ok) return readSession();
      const error = await response.json().catch(() => null);
      if (error?.code === "Customer.RefreshToken.ReplacedToken") {
        // Fallback for browsers without Web Locks.
        await new Promise(resolve => setTimeout(resolve, 300));
        session = await readSession();
        if (!forceRefresh && isFresh(session)) return session;
        continue;
      }
      if (response.status === 401) return anonymousSession;
      throw new Error("Unable to renew session.");
    }
    throw new Error("Session renewal is busy.");
  }).finally(() => { pending = null; });
  return pending;
}

// Only retry a rejected (401) request, never a mutation with an ambiguous network result.
export async function authenticatedFetch(input: string, init?: RequestInit): Promise<Response> {
  await getSession();
  const response = await fetch(input, init);
  if (response.status !== 401) return response;
  const session = await getSession(true);
  return session.authenticated ? fetch(input, init) : response;
}

