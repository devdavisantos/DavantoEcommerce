import { cookies } from "next/headers";
import { NextResponse } from "next/server";

type BackendRefreshResponse = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

type BackendErrorResponse = {
  code?: string;
  message?: string;
};

export async function POST() {
  try {
    const cookieStore = await cookies();
    const refreshToken = cookieStore.get("refresh_token")?.value;

    if (!refreshToken) {
      console.error("[Auth Refresh] Refresh token cookie not found.");

      return NextResponse.json(
        {
          code: "Customer.RefreshToken.NotFound",
          message: "Refresh token not found.",
        },
        { status: 401 }
      );
    }

    const apiBaseUrl = process.env.API_BASE_URL;

    if (!apiBaseUrl) {
      console.error("[Auth Refresh] API_BASE_URL is not configured.");

      return NextResponse.json(
        { message: "Unable to refresh session." },
        { status: 500 }
      );
    }

    const apiResponse = await fetch(`${apiBaseUrl}/api/auth/refresh`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        refreshToken,
      }),
      cache: "no-store",
    });

    console.log("[Auth Refresh] Backend status:", apiResponse.status);

    if (!apiResponse.ok) {
      let error: BackendErrorResponse | null = null;

      try {
        error = (await apiResponse.json()) as BackendErrorResponse;
      } catch {
        error = null;
      }

      console.error("[Auth Refresh] Backend rejected refresh:", {
        status: apiResponse.status,
        code: error?.code ?? null,
        message: error?.message ?? null,
      });

      return NextResponse.json(
        {
          code: error?.code,
          message: error?.message ?? "Unable to refresh session.",
        },
        { status: apiResponse.status }
      );
    }

    const refreshResult =
      (await apiResponse.json()) as BackendRefreshResponse;

    console.log("[Auth Refresh] Backend refresh succeeded:", {
      hasAccessToken: Boolean(refreshResult.accessToken),
      hasRefreshToken: Boolean(refreshResult.refreshToken),
      accessTokenExpiresAt: refreshResult.accessTokenExpiresAt,
      refreshTokenExpiresAt: refreshResult.refreshTokenExpiresAt,
    });

    if (
      !refreshResult.accessToken ||
      !refreshResult.refreshToken ||
      !refreshResult.accessTokenExpiresAt ||
      !refreshResult.refreshTokenExpiresAt
    ) {
      console.error(
        "[Auth Refresh] Backend response is missing required fields."
      );

      return NextResponse.json(
        { message: "Unable to refresh session." },
        { status: 500 }
      );
    }

    const accessTokenExpiresAt = new Date(
      refreshResult.accessTokenExpiresAt
    );

    const refreshTokenExpiresAt = new Date(
      refreshResult.refreshTokenExpiresAt
    );

    console.log("[Auth Refresh] Parsed expiration dates:", {
      accessTokenExpiresAtValid:
        !Number.isNaN(accessTokenExpiresAt.getTime()),
      refreshTokenExpiresAtValid:
        !Number.isNaN(refreshTokenExpiresAt.getTime()),
    });

    if (
      Number.isNaN(accessTokenExpiresAt.getTime()) ||
      Number.isNaN(refreshTokenExpiresAt.getTime())
    ) {
      console.error(
        "[Auth Refresh] Backend returned invalid expiration dates."
      );

      return NextResponse.json(
        { message: "Unable to refresh session." },
        { status: 500 }
      );
    }

    const response = NextResponse.json(
      {
        success: true,
        accessTokenExpiresAt: refreshResult.accessTokenExpiresAt,
        refreshTokenExpiresAt: refreshResult.refreshTokenExpiresAt,
      },
      { status: 200 }
    );

    response.cookies.set("access_token", refreshResult.accessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/api",
      expires: accessTokenExpiresAt,
    });

    console.log("[Auth Refresh] Access cookie prepared.");

    response.cookies.set("refresh_token", refreshResult.refreshToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "strict",
      path: "/api/auth",
      expires: refreshTokenExpiresAt,
    });

    console.log("[Auth Refresh] Refresh cookie prepared.");

    response.cookies.set(
      "refresh_token_expires_at",
      refreshResult.refreshTokenExpiresAt,
      {
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        sameSite: "strict",
        path: "/api/auth",
        expires: refreshTokenExpiresAt,
      }
    );

    console.log("[Auth Refresh] Refresh expiration cookie prepared.");
    console.log("[Auth Refresh] Refresh completed successfully.");

    return response;
  } catch (error) {
    console.error(
      "[Auth Refresh] Unexpected failure:",
      error instanceof Error
        ? {
            name: error.name,
            message: error.message,
            stack: error.stack,
          }
        : error
    );

    return NextResponse.json(
      { message: "Unable to refresh session." },
      { status: 500 }
    );
  }
}