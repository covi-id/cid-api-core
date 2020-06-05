using System.Collections.Generic;

namespace CoviIDApiCore.V1.Constants
{
    public class UrlConstants
    {
        public static Dictionary<Routes, string> PartialRoutes = new Dictionary<Routes, string>
        {
            { Routes.Agency, "agency/v1/" },
            { Routes.Custodian, "custodian/v1/api/" },
            { Routes.Sendgrid, "v3/mail/send"},
            { Routes.ClickatellSend, "/messages"},
            { Routes.ClickatellBalance, "/public-client/balance"},
            { Routes.Organisation, "/api/organisations" },
            { Routes.Health, "/" },
            { Routes.ShortenUrl, "v4/shorten" },
            { Routes.WebCreateWallet, "create-wallet/details" },
            { Routes.SafePlacesLogin, "login" },
            { Routes.SafePlacesRedactedTrails, "redacted_trails" },
            { Routes.SafePlacesRedactedTrail, "redacted_trail" },
            { Routes.SafePlacesSafePath, "safe_path" }

        };

        public enum Routes
        {
            Agency,
            Custodian,
            Sendgrid,
            ClickatellSend,
            ClickatellBalance,
            Organisation,
            Health,
            ShortenUrl,
            WebCreateWallet,
            SafePlacesLogin,
            SafePlacesRedactedTrails,
            SafePlacesRedactedTrail,
            SafePlacesSafePath
        }
    }
}
