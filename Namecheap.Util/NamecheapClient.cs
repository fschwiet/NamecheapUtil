using System;
using System.Linq;
using System.Net;

namespace Namecheap.Util
{
    public class NamecheapClient
    {
        private readonly string _apiKey;
        private readonly string _username;
        private readonly string _hostname;        

        public NamecheapClient(string apiKey, string username, bool useSandbox = false)
        {
            _apiKey = apiKey;
            _username = username;
            _hostname = useSandbox ? "api.sandbox.namecheap.com" : "api.namecheap.com";
        }

        public void SetHostEntry(string hostname, string address)
        {
            var addressPartsReversed = hostname.Split('.').Reverse();
            var sld = addressPartsReversed.Skip(1).First();
            var tld = addressPartsReversed.First();

            var querystringParameters = GetCommonParameters();
            querystringParameters.Add("Command", "namecheap.domains.dns.setHosts");
            querystringParameters.Add("SLD", sld);
            querystringParameters.Add("TLD", tld);
            querystringParameters.Add("HostName1", "@");
            querystringParameters.Add("RecordType1", "A");
            querystringParameters.Add("Address1", address);
            querystringParameters.Add("MXPref1", "10");    // only valid for MX records, though sample showed it being set
            querystringParameters.Add("TTL1", "180");
            querystringParameters.Add("EmailType", "OX"); // ?

            var url = string.Format("http://{0}/xml.response?{1}", _hostname, querystringParameters.AsQuerystring());
            Console.WriteLine("url: " + url);
            using(var client = new WebClient())
            {
                var response = client.DownloadString(url);

                Console.WriteLine(response);
            }

            throw new NotImplementedException();
        }

        public string GetHostEntry(string domainName)
        {
            throw new NotImplementedException();
        }

        private QuerystringParameters GetCommonParameters()
        {
            return new QuerystringParameters()
            {
                {"apiuser", _username},
                {"username", _username},
                {"apikey", _apiKey},
                {"ClientIp", "127.0.0.1"}
            };
        }
    }
}