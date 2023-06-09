﻿using System.Net.Mail;
using System.Net;

namespace ComUnity.Application.Infrastructure.Settings;

public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Account { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}