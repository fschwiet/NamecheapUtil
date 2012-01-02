using System;
using System.Linq;

namespace Namecheap.Util
{
    public class PartsOfDNSRecord
    {
        public string Subdomain;
        public string SecondLevelDomain;
        public string TopLevelDomain;

        public static PartsOfDNSRecord ExtractFromHostname(string hostname)
        {
            var addressPartsReversed = hostname.Split('.').Reverse();

            var result = new PartsOfDNSRecord()
            {
                Subdomain = "@",
                SecondLevelDomain = addressPartsReversed.Skip(1).First(),
                TopLevelDomain = addressPartsReversed.First()
            };

            if (addressPartsReversed.Count() > 2)
                result.Subdomain = string.Join(".", addressPartsReversed.Skip(2).Reverse());

            return result;
        }

        public static string GetSubdomain(string hostname)
        {
            return ExtractFromHostname(hostname).Subdomain;
        }
    }
}