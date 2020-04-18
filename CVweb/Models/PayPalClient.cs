using PayPalCheckoutSdk.Core;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Models
{
    public static class PayPalClient
    {
        /*
            Set up PayPal environment with sandbox credentials.
            In production, use LiveEnvironment.
         */
        public static PayPalEnvironment GetEnvironment() =>
            new SandboxEnvironment(@"AVz45kKTZqHbWZfDsb4FQvQ1hk9-4MuwWgyD7C5gn6RTJoOXLJFxGElvvlBjPI1fc__l4toYucGpnrO5",
            @"EIcWJy41ot1YM6N2g_ZP_NZFayfX6lbTDRYptmzsBkdCjMBH9qw-d1eTXxO4luiewkszX6G6G8Sn12RS");

        /*
            Returns PayPalHttpClient instance to invoke PayPal APIs.
         */
        public static HttpClient GetClient() => new PayPalHttpClient(GetEnvironment());

        public static HttpClient GetClient(string refreshToken) => new PayPalHttpClient(GetEnvironment(), refreshToken);
    }
}
