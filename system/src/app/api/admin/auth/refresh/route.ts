import { setAdminAuthCookies } from "@/lib/admin-auth-cookies";
import { NextRequest, NextResponse } from "next/server";

const API_BASE_URL = process.env.API_BASE_URL;

type AdminRefreshResponse = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

export async function POST(request: NextRequest) {
  try {
    const refreshToken = request.cookies.get("refresh_token_admin")?.value;

    if (!refreshToken) {
      return NextResponse.json(
        {
          code: "Admin.InvalidRefreshToken",
          message: "Invalid refresh token."
        },
        { status: 401 }
      );
    }

    const response = await fetch(`${API_BASE_URL}/api/admin/auth/refresh`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "User-Agent": request.headers.get("user-agent") ?? ""
      },
      body: JSON.stringify({
        refreshToken
      }),
      cache: "no-store"
    });

    if (!response.ok) {
      const error = await response.json().catch(() => null);
      // Transient failures and concurrent rotation must not delete valid cookies.
      return NextResponse.json({
        code: error?.code,
        message: error?.message ?? "Unable to refresh the admin session.",
      }, { status: response.status, headers: { "Cache-Control": "no-store" } });
    }
    const data: AdminRefreshResponse = await response.json();

    const result = NextResponse.json(
      { success: true },
      { status: 200 }
    );

    return setAdminAuthCookies(result, data);
  } catch {
    return NextResponse.json(
      {
        code: "Admin.RefreshFailed",
        message: "Unable to refresh the session."
      },
      { status: 500 }
    );
  }
}



