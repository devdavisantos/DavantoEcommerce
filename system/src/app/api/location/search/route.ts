import { NextRequest, NextResponse } from "next/server";

const PHOTON_URL = "https://photon.komoot.io/api";

type PhotonFeature = {
  properties?: {
    name?: string;
    street?: string;
    district?: string;
    locality?: string;
    city?: string;
    state?: string;
    statecode?: string;
    postcode?: string;
    country?: string;
    countrycode?: string;
    osm_key?: string;
    osm_value?: string;
  };
};

type PhotonResponse = {
  features?: PhotonFeature[];
};

type LocationResult = {
  street: string;
  district: string;
  city: string;
  state: string;
  stateCode: string;
  postalCode: string;
  country: string;
  countryCode: string;
};

function normalize(value?: string) {
  return value?.trim() ?? "";
}

function normalizePostalCode(value?: string) {
  return normalize(value);
}

function getStreet(properties: PhotonFeature["properties"]) {
  if (!properties) return "";

  const street = normalize(properties.street);

  if (street) return street;

  const name = normalize(properties.name);
  const postcode = normalize(properties.postcode);

  if (!name) return "";

  const normalizedName = name.replace(/\W/g, "").toLowerCase();
  const normalizedPostcode = postcode.replace(/\W/g, "").toLowerCase();

  if (normalizedName && normalizedName === normalizedPostcode) {
    return "";
  }

  return name;
}

function mapLocation(feature: PhotonFeature): LocationResult | null {
  const properties = feature.properties;

  if (!properties) return null;

  const location: LocationResult = {
    street: getStreet(properties),
    district: normalize(properties.district ?? properties.locality),
    city: normalize(properties.city),
    state: normalize(properties.state),
    stateCode: normalize(properties.statecode).toUpperCase(),
    postalCode: normalizePostalCode(properties.postcode),
    country: normalize(properties.country),
    countryCode: normalize(properties.countrycode).toUpperCase(),
  };

  if (
    !location.street &&
    !location.district &&
    !location.city &&
    !location.state &&
    !location.postalCode &&
    !location.country
  ) {
    return null;
  }

  return location;
}

function removeDuplicates(locations: LocationResult[]) {
  const seen = new Set<string>();

  return locations.filter((location) => {
    const key = [
      location.street,
      location.district,
      location.city,
      location.state,
      location.postalCode,
      location.countryCode,
    ]
      .map((value) => value.trim().toLowerCase())
      .join("|");

    if (seen.has(key)) return false;

    seen.add(key);

    return true;
  });
}

export async function GET(request: NextRequest) {
  const query = request.nextUrl.searchParams.get("q")?.trim();

  if (!query || query.length < 3) {
    return NextResponse.json(
      { message: "Enter a valid location." },
      { status: 400 },
    );
  }

  try {
    const params = new URLSearchParams({
      q: query,
      limit: "10",
      lang: "en",
    });

    const response = await fetch(`${PHOTON_URL}?${params.toString()}`, {
      headers: { Accept: "application/json" },
      cache: "no-store",
    });

    if (!response.ok) {
      return NextResponse.json(
        { message: "Unable to search this location." },
        { status: 502 },
      );
    }

    const data = (await response.json()) as PhotonResponse;

    const locations = removeDuplicates(
      (data.features ?? [])
        .map(mapLocation)
        .filter((location): location is LocationResult => location !== null),
    ).slice(0, 5);

    return NextResponse.json(locations);
  } catch {
    return NextResponse.json(
      { message: "Unable to search this location." },
      { status: 500 },
    );
  }
}