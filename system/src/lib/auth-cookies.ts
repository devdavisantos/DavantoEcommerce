import { NextResponse } from "next/server";

export function clearAuthCookies(response: NextResponse) {
  for (const name of ["access_token", "refresh_token", "refresh_token_expires_at"]) {
    response.cookies.set(name, "", {
      httpOnly: true, secure: process.env.NODE_ENV === "production",
      sameSite: name === "access_token" ? "lax" : "strict",
      path: name === "access_token" ? "/api" : "/api/auth", maxAge: 0,
    });
  }
  response.headers.set("Cache-Control", "no-store");
  return response;
}
