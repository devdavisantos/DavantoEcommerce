import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { clearAuthCookies } from "@/lib/auth-cookies";

export async function POST() {
  const refreshToken = (await cookies()).get("refresh_token")?.value;
  if (refreshToken) {
    const apiBaseUrl = process.env.API_BASE_URL;
    if (!apiBaseUrl) {
      return NextResponse.json({ message: "Server configuration error." }, { status: 500 });
    }
    try {
      const result = await fetch(`${apiBaseUrl}/api/auth/logout`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken }),
        cache: "no-store",
      });
      if (!result.ok && result.status !== 401) {
        return NextResponse.json({ message: "Unable to logout." }, { status: 503 });
      }
    } catch {
      return NextResponse.json({ message: "Unable to logout." }, { status: 503 });
    }
  }
  return clearAuthCookies(NextResponse.json({ success: true }));
}
