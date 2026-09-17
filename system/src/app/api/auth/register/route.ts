import { NextResponse } from "next/server";

type RegisterRequest = {
  name: string;
  email: string;
  password: string;
};

export async function POST(request: Request) {
  try {
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

    const body = (await request.json()) as RegisterRequest;

    const response = await fetch(`${apiBaseUrl}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: body.name,
        email: body.email,
        password: body.password,
      }),
      cache: "no-store",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => null);

      return NextResponse.json(
        {
          code: error?.code ?? "RegisterFailed",
          message: error?.message ?? "Unable to create account.",
        },
        {
          status: response.status,
        }
      );
    }

    return NextResponse.json(
      {
        success: true,
      },
      {
        status: 201,
      }
    );
  } catch (error) {
    console.error(
      "[Register] Failed to create account:",
      error instanceof Error ? error.message : "Unknown error"
    );

    return NextResponse.json(
      {
        code: "InternalServerError",
        message: "Unable to create account.",
      },
      {
        status: 500,
      }
    );
  }
}