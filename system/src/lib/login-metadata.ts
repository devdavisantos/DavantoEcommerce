import { isIP } from "node:net";

// Enable only for a header overwritten by the trusted ingress proxy.
// NextRequest does not expose the TCP peer address.
export function loginMetadataHeaders(request: Request): Record<string, string> {
  const headers: Record<string, string> = {
    "User-Agent": (request.headers.get("user-agent") ?? "").slice(0, 2048),
    "X-Lastore-Proxy": "1",
  };
  const headerName = process.env.TRUSTED_CLIENT_IP_HEADER;
  if (headerName) {
    const value = request.headers.get(headerName)?.trim();
    // Require a single address. Do not blindly trust the first X-Forwarded-For entry.
    if (value && isIP(value)) headers["X-Lastore-Client-IP"] = value;
  }
  return headers;
}
