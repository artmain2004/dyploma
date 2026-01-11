namespace NotificationService.Templates;

public static class EmailTemplates
{
    public static string ConfirmEmailTemplate(string? firstName, string confirmUrl)
    {
        var safeName = string.IsNullOrWhiteSpace(firstName) ? "tam" : System.Net.WebUtility.HtmlEncode(firstName);
        var safeUrl = System.Net.WebUtility.HtmlEncode(confirmUrl);
        return $@"
<!doctype html>
<html lang=""pl"">
<body style=""margin:0;background:#f5f7fb;font-family:Arial,Helvetica,sans-serif;color:#0f172a;"">
  <div style=""max-width:640px;margin:0 auto;padding:32px 20px;"">
    <div style=""background:#0f172a;color:#ffffff;padding:18px 24px;border-radius:16px 16px 0 0;"">
      <div style=""font-size:12px;opacity:0.8;margin-top:4px;"">Potwierdzenie konta</div>
    </div>
    <div style=""background:#ffffff;border-radius:0 0 16px 16px;padding:28px 24px;box-shadow:0 12px 32px rgba(15,23,42,0.08);"">
      <h2 style=""margin:0 0 10px 0;font-size:22px;"">Potwierdz email</h2>
      <p style=""margin:0 0 18px 0;font-size:14px;line-height:1.6;color:#475569;"">Czesc {safeName}, kliknij przycisk ponizej, aby aktywowac konto i potwierdzic adres email.</p>
      <a href=""{safeUrl}"" style=""display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:10px;font-weight:600;font-size:14px;"">
        Potwierdz email
      </a>
      <div style=""margin-top:18px;font-size:12px;color:#94a3b8;"">Jesli to nie Ty prosiles o zalozenie konta, zignoruj ta wiadomosc.</div>
    </div>
  </div>
</body>
</html>";
    }

    public static string ResetPasswordTemplate(string? firstName, string resetUrl)
    {
        var safeName = string.IsNullOrWhiteSpace(firstName) ? "tam" : System.Net.WebUtility.HtmlEncode(firstName);
        var safeUrl = System.Net.WebUtility.HtmlEncode(resetUrl);
        return $@"
<!doctype html>
<html lang=""pl"">
<body style=""margin:0;background:#f5f7fb;font-family:Arial,Helvetica,sans-serif;color:#0f172a;"">
  <div style=""max-width:640px;margin:0 auto;padding:32px 20px;"">
    <div style=""background:#0f172a;color:#ffffff;padding:18px 24px;border-radius:16px 16px 0 0;"">
      <div style=""font-size:12px;opacity:0.8;margin-top:4px;"">Reset hasla</div>
    </div>
    <div style=""background:#ffffff;border-radius:0 0 16px 16px;padding:28px 24px;box-shadow:0 12px 32px rgba(15,23,42,0.08);"">
      <h2 style=""margin:0 0 10px 0;font-size:22px;"">Ustaw nowe haslo</h2>
      <p style=""margin:0 0 18px 0;font-size:14px;line-height:1.6;color:#475569;"">Czesc {safeName}, kliknij przycisk ponizej, aby zresetowac haslo.</p>
      <a href=""{safeUrl}"" style=""display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:10px;font-weight:600;font-size:14px;"">
        Resetuj haslo
      </a>
      <div style=""margin-top:18px;font-size:12px;color:#94a3b8;"">Jesli nie prosiles o zmiane hasla, zignoruj ta wiadomosc.</div>
    </div>
  </div>
</body>
</html>";
    }

    public static string OrderCreatedTemplate(string? customerName, string orderNumber, decimal totalPrice, List<string> items)
    {
        var safeName = string.IsNullOrWhiteSpace(customerName) ? "Klient" : System.Net.WebUtility.HtmlEncode(customerName);
        var safeOrderNumber = System.Net.WebUtility.HtmlEncode(orderNumber);
        var listItems = string.Join("", items.Select(i => $"<li>{System.Net.WebUtility.HtmlEncode(i)}</li>"));
        return $@"
<!doctype html>
<html lang=""pl"">
<body style=""margin:0;background:#f5f7fb;font-family:Arial,Helvetica,sans-serif;color:#0f172a;"">
  <div style=""max-width:640px;margin:0 auto;padding:32px 20px;"">
    <div style=""background:#0f172a;color:#ffffff;padding:18px 24px;border-radius:16px 16px 0 0;"">
      <div style=""font-size:12px;opacity:0.8;margin-top:4px;"">Potwierdzenie zamowienia</div>
    </div>
    <div style=""background:#ffffff;border-radius:0 0 16px 16px;padding:28px 24px;box-shadow:0 12px 32px rgba(15,23,42,0.08);"">
      <h2 style=""margin:0 0 10px 0;font-size:22px;"">Dziekujemy za zakup</h2>
      <p style=""margin:0 0 18px 0;font-size:14px;line-height:1.6;color:#475569;"">Czesc {safeName}, Twoje zamowienie zostalo utworzone. Ponizej znajdziesz podsumowanie.</p>
      <div style=""display:flex;gap:16px;flex-wrap:wrap;margin:16px 0;padding:14px;border-radius:12px;background:#f8fafc;"">
        <div style=""min-width:200px;""><div style=""font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:0.6px;"">Numer zamowienia</div><div style=""font-size:14px;font-weight:700;"">{safeOrderNumber}</div></div>
        <div style=""min-width:200px;""><div style=""font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:0.6px;"">Suma</div><div style=""font-size:14px;font-weight:700;"">{totalPrice:F2}</div></div>
      </div>
      <div style=""font-size:12px;color:#94a3b8;text-transform:uppercase;letter-spacing:0.6px;margin-bottom:8px;"">Produkty</div>
      <ul style=""padding-left:18px;margin:0;color:#475569;font-size:14px;line-height:1.6;"">{listItems}</ul>
      <div style=""margin-top:18px;font-size:12px;color:#94a3b8;"">Jesli masz pytania, odpowiedz na tego maila.</div>
    </div>
  </div>
</body>
</html>";
    }
}
