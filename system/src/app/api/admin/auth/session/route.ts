export const dynamic = "force-dynamic";

import { cookies } from "next/headers";
import { NextResponse } from "next/server";

type JwtPayload = {
  exp?: number;
};

function getTokenExpiration(token: string): number | null {
  try {
    const parts = token.split(".");

    if (parts.length !== 3)
      return null;

    const payload = JSON.parse(
      Buffer.from(parts[1], "base64url").toString("utf8")
    ) as JwtPayload;

    return typeof payload.exp === "number" ? payload.exp : null;
  } catch {
    return null;
  }
}

function getTokenExpirationDate(token: string): string | null {
  const expiration = getTokenExpiration(token);

  if (!expiration)
    return null;

  return new Date(expiration * 1000).toISOString();
}

function isAccessTokenExpired(token: string): boolean {
  const expiration = getTokenExpiration(token);

  if (!expiration)
    return true;

  return expiration <= Math.floor(Date.now() / 1000);
}

function canRefreshSession(
  refreshToken: string | undefined,
  refreshTokenExpiresAt: string | null
): boolean {
  if (!refreshToken)
    return false;

  if (!refreshTokenExpiresAt)
    return true;

  const expiresAt = new Date(refreshTokenExpiresAt).getTime();

  if (Number.isNaN(expiresAt))
    return false;

  return expiresAt > Date.now();
}

export async function GET() {
  try {
    const apiBaseUrl = process.env.API_BASE_URL;

    if (!apiBaseUrl) {
      return NextResponse.json(
        {
          authenticated: false,
          canRefresh: false,
          user: null,
          accessTokenExpiresAt: null,
          refreshTokenExpiresAt: null
        },
        { status: 500 }
      );
    }

    const cookieStore = await cookies();

    const accessToken = cookieStore.get("access_token_admin")?.value;
    const refreshToken = cookieStore.get("refresh_token_admin")?.value;
    const refreshTokenExpiresAt =
      cookieStore.get("refresh_token_admin_expires_at")?.value ?? null;

    const canRefresh = canRefreshSession(
      refreshToken,
      refreshTokenExpiresAt
    );

    if (!canRefresh || !accessToken || isAccessTokenExpired(accessToken)) {
      return NextResponse.json(
        {
          authenticated: false,
          canRefresh,
          user: null,
          accessTokenExpiresAt: null,
          refreshTokenExpiresAt
        },
        { status: 200 }
      );
    }

    const profileResponse = await fetch(
      `${apiBaseUrl}/api/admin/profile`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${accessToken}`
        },
        cache: "no-store"
      }
    );

    if (!profileResponse.ok) {
      return NextResponse.json(
        {
          authenticated: false,
          canRefresh,
          user: null,
          accessTokenExpiresAt: null,
          refreshTokenExpiresAt
        },
        {
          status: profileResponse.status === 401 ? 200 : 503
        }
      );
    }

    const user = await profileResponse.json();

    return NextResponse.json(
      {
        authenticated: true,
        canRefresh,
        user,
        accessTokenExpiresAt: getTokenExpirationDate(accessToken),
        refreshTokenExpiresAt
      },
      { status: 200 }
    );
  } catch (error) {
    console.error(
      "[Admin Auth Session] Session check failed:",
      error instanceof Error ? error.message : "Unknown error"
    );

    return NextResponse.json(
      {
        authenticated: false,
        canRefresh: false,
        user: null,
        accessTokenExpiresAt: null,
        refreshTokenExpiresAt: null
      },
      { status: 500 }
    );
  }
}