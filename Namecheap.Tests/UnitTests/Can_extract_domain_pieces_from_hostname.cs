using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using Namecheap.Util;

namespace Namecheap.Tests.UnitTests
{
    public class Can_extract_domain_pieces_from_hostname : GivenWhenThenFixture
    {
        public override void Specify()
        {
            should_handle_hostname_as("subdomain.sld.tld", "subdomain", "sld", "tld");
            should_handle_hostname_as("foobar.subdomain.sld.tld", "foobar.subdomain", "sld", "tld");
            should_handle_hostname_as("sld.tld", "@", "sld", "tld");
        }

        private void should_handle_hostname_as(
            string hostname, 
            string expectedSubdomain, 
            string expectedSecondLevelDomain, 
            string expectedTopLevelDomain)
        {
            given("hostname is " + hostname, delegate()
            {
                then("then the pieces can be extracted", delegate()
                {
                    var parts = PartsOfDNSRecord.ExtractFromHostname(hostname);

                    expect(() => parts.Subdomain == expectedSubdomain);
                    expect(() => parts.SecondLevelDomain == expectedSecondLevelDomain);
                    expect(() => parts.TopLevelDomain == expectedTopLevelDomain);
                });

                then("just the subdomain can be read", delegate()
                {
                    expect(() => PartsOfDNSRecord.GetSubdomain(hostname) == expectedSubdomain);
                });
            });
        }
    }
}
