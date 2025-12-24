using System.Globalization;

namespace MaysaraRazorPages.Helpers
{
    public static class CurrencyHelper
    {
        private static readonly CultureInfo OmrCulture = new CultureInfo("en-OM");

        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", OmrCulture);
        }
    }
}
