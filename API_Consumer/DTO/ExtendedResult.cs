using System;
using System.Collections.Generic;
using System.Text;

namespace API_Consumer
{
    public class ExtendedResult
    {
        public string Result { get; set; }
        //public System.Collections.Specialized.NameValueCollection Headers;
        public List<Headers> headers = new List<Headers>();
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Status { get; set; }
        public string ContentType { get; set; }
        public string Server { get; set; }

    }
}
