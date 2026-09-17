import { clearAuthCookies } from "@/lib/auth-cookies";
import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

export async function PUT(request: NextRequest) {
  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    return NextResponse.json(
      {
        code: "ConfigurationError",
        message: "Server configuration error.",
      },
      { status: 500 }
    );
  }

  const cookieStore = await cookies();
  const accessToken = cookieStore.get("access_token")?.value;

  if (!accessToken) {
    return NextResponse.json(
      {
        code: "Unauthorized",
        message: "Authentication required.",
      },
      { status: 401 }
    );
  }

  const body = await request.json();

  const response = await fetch(
    `${apiBaseUrl}/api/customer/change-password`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
      cache: "no-store",
    }
  );

  if (response.status === 204) {
    const nextResponse = new NextResponse(null, {
      status: 204,
    });

    return clearAuthCookies(nextResponse);
  }

  const data = await response.json().catch(() => null);

  return NextResponse.json(data, {
    status: response.status,
  });
}
