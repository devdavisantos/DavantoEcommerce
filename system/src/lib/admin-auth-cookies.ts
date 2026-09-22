import { NextResponse } from "next/server";

export function clearAdminAuthCookies(response: NextResponse) {
  for (const name of [
    "access_token_admin",
    "refresh_token_admin",
    "refresh_token_admin_expires_at"
  ]) {
    response.cookies.set(name, "", {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: name === "access_token_admin" ? "lax" : "strict",
      path: name === "access_token_admin" ? "/api" : "/api/admin/auth",
      maxAge: 0
    });
  }

  response.headers.set("Cache-Control", "no-store");

  return response;
}
export type AdminTokens = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

export function setAdminAuthCookies(response: NextResponse, tokens: AdminTokens) {
  const access = new Date(tokens.accessTokenExpiresAt);
  const refresh = new Date(tokens.refreshTokenExpiresAt);
  if (!tokens.accessToken || !tokens.refreshToken ||
      !Number.isFinite(access.getTime()) || !Number.isFinite(refresh.getTime()) ||
      access.getTime() <= Date.now() || refresh.getTime() <= Date.now()) {
    throw new Error("Invalid admin token response.");
  }
  const base = { httpOnly: true, secure: process.env.NODE_ENV === "production" };
  response.cookies.set("access_token_admin", tokens.accessToken, {
    ...base, sameSite: "lax", path: "/api",
    expires: new Date(Math.min(access.getTime(), refresh.getTime())),
  });
  response.cookies.set("refresh_token_admin", tokens.refreshToken, {
    ...base, sameSite: "strict", path: "/api/admin/auth", expires: refresh,
  });
  response.cookies.set("refresh_token_admin_expires_at", refresh.toISOString(), {
    ...base, sameSite: "strict", path: "/api/admin/auth", expires: refresh,
  });
  response.headers.set("Cache-Control", "no-store");
  return response;
}
