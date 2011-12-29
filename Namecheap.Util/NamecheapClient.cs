using System;

namespace Namecheap.Util
{
    public class NamecheapClient
    {
        private readonly string _apiKey;

        public NamecheapClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public void SetHostEntry(string hostname, string address)
        {
            throw new NotImplementedException();
        }

        public string GetHostEntry(string domainName)
        {
            throw new NotImplementedException();
        }
    }
}