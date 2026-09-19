import { NextRequest, NextResponse } from "next/server";

const PHOTON_URL = "https://photon.komoot.io/structured";
const NOMINATIM_URL = "https://nominatim.openstreetmap.org/search";

type PostalCodeResult = {
  street: string;
  district: string;
  city: string;
  state: string;
  postalCode: string;
  countryCode: string;
};

type PhotonFeature = {
  properties?: {
    name?: string;
    street?: string;
    district?: string;
    locality?: string;
    city?: string;
    county?: string;
    state?: string;
    postcode?: string;
    country?: string;
    countrycode?: string;
  };
};

type PhotonResponse = {
  features?: PhotonFeature[];
};

type NominatimAddress = {
  road?: string;
  pedestrian?: string;
  suburb?: string;
  neighbourhood?: string;
  quarter?: string;
  city_district?: string;
  borough?: string;
  city?: string;
  town?: string;
  village?: string;
  municipality?: string;
  state?: string;
  postcode?: string;
  country_code?: string;
};

type NominatimResult = {
  display_name?: string;
  address?: NominatimAddress;
};

export async function GET(request: NextRequest) {
  const postalCode = request.nextUrl.searchParams.get("postalCode")?.trim();
  const countryCode = request.nextUrl.searchParams.get("countryCode")?.trim().toUpperCase();

  if (!postalCode || !countryCode) {
    return NextResponse.json(
      { message: "Postal code and country are required." },
      { status: 400 },
    );
  }

  try {
    const photonResults = await searchPhoton(postalCode, countryCode);

    if (photonResults.length > 0) {
      return NextResponse.json(photonResults);
    }

    const nominatimResults = await searchNominatim(postalCode, countryCode);

    return NextResponse.json(nominatimResults);
  } catch {
    return NextResponse.json(
      { message: "Unable to search this postal code." },
      { status: 500 },
    );
  }
}

async function searchPhoton(postalCode: string, countryCode: string): Promise<PostalCodeResult[]> {
  const params = new URLSearchParams({
    postcode: postalCode,
    countrycode: countryCode,
    limit: "10",
    lang: "en",
  });

  const response = await fetch(`${PHOTON_URL}?${params.toString()}`, {
    headers: { Accept: "application/json" },
    cache: "no-store",
  });

  if (!response.ok) {
    return [];
  }

  const data = (await response.json()) as PhotonResponse;

  return (data.features ?? [])
    .map((feature): PostalCodeResult | null => {
      const properties = feature.properties;

      if (!properties) return null;

      return {
        street: properties.street ?? properties.name ?? "",
        district: properties.district ?? properties.locality ?? "",
        city: properties.city ?? "",
        state: properties.state ?? "",
        postalCode: properties.postcode ?? "",
        countryCode: properties.countrycode?.toUpperCase() ?? "",
      };
    })
    .filter((result): result is PostalCodeResult =>
      result !== null &&
      result.countryCode === countryCode &&
      normalizePostalCode(result.postalCode) === normalizePostalCode(postalCode)
    )
    .filter(isUniqueResult);
}

async function searchNominatim(postalCode: string, countryCode: string): Promise<PostalCodeResult[]> {
  const params = new URLSearchParams({
    postalcode: postalCode,
    countrycodes: countryCode.toLowerCase(),
    format: "jsonv2",
    addressdetails: "1",
    limit: "10",
  });

  const response = await fetch(`${NOMINATIM_URL}?${params.toString()}`, {
    headers: {
      Accept: "application/json",
      "User-Agent": "Lastore/1.0",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    return [];
  }

  const data = (await response.json()) as NominatimResult[];

  return data
    .map((result): PostalCodeResult | null => {
      const address = result.address;

      if (!address) return null;

      return {
        street: address.road ?? address.pedestrian ?? "",
        district:
          address.suburb ??
          address.neighbourhood ??
          address.quarter ??
          address.city_district ??
          address.borough ??
          "",
        city:
          address.city ??
          address.town ??
          address.village ??
          address.municipality ??
          "",
        state: address.state ?? "",
        postalCode: address.postcode ?? "",
        countryCode: address.country_code?.toUpperCase() ?? "",
      };
    })
    .filter((result): result is PostalCodeResult =>
      result !== null &&
      result.countryCode === countryCode &&
      normalizePostalCode(result.postalCode) === normalizePostalCode(postalCode)
    )
    .filter(isUniqueResult);
}

function normalizePostalCode(value: string) {
  return value.replace(/[\s-]/g, "").toUpperCase();
}

function isUniqueResult(
  result: PostalCodeResult,
  index: number,
  results: PostalCodeResult[],
) {
  return index === results.findIndex((item) =>
    item.street === result.street &&
    item.district === result.district &&
    item.city === result.city &&
    item.state === result.state &&
    item.postalCode === result.postalCode
  );
}