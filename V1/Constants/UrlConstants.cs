using System.Collections.Generic;

namespace CoviIDApiCore.V1.Constants
{
    public class UrlConstants
    {
        public static Dictionary<Routes, string> PartialRoutes = new Dictionary<Routes, string>
        {
            { Routes.Sendgrid, "v3/mail/send"},
            { Routes.ClickatellSend, "/messages"},
            { Routes.ClickatellBalance, "/public-client/balance"},
            { Routes.Organisation, "/api/organisations" },
            { Routes.Health, "/" },
            { Routes.ShortenUrl, "v4/shorten" },
            { Routes.WebCreateWallet, "create-wallet/details" }

        };

        public enum Routes
        {
            Sendgrid,
            ClickatellSend,
            ClickatellBalance,
            Organisation,
            Health,
            ShortenUrl,
            WebCreateWallet
        }
    }
}
