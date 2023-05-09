﻿namespace ComUnity.Application.Features.Authentication.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; }
}
