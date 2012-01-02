using System;
using NUnit.Framework;
using Namecheap.Util;
using NJasmine;

namespace Namecheap.Tests
{
    public class Can_set_a_hosts_entry : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var domainName = GetUniqueDomainName();

            var namecheapClient = new NamecheapClient(
                Properties.Settings.Default.APIKey, 
                Properties.Settings.Default.APIUser, 
                true);

            when("we assign a hostname", delegate()
            {
                var firstIPAddress = "192.168.0.1";

                arrange(() => namecheapClient.SetHostEntry(domainName, firstIPAddress));

                then_hostname_has_IPAddress(namecheapClient, domainName, firstIPAddress);

                when("we re-assign the hostname", delegate()
                {
                    var secondIPAddress = "192.168.0.2";

                    arrange(() => namecheapClient.SetHostEntry(domainName, secondIPAddress));

                    then_hostname_has_IPAddress(namecheapClient, domainName, secondIPAddress);
                });
            });

            when("we assign 500 hostnames", delegate()
            {
                ignoreBecause("this test takes a long time.");

                arrange(delegate()
                {
                    for(var i = 0; i < 500; i++)
                        arrange(() => namecheapClient.SetHostEntry(GetUniqueDomainName(), "192.168.0.10"));
                });

                var lastDomainName = GetUniqueDomainName();
                var expectedLastIPAddress = "192.168.0.11";

                arrange(() => namecheapClient.SetHostEntry(lastDomainName, expectedLastIPAddress));

                describe("the assignments complete, and we can still set more", delegate()
                {
                    then_hostname_has_IPAddress(namecheapClient, lastDomainName, expectedLastIPAddress);
                });
            });

            when("we assign to a hostname that isn't ours", delegate()
            {
                var exception = arrange(() => Assert.Throws<Exception>(delegate()
                {
                    namecheapClient.SetHostEntry("this.is.not.our.domain", "192.168.0.1");
                }));

                then("the exception tells us the errorcode", delegate()
                {
                    expect(() => exception.Message == "Error reported by Namecheap webservice (2019166): Domain name not found");
                });
            });
        }

        private static string GetUniqueDomainName()
        {
            return String.Format("foo{0}.{1}", Guid.NewGuid().ToString().Replace("-", ""),
                Properties.Settings.Default.TestBaseDomain);
        }

        private void then_hostname_has_IPAddress(NamecheapClient namecheapClient, string domainName, string IPAddress)
        {
            then("it has the new value", delegate()
            {
                expect(() => namecheapClient.GetHostEntry(domainName) == IPAddress);
            });
        }
    }
}
