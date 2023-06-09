﻿using ComUnity.Frontend.Api;
using System.Text;

namespace ComUnity.Frontend.Extensions;

public static class NotificationExtensions
{
    public static string GetMessage(this Notification notification)
    {
        if (notification.AdditionalProperties is null)
        {
            return notification.Content;
        }

        var stringBuilder = new StringBuilder(notification.Content);
        foreach(KeyValuePair<string, string> entry in notification.AdditionalProperties)
        {
            stringBuilder.Replace($$"""{{{entry.Key}}}""", entry.Value);
        }

        return stringBuilder.ToString();
    }

    public static string GetTokenValue(this Notification notification, string TokenName)
    {
        if (notification.AdditionalProperties is null)
        {
            return string.Empty;
        }
        return notification.AdditionalProperties.GetValueOrDefault(TokenName, string.Empty);
    }
}
