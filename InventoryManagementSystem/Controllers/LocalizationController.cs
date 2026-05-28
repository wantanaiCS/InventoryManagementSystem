using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace InventoryManagementSystem.Controllers;

public class LocalizationController : Controller
{
    private readonly IStringLocalizer<SharedResource> _l;

    public LocalizationController(IStringLocalizer<SharedResource> localizer)
    {
        _l = localizer;
    }

    [HttpGet]
    public IActionResult Ping()
    {
        var title = _l["App.Title"].Value;
        return Content($"culture={CultureInfo.CurrentUICulture.Name}; App.Title={title}");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(culture))
            culture = "th-TH";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Dashboard");
    }
}

