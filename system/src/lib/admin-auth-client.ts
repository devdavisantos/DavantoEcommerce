export type AdminAuthUser = {
  externalId: string;
  name: string;
  email: string;
};

export type AdminSession = {
  authenticated: boolean;
  canRefresh: boolean;
  user: AdminAuthUser | null;
  accessTokenExpiresAt: string | null;
  refreshTokenExpiresAt: string | null;
};

export const ADMIN_REFRESH_BEFORE_MS = 50_000;

export const anonymousAdminSession: AdminSession = {
  authenticated: false, canRefresh: false, user: null,
  accessTokenExpiresAt: null, refreshTokenExpiresAt: null
};

let pending: Promise<AdminSession> | null = null;

export async function withAdminSessionLock<T>(
  action: () => Promise<T>
): Promise<T> {
  if (typeof navigator !== "undefined" && navigator.locks)
    return navigator.locks.request("admin-session", action);

  return action();
}

async function readAdminSession(): Promise<AdminSession> {
  const response = await fetch("/api/admin/auth/session", {
    credentials: "same-origin", cache: "no-store"
  });

  if (!response.ok)
    throw new Error("Unable to check admin session.");

  return response.json();
}

function isFresh(session: AdminSession): boolean {
  return session.authenticated &&
    Date.parse(session.accessTokenExpiresAt ?? "") >
      Date.now() + ADMIN_REFRESH_BEFORE_MS;
}

export function getAdminSession(
  forceRefresh = false
): Promise<AdminSession> {
  if (pending)
    return pending;

  pending = withAdminSessionLock(async () => {
    let session = await readAdminSession();

    if (
      session.refreshTokenExpiresAt &&
      Date.parse(session.refreshTokenExpiresAt) <= Date.now()
    )
      return anonymousAdminSession;

    if (!forceRefresh && isFresh(session))
      return session;

    if (!session.canRefresh)
      return session.authenticated ? session : anonymousAdminSession;

    for (let attempt = 0; attempt < 2; attempt++) {
      const response = await fetch("/api/admin/auth/refresh", {
        method: "POST", credentials: "same-origin", cache: "no-store"
      });

      if (response.ok)
        return readAdminSession();

      const error = await response.json().catch(() => null);

      if (
        error?.code === "Admin.RefreshToken.ReplacedToken" ||
        error?.code === "Admin.RefreshToken.InvalidToken"
      ) {
        // In-place rotation removes the old hash. A competing tab may have
        // already updated the cookies before this request reached the API.
        await new Promise(resolve => setTimeout(resolve, 300));

        session = await readAdminSession();

        if (isFresh(session))
          return session;

        if (attempt === 1)
          return anonymousAdminSession;
        continue;
      }

      if (response.status === 401)
        return anonymousAdminSession;

      throw new Error("Unable to renew admin session.");
    }

    throw new Error("Admin session renewal is busy.");
  }).finally(() => {
    pending = null;
  });

  return pending;
}

export async function adminAuthenticatedFetch(
  input: string,
  init?: RequestInit
): Promise<Response> {
  await getAdminSession();

  const response = await fetch(input, init);

  if (response.status !== 401)
    return response;

  const session = await getAdminSession(true);

  return session.authenticated
    ? fetch(input, init)
    : response;
}
