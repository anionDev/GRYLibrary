using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.RequestLogging
{
    public class Request
    {
        public DateTime Timestamp { get; set; }
        public bool LogClientIP { get; set; }
        public IPAddress ClientIPAddress { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public IHeaderDictionary RequestHeader { get; set; }
        public string RequestBody { get; set; }
        public IDictionary<object, object> InformationFromController { get; set; }
        public ushort ResponseStatusCode { get; set; }
        public IHeaderDictionary ResponseHeader { get; set; }
        public string ResponseBody { get; set; }
        public IQueryCollection Query { get; set; }
        public Request(DateTime timestamp, IPAddress clientIPAddress, string method, string route, IQueryCollection query, IHeaderDictionary requestHeader, string requestBody, IDictionary<object, object> informationFromController, ushort responseStatusCode, IHeaderDictionary responseHeader, string responseBody)
        {
            this.Timestamp = timestamp;
            this.ClientIPAddress = clientIPAddress;
            this.Method = method;
            this.Route = route;
            this.RequestHeader = requestHeader;
            this.RequestBody = requestBody;
            this.InformationFromController = informationFromController;
            this.ResponseStatusCode = responseStatusCode;
            this.ResponseHeader = responseHeader;
            this.ResponseBody = responseBody;
            this.Query = query;
        }

        internal string GetFormattedQuery()
        {
            if(Query.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return "?" + string.Join(",", Query.Select(kvp => kvp.Key + "=" + kvp.Value));
            }
        }
    }
}
