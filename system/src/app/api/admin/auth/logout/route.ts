import { NextRequest, NextResponse } from "next/server";
import { clearAdminAuthCookies } from "@/lib/admin-auth-cookies";

export async function POST(request: NextRequest) {
  const refreshToken = request.cookies.get("refresh_token_admin")?.value;
  if (refreshToken) {
    const apiBaseUrl = process.env.API_BASE_URL;
    if (!apiBaseUrl) return NextResponse.json({ message: "Server configuration error." }, { status: 500 });
    try {
      const response = await fetch(apiBaseUrl + "/api/admin/auth/logout", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken }),
        cache: "no-store",
      });
      if (!response.ok && response.status !== 401) {
        return NextResponse.json({ message: "Unable to logout." }, { status: 503 });
      }
    } catch {
      return NextResponse.json({ message: "Unable to logout." }, { status: 503 });
    }
  }
  return clearAdminAuthCookies(NextResponse.json({ success: true }));
}
