import { loginMetadataHeaders } from "@/lib/login-metadata";
import { setAdminAuthCookies } from "@/lib/admin-auth-cookies";
import { NextRequest, NextResponse } from "next/server";

const API_BASE_URL = process.env.API_BASE_URL;

type AdminLoginResponse = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const response = await fetch(`${API_BASE_URL}/api/admin/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...loginMetadataHeaders(request)
      },
      body: JSON.stringify(body),
      cache: "no-store"
    });

    if (!response.ok) {
      const error = await response.json().catch(() => null);

      return NextResponse.json(
        error ?? {
          code: "Admin.LoginFailed",
          message: "Unable to sign in."
        },
        { status: response.status }
      );
    }

    const data: AdminLoginResponse = await response.json();

    const result = NextResponse.json(
      { success: true },
      { status: 200 }
    );

    return setAdminAuthCookies(result, data);
  } catch {
    return NextResponse.json(
      {
        code: "Admin.LoginFailed",
        message: "Unable to sign in."
      },
      { status: 500 }
    );
  }
}

