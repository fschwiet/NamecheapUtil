using System;
using Namecheap.Util;
using NJasmine;

namespace Namecheap.Tests
{
    public class Can_set_a_hosts_entry : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var domainName =
                String.Format("foo{0}.{1}", Guid.NewGuid().ToString().Replace("-", ""),
                    Properties.Settings.Default.TestBaseDomain);

            var namecheapClient = new NamecheapClient(Properties.Settings.Default.APIKey, Properties.Settings.Default.APIUser, true);

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
