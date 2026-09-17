import { loginMetadataHeaders } from "@/lib/login-metadata";
import { NextRequest, NextResponse } from "next/server";

type BackendLoginResponse = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

type BackendErrorResponse = {
  code?: string;
  message?: string;
};

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const email =
      typeof body.email === "string"
        ? body.email.trim()
        : "";

    const password =
      typeof body.password === "string"
        ? body.password
        : "";

    if (!email || !password) {
      return NextResponse.json(
        {
          message: "Email and password are required.",
        },
        {
          status: 400,
        }
      );
    }

    const apiBaseUrl = process.env.API_BASE_URL;

    if (!apiBaseUrl) {
      return NextResponse.json(
        {
          message: "Unable to process login.",
        },
        {
          status: 500,
        }
      );
    }

    const apiResponse = await fetch(
      `${apiBaseUrl}/api/auth/login`,
      {
        method: "POST",

        headers: {
          "Content-Type": "application/json",
          ...loginMetadataHeaders(request),
        },

        body: JSON.stringify({
          email,
          password,
        }),

        cache: "no-store",
      }
    );

    if (!apiResponse.ok) {
      let error: BackendErrorResponse | null = null;

      try {
        error =
          (await apiResponse.json()) as BackendErrorResponse;
      } catch {
        error = null;
      }

      return NextResponse.json(
        {
          code: error?.code,
          message:
            error?.message ??
            "Unable to sign in.",
        },
        {
          status: apiResponse.status,
        }
      );
    }

    const loginResult =
      (await apiResponse.json()) as BackendLoginResponse;

    const response = NextResponse.json(
      {
        success: true,
      },
      {
        status: 200,
      }
    );

    const accessTokenExpiresAt = new Date(
      loginResult.accessTokenExpiresAt
    );

    const refreshTokenExpiresAt = new Date(
      loginResult.refreshTokenExpiresAt
    );

    if ( Number.isNaN(accessTokenExpiresAt.getTime()) || Number.isNaN(refreshTokenExpiresAt.getTime()) ) {
      return NextResponse.json(
        {
          message: "Unable to process login.",
        },
        {
          status: 500,
        }
      );
    }

    response.cookies.set("access_token", loginResult.accessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/api",
      expires: accessTokenExpiresAt,
    });
    
    response.cookies.set("refresh_token", loginResult.refreshToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "strict",
      path: "/api/auth",
      expires: refreshTokenExpiresAt,
    });

    response.cookies.set("refresh_token_expires_at", loginResult.refreshTokenExpiresAt, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "strict",
      path: "/api/auth",
      expires: refreshTokenExpiresAt,
    });

    return response;

  } catch {
    return NextResponse.json(
      {
        message: "Unable to sign in.",
      },
      {
        status: 500,
      }
    );
  }
}
