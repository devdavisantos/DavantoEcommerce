import { NextRequest, NextResponse } from "next/server";

const API_BASE_URL = process.env.API_BASE_URL;

export async function GET(request: NextRequest) {
  try {
    const accessToken = request.cookies.get("access_token_admin")?.value;

    if (!accessToken) {
      return NextResponse.json(
        {
          code: "Admin.Unauthorized",
          message: "Unauthorized."
        },
        { status: 401 }
      );
    }

    const response = await fetch(`${API_BASE_URL}/api/admin/profile`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${accessToken}`
      },
      cache: "no-store"
    });

    if (!response.ok) {
      return NextResponse.json(
        {
          code: "Admin.Unauthorized",
          message: "Unauthorized."
        },
        { status: response.status }
      );
    }

    const profile = await response.json();

    return NextResponse.json(profile, {
      status: 200
    });
  } catch {
    return NextResponse.json(
      {
        code: "Admin.ProfileFailed",
        message: "Unable to load the admin profile."
      },
      { status: 500 }
    );
  }
}