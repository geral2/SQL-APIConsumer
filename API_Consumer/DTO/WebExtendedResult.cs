using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace API_Consumer
{
    public class WebExtendedResult
    {
        public SqlString Result { get; set; }
        public SqlString StatusCode { get; set; }
        public SqlString StatusDescription { get; set; }
        public SqlString ContentType { get; set; }
        public SqlString Server { get; set; }
        public SqlString Headers { get; set; }

        public WebExtendedResult(SqlString _Result
                         , SqlString _StatusCode
                         , SqlString _StatusDescription
                         , SqlString _ContentType
                         , SqlString _Server
                        , SqlString _Headers
             )
        {
            this.Result = _Result;
            this.StatusCode = _StatusCode;
            this.StatusDescription = _StatusDescription;
            this.ContentType = _ContentType;
            this.Server = _Server;
            this.Headers = _Headers;
        }
    }
}
