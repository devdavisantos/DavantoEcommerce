using System.Net;
using System.Text.Json;

namespace Api.Controllers.Requests.Auth;

public static class LoginDevice
{
    // Descriptive metadata only; never used to authenticate or identify a session.
    public static string Describe(string userAgent)
    {
        var ua = userAgent.Length > 2048 ? userAgent[..2048] : userAgent;
        var browser = ua.Contains("Edg", StringComparison.OrdinalIgnoreCase) ? "Edge"
            : ua.Contains("OPR/") ? "Opera"
            : ua.Contains("Firefox/") || ua.Contains("FxiOS/") ? "Firefox"
            : ua.Contains("Chrome/") || ua.Contains("CriOS/") ? "Chrome"
            : ua.Contains("Safari/") ? "Safari" : "Unknown";
        var os = ua.Contains("Android") ? "Android"
            : ua.Contains("iPhone") || ua.Contains("iPad") ? "iOS"
            : ua.Contains("Windows") ? "Windows"
            : ua.Contains("Macintosh") || ua.Contains("Mac OS X") ? "macOS"
            : ua.Contains("Linux") ? "Linux" : "Unknown";
        var type = ua.Contains("iPad") || (os == "Android" && !ua.Contains("Mobile")) ? "tablet"
            : ua.Contains("Mobile") || ua.Contains("iPhone") ? "mobile"
            : os != "Unknown" ? "desktop" : "unknown";
        return JsonSerializer.Serialize(new { browser, os, type });
    }

    public static string? NormalizeIp(string? value)
    {
        if (!IPAddress.TryParse(value, out var address)) return null;
        return (address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address).ToString();
    }
}
