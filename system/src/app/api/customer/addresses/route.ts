import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

const getApiBaseUrl = () => {
  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }

  return apiBaseUrl;
};

const getAccessToken = async () => {
  const cookieStore = await cookies();

  return cookieStore.get("access_token")?.value;
};

export async function GET() {
  try {
    const accessToken = await getAccessToken();

    if (!accessToken) {
      return NextResponse.json(
        {
          code: "Unauthorized",
          message: "Authentication required.",
        },
        {
          status: 401,
        }
      );
    }

    const response = await fetch(
      `${getApiBaseUrl()}/api/customer/addresses`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
        cache: "no-store",
      }
    );

    if (!response.ok) {
      const error = await response
        .json()
        .catch(() => null);

      return NextResponse.json(
        error ?? {
          code: "AddressLoadFailed",
          message: "Unable to load addresses.",
        },
        {
          status: response.status,
        }
      );
    }

    const data = await response
      .json()
      .catch(() => []);

    const addresses = Array.isArray(data)
      ? data.map((address) => ({
          ...address,
          zipCode: address.postalCode ?? "",
        }))
      : [];

    return NextResponse.json(
      addresses,
      {
        status: 200,
      }
    );
  } catch {
    return NextResponse.json(
      {
        code: "InternalServerError",
        message: "Unable to load addresses.",
      },
      {
        status: 500,
      }
    );
  }
}

export async function POST(
  request: NextRequest
) {
  try {
    const accessToken = await getAccessToken();

    if (!accessToken) {
      return NextResponse.json(
        {
          code: "Unauthorized",
          message: "Authentication required.",
        },
        {
          status: 401,
        }
      );
    }

    const body = await request.json();

    const payload = {
      name: typeof body.name === "string" && body.name.trim()
        ? body.name.trim()
        : null,

      recipientName: typeof body.recipientName === "string"
        ? body.recipientName.trim()
        : "",

      street: typeof body.street === "string"
        ? body.street.trim()
        : "",

      number: typeof body.number === "string"
        ? body.number.trim()
        : "",

      complement: typeof body.complement === "string" && body.complement.trim()
        ? body.complement.trim()
        : null,

      district: typeof body.district === "string"
        ? body.district.trim()
        : "",

      city: typeof body.city === "string"
        ? body.city.trim()
        : "",

      state: typeof body.state === "string"
        ? body.state.trim()
        : "",

      postalCode: typeof body.zipCode === "string"
        ? body.zipCode.trim()
        : "",

      country: typeof body.country === "string"
        ? body.country.trim().toUpperCase()
        : "",

      isDefault: body.isDefault === true,
    };

    const response = await fetch(
      `${getApiBaseUrl()}/api/customer/addresses`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
        cache: "no-store",
      }
    );

    if (response.status === 204) {
      return new NextResponse(null, {
        status: 204,
      });
    }

    const data = await response
      .json()
      .catch(() => null);

    if (!response.ok) {
      return NextResponse.json(
        data ?? {
          code: "AddressCreateFailed",
          message: "Unable to create address.",
        },
        {
          status: response.status,
        }
      );
    }

    return NextResponse.json(
      data ?? {
        success: true,
      },
      {
        status: response.status,
      }
    );
  } catch {
    return NextResponse.json(
      {
        code: "InternalServerError",
        message: "Unable to create address.",
      },
      {
        status: 500,
      }
    );
  }
}