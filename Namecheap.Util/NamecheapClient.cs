using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

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
            var querystringParameters = StartParametersForCommand("namecheap.domains.dns.setHosts");

            AddSDLandTLDParameters(hostname, querystringParameters);
            querystringParameters.Add("HostName1", PartsOfDNSRecord.GetSubdomain(hostname));
            querystringParameters.Add("RecordType1", "A");
            querystringParameters.Add("Address1", address);
            querystringParameters.Add("MXPref1", "10");    // only valid for MX records, though sample showed it being set
            querystringParameters.Add("TTL1", "180");
            querystringParameters.Add("EmailType", "OX"); // ?

            var result = GetApiResult(querystringParameters);
        }

        public string GetHostEntry(string hostname)
        {
            var querystringParameters = StartParametersForCommand("namecheap.domains.dns.getHosts");
            AddSDLandTLDParameters(hostname, querystringParameters);

            var subdomain = PartsOfDNSRecord.GetSubdomain(hostname);

            var result = GetApiResult(querystringParameters);

            var ns = XNamespace.Get("http://api.namecheap.com/xml.response");

            var commandResponse = result.Root.Elements(ns + "CommandResponse").Single();
            var getHostsResult = commandResponse.Elements(ns + "DomainDNSGetHostsResult").Single();
            var hosts = getHostsResult.Elements(ns + "host");
            var host = hosts.Where(h => h.Attribute("Name").Value == subdomain).Single();

            return host.Attribute("Address").Value;
        }

        private XDocument GetApiResult(QuerystringParameters querystringParameters)
        {
            XDocument result;

            var url = string.Format("http://{0}/xml.response?{1}", _hostname, querystringParameters.AsQuerystring());

            using (var client = new WebClient())
            {
                var response = client.DownloadString(url);

                Console.WriteLine(response);

                result = XDocument.Parse(response);
                var value = result.Root.Attribute("Status").Value;

                //if (value != "OK")
                //    throw new Exception("Namecheap service call failed");
            }
            return result;
        }

        private QuerystringParameters StartParametersForCommand(string command)
        {
            return new QuerystringParameters()
            {
                {"apiuser", _username},
                {"username", _username},
                {"apikey", _apiKey},
                {"ClientIp", "127.0.0.1"},
                {"Command", command}
            };
        }

        private static void AddSDLandTLDParameters(string hostname, QuerystringParameters querystringParameters)
        {
            var pieces = PartsOfDNSRecord.ExtractFromHostname(hostname);

            querystringParameters.Add("SLD", pieces.SecondLevelDomain);
            querystringParameters.Add("TLD", pieces.TopLevelDomain);
        }
    }
}