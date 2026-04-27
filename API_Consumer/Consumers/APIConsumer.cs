using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using API_Consumer;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace SQLAPI_Consumer
{
    /// <summary>
    /// This class will be used for consume API using the URL provided.
    /// Actually only GET & POST Method are supported.
    /// </summary>
    public static class APIConsumer
    {
        /// <summary>
        /// Fixed Context type supported.
        /// </summary>
        private const string CONTENTTYPE = "application/json; charset=utf-8";

        /// <summary>
        /// Fixed Context type supported.
        /// </summary>
        private const string CONTENTTYPE_URLENCODED = "application/x-www-form-urlencoded";

        /// <summary>
        /// Header Content-Type needed in generic calls.
        /// </summary>
        private const string Header_ContentType= "Content-Type";
        

        /// <summary>
        /// Fixed string for POST method
        /// </summary>
        private const string POST_WebMethod = "POST";

        /// <summary>
        /// Fixed string for GET method
        /// </summary>
        private const string GET_WebMethod = "GET";

        /// <summary>
        /// DEFAULT_EXECUTION_RESULT
        /// </summary>
        public const int DEFAULT_EXECUTION_RESULT = 0;

        /// <summary>
        /// FAILED_EXECUTION_RESULT
        /// </summary>
        public const int FAILED_EXECUTION_RESULT = -1;

        /// <summary>
        /// DEFAULT_COLUMN_RESULT
        /// </summary>
        public const string DEFAULT_COLUMN_RESULT = "Result";

        /// <summary>
        /// DEFAULT_COLUMN_RESULT
        /// </summary>
        public const string DEFAULT_COLUMN_ERROR = "Error";

        private enum ParamsName { webMethod , URL }

        // Add static HttpClient instance
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <param name="Authorization">Header Authorization token, user-passwrod, JWT, etc.</param>
        /// <returns>String Api result</returns>
        public static string POSTMethod(string url, string JsonBody, string Authorization = "")
        {
            string ContentResult = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = POST_WebMethod;

                if (!String.IsNullOrEmpty(Authorization))
                    request.Headers.Add(HttpRequestHeader.Authorization, Authorization);

                if (!String.IsNullOrEmpty(JsonBody))
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonBody);
                        streamWriter.Flush();
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }


        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <returns>String Api result</returns>
        public static string POSTMethod(string url, string JsonBody)
        {
            string ContentResult = string.Empty ;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = POST_WebMethod;

                if (!String.IsNullOrEmpty(JsonBody))
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonBody);
                        streamWriter.Flush();
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader?.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <param name="JsonHeaders">Headers added in Json format: Authorization token, user-passwrod, JWT, etc.</param>
        /// <returns>String Api result</returns>
        public static string POSTMethod_Header(string url, string JsonBody = "", string JsonHeaders = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = POST_WebMethod;

                if (!string.IsNullOrEmpty(JsonHeaders))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(JsonHeaders);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                            request.Headers.Add(Header.Name, Header.Value);
                    }
                }

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    if(!String.IsNullOrEmpty(JsonBody))
                        streamWriter.Write(JsonBody);

                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <param name="JsonHeaders">Headers added in Json format: Authorization token, user-passwrod, JWT, etc.</param>
        /// <returns>String Api result</returns>
        public static string POSTMethod_Extended(ref ExtendedResult extResult, string url, string JsonBody = "", string JsonHeaders = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
               
                request.Method = POST_WebMethod;

                if (!string.IsNullOrEmpty(JsonHeaders))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(JsonHeaders);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                        {
                            if (Header.Name.Contains(Header_ContentType))
                            {
                                request.ContentType = Header.Value;
                            }
                            else
                            {
                                request.Headers.Add(Header.Name, Header.Value);
                            }
                        }
                    }
                }

                // Set default Content-Type
                if (string.IsNullOrEmpty(request.ContentType))
                {
                    request.ContentType = CONTENTTYPE;
                }

                if (request.ContentType.ToLower() == CONTENTTYPE_URLENCODED.ToLower())
                {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes((!String.IsNullOrEmpty(JsonBody)) ? JsonBody : "");
                    // Set the ContentLength property of the WebRequest.  
                    request.ContentLength = byteArray.Length;

                    using (var streamWriter = request.GetRequestStream())
                    {
                        streamWriter.Write(byteArray, 0, byteArray.Length);
                        // Close the Stream object.  
                        streamWriter.Close();
                        // Get the response.  

                        streamWriter.Flush();
                    }
                }
                else
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        if (!String.IsNullOrEmpty(JsonBody))
                            streamWriter.Write(JsonBody);

                        streamWriter.Flush();
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();

                if (httpResponse != null)
                {
                    extResult.ContentType = httpResponse.ContentType;
                    extResult.Server = httpResponse.Server;
                    extResult.StatusCode = ((int)httpResponse?.StatusCode).ToString();
                    extResult.StatusDescription = httpResponse.StatusDescription;
                }

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    extResult.Result = ContentResult = result;

                    for (int i = 0; i < httpResponse.Headers.Count; ++i)
                    {
                        extResult.headers.Add(
                                                   new Headers()
                                                   {
                                                       Name = httpResponse.Headers.Keys[i],
                                                       Value = httpResponse.Headers[i]
                                                   }
                                            );
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response != null)
                        {
                            extResult.StatusCode = ((int)response.StatusCode).ToString();
                            extResult.StatusDescription = response.StatusDescription;
                            extResult.ContentType = response.ContentType;
                            extResult.Server = response.Server;
                        }

                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            extResult.Result = ContentResult = result;
                        }
                    }
                    else
                    {
                        extResult.Result = ContentResult = ex.Message.ToString();
                        extResult.StatusCode = ((int) HttpStatusCode.InternalServerError).ToString();
                        extResult.StatusDescription = HttpStatusCode.InternalServerError.ToString();
                    }

                    if (string.IsNullOrEmpty(extResult.Result))
                    {
                        extResult.Result = ContentResult;
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                extResult.Result = ContentResult;
                extResult.StatusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                extResult.StatusDescription = HttpStatusCode.InternalServerError.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <param name="JsonHeaders">Headers added in Json format: Authorization token, user-passwrod, JWT, etc.</param>
        /// <returns>String Api result</returns>
        public static string POSTMethod_urlencoded(string url, string JsonBody, string JsonHeaders = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE_URLENCODED;
                request.Method = POST_WebMethod;

                if (!string.IsNullOrEmpty(JsonHeaders))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(JsonHeaders);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                            request.Headers.Add(Header.Name, Header.Value);
                    }
                }

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes((!String.IsNullOrEmpty(JsonBody)) ? JsonBody : "");
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;

                using (var streamWriter = request.GetRequestStream())
                {
                    streamWriter.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.  
                    streamWriter.Close();
                    // Get the response.  


                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Header Authorization</param>
        /// <returns>String Api result</returns>
        public static string GETMethod(string url, string Authorization = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!String.IsNullOrEmpty(Authorization))
                    request.Headers.Add(HttpRequestHeader.Authorization, Authorization);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Header Authorization</param>
        /// <returns>String Api result</returns>
        public static string GETMethod_Headers(string url, string Headers)
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Headers))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(Headers);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                            request.Headers.Add(Header.Name, Header.Value);
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Header Authorization</param>
        /// <returns>String Api result</returns>
        public static string GETMethod_Headers(string url,  string JsonBody = "", string Headers = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Headers))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(Headers);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                            request.Headers.Add(Header.Name, Header.Value);
                    }
                }

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonBody);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="Id">Id</param>
        /// <param name="Authorization">Authorization</param>
        /// <returns>String Api result</returns>
        public static string GETMethod(string url, string Id, string Authorization = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(url,"/",Id));
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Authorization))
                    request.Headers.Add(HttpRequestHeader.Authorization, Authorization);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <typeparam name="T">Object used to deserialize the result</typeparam>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Authorization header</param>
        /// <returns>String Api result</returns>
        public static string GETMethod<T>(string url, ref T ObjectResult, string Authorization = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Authorization))
                    request.Headers.Add(HttpRequestHeader.Authorization, Authorization);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ObjectResult = JsonConvert.DeserializeObject<T>(result);
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <typeparam name="T">Object used to deserialize the result</typeparam>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Authorization header</param>
        /// <returns>String Api result</returns>
        public static string GETMethod<T>(string url, string Id, ref T ObjectResult, string Authorization = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(url,"/",Id));
                request.ContentType = CONTENTTYPE;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Authorization))
                    request.Headers.Add(HttpRequestHeader.Authorization, Authorization);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ObjectResult = JsonConvert.DeserializeObject<T>(result);
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }
         
        /// <summary>
        /// Request GET Method to the URL API provided.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="Authorization">Header Authorization</param>
        /// <returns>String Api result</returns>
        public static string GETMethod_Extended(ref ExtendedResult extResult, string url, string JsonBody = "", string Headers = "" )
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.ContentType = ContentType;
                request.Method = GET_WebMethod;

                if (!string.IsNullOrEmpty(Headers))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(Headers);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                        {
                            if (Header.Name.Contains(Header_ContentType))
                            {
                                request.ContentType = Header.Value;
                            }
                            else
                            {
                                request.Headers.Add(Header.Name, Header.Value);
                            }

                        }

                    }
                }

                // Set default Content-Type
                if (string.IsNullOrEmpty(request.ContentType))
                {
                    request.ContentType = CONTENTTYPE;
                }

                if (request.ContentType.ToLower() == CONTENTTYPE_URLENCODED.ToLower())
                {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes((!String.IsNullOrEmpty(JsonBody)) ? JsonBody : "");
                    // Set the ContentLength property of the WebRequest.  
                    request.ContentLength = byteArray.Length;

                    using (var streamWriter = request.GetRequestStream())
                    {
                        streamWriter.Write(byteArray, 0, byteArray.Length);
                        // Close the Stream object.  
                        streamWriter.Close();
                        // Get the response.  

                        streamWriter.Flush();
                    }
                }
                else if (!String.IsNullOrEmpty(JsonBody)
                      && !request.Method.ToUpper().Contains("GET"))
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonBody);
                        streamWriter.Flush();
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();

                if (httpResponse != null)
                {
                    extResult.ContentType = httpResponse.ContentType;
                    extResult.Server = httpResponse.Server;
                    extResult.Result = ContentResult;
                    extResult.StatusCode = ((int)httpResponse.StatusCode).ToString();
                    extResult.StatusDescription = httpResponse.StatusDescription;
                }

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    extResult.Result = ContentResult = result;

                    for (int i = 0; i < httpResponse.Headers.Count; ++i)
                    {
                        extResult.headers.Add(
                                                   new Headers()
                                                   {
                                                       Name = httpResponse.Headers.Keys[i],
                                                       Value = httpResponse.Headers[i]
                                                   }
                                            );
                    }

                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response != null)
                        {
                            extResult.StatusCode = ((int)response.StatusCode).ToString();
                            extResult.StatusDescription = response.StatusDescription;
                            extResult.ContentType = response.ContentType;
                            extResult.Server = response.Server;
                        }

                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            extResult.Result = ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                        extResult.StatusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                        extResult.StatusDescription = HttpStatusCode.InternalServerError.ToString();
                    }

                    if (string.IsNullOrEmpty(extResult.Result))
                    {
                        extResult.Result = ContentResult;
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                extResult.Result = ContentResult;
                extResult.StatusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                extResult.StatusDescription = HttpStatusCode.InternalServerError.ToString();
                throw ex;
            }

            return ContentResult;
        }


        /// <summary>
        /// POST to Resful API sending Json body.
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="JsonBody">Content Application By Default Json</param>
        /// <param name="JsonHeaders">Headers added in Json format: Authorization token, user-passwrod, JWT, etc.</param>
        /// <returns>String Api result</returns>
        public static string WebMethod(string httpMethod, string url, string JsonBody = "", string JsonHeaders = "")
        {
            string ContentResult = string.Empty;
            try
            {
                SetSSL();

                validateParams(ParamsName.webMethod, httpMethod);
                validateParams(ParamsName.URL, url);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
               // request.ContentType = Header_ContentType;
                request.Method = httpMethod;


                if (!string.IsNullOrEmpty(JsonHeaders))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(JsonHeaders);

                    foreach (var Header in _headers)
                    {
                        if (!string.IsNullOrEmpty(Header.Name) && !string.IsNullOrEmpty(Header.Value))
                        {
                            if (Header.Name.Contains(Header_ContentType))
                            {
                                request.ContentType = Header.Value;
                            }
                            else
                            {
                                request.Headers.Add(Header.Name, Header.Value);
                            }

                        }

                    }
                }

                // Set default Content-Type
                if (string.IsNullOrEmpty(request.ContentType))
                {
                    request.ContentType = CONTENTTYPE;
                }

                if (request.ContentType.ToLower() == CONTENTTYPE_URLENCODED.ToLower())
                {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes((!String.IsNullOrEmpty(JsonBody)) ? JsonBody : "");
                    // Set the ContentLength property of the WebRequest.  
                    request.ContentLength = byteArray.Length;

                    using (var streamWriter = request.GetRequestStream())
                    {
                        streamWriter.Write(byteArray, 0, byteArray.Length);
                        // Close the Stream object.  
                        streamWriter.Close();
                        // Get the response.  

                        streamWriter.Flush();
                    }
                }
                else if (!String.IsNullOrEmpty(JsonBody)
                        && !httpMethod.ToUpper().Contains("GET"))
                {
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonBody);
                        streamWriter.Flush();
                    }
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ContentResult = result;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response?.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            ContentResult = result;
                        }
                    }
                    else
                    {
                        ContentResult = ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ContentResult = ex.Message.ToString();
                throw ex;
            }

            return ContentResult;
        }

        /// <summary>
        /// Generic web method request.
        /// </summary>
        /// <param name="extResult">DTO which will hold the result set</param>
        /// <param name="httpMethod">Web method action</param>
        /// <param name="url">URL</param>
        /// <param name="JsonBody">Body</param>
        /// <param name="Headers">Headers</param>
        /// <returns></returns>
        public static string WebMethod_Extended(ref ExtendedResult extResult, string httpMethod, string url, string JsonBody = "", string Headers = "")
        {
            // Create a local copy to work with
            ExtendedResult localResult = extResult ?? new ExtendedResult();
            
            // Call the async method and wait for it synchronously without using Task.Run with ref parameter
            string result = WebMethod_ExtendedAsync(localResult, httpMethod, url, JsonBody, Headers).GetAwaiter().GetResult();
            
            // Copy the local result back to the ref parameter
            extResult = localResult;
            
            return result;
        }
        
        /// <summary>
        /// Async implementation of the WebMethod_Extended to avoid ref parameter in lambda issues
        /// </summary>
        private static async Task<string> WebMethod_ExtendedAsync(ExtendedResult extResult, string httpMethod, string url, string JsonBody = "", string Headers = "")
        {
            string contentResult = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                SetSSL();
                validateParams(ParamsName.webMethod, httpMethod);
                validateParams(ParamsName.URL, url);
                
                var request = new HttpRequestMessage(new HttpMethod(httpMethod), url);
                
                // Add headers
                if (!string.IsNullOrEmpty(Headers))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(Headers);
                    foreach (var header in _headers)
                    {
                        if (!string.IsNullOrEmpty(header.Name) && !string.IsNullOrEmpty(header.Value))
                        {
                            if (header.Name.Contains(Header_ContentType))
                            {
                                // Don't set content-type header directly on the request
                                // Store it for when we create the content
                                continue;
                            }
                            else
                            {
                                request.Headers.Add(header.Name, header.Value);
                            }
                        }
                    }
                }
                
                // Add content based on content type
                string contentType = CONTENTTYPE; // Default content type
                
                if (!string.IsNullOrEmpty(Headers))
                {
                    List<Headers> _headers = JsonConvert.DeserializeObject<List<Headers>>(Headers);
                    foreach (var header in _headers)
                    {
                        if (!string.IsNullOrEmpty(header.Name) && header.Name.Contains(Header_ContentType))
                        {
                            contentType = header.Value;
                            break;
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(JsonBody) && httpMethod.ToUpper() != "GET")
                {
                    if (contentType.ToLower() == CONTENTTYPE_URLENCODED.ToLower())
                    {
                        request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                    }
                    else
                    {
                        request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                    }
                }
                
                // Execute request
                response = await _httpClient.SendAsync(request);
                
                // Populate extended result with HTTP info
                extResult.StatusCode = ((int)response.StatusCode).ToString();
                extResult.StatusDescription = response.ReasonPhrase;
                extResult.ContentType = response.Content.Headers.ContentType?.MediaType;
                extResult.Server = response.Headers.Server?.ToString();
                
                // Get response content
                contentResult = await response.Content.ReadAsStringAsync();
                extResult.Result = contentResult;
                
                // Add response headers
                foreach (var header in response.Headers)
                {
                    extResult.headers.Add(new Headers { Name = header.Key, Value = string.Join(",", header.Value) });
                }
                
                // Add content headers too
                foreach (var header in response.Content.Headers)
                {
                    extResult.headers.Add(new Headers { Name = header.Key, Value = string.Join(",", header.Value) });
                }
                
                return contentResult;
            }
            catch (HttpRequestException ex)
            {
                extResult.Result = ex.Message;
                
                // Handle StatusCode property which may not exist in older .NET versions
                try
                {
                    // Use reflection to safely check for StatusCode property
                    var statusCodeProperty = ex.GetType().GetProperty("StatusCode");
                    if (statusCodeProperty != null)
                    {
                        var statusCode = statusCodeProperty.GetValue(ex) as HttpStatusCode?;
                        if (statusCode.HasValue)
                        {
                            extResult.StatusCode = ((int)statusCode.Value).ToString();
                            extResult.StatusDescription = statusCode.Value.ToString();
                        }
                        else
                        {
                            extResult.StatusCode = "500";
                            extResult.StatusDescription = "Internal Server Error";
                        }
                    }
                    else
                    {
                        extResult.StatusCode = "500";
                        extResult.StatusDescription = "Internal Server Error";
                    }
                }
                catch
                {
                    // Fallback if reflection fails
                    extResult.StatusCode = "500";
                    extResult.StatusDescription = "Internal Server Error";
                }
                
                // Include inner exception details if available
                if (ex.InnerException != null)
                {
                    extResult.Result += $" | Inner error: {ex.InnerException.Message}";
                }
                
                return extResult.Result;
            }
            catch (TaskCanceledException)
            {
                extResult.Result = "Request timed out or was canceled";
                extResult.StatusCode = "408"; // Request Timeout
                extResult.StatusDescription = "Request Timeout";
                return extResult.Result;
            }
            catch (JsonException ex)
            {
                extResult.Result = $"JSON parsing error: {ex.Message}";
                extResult.StatusCode = "400";
                extResult.StatusDescription = "Bad Request - Invalid JSON";
                return extResult.Result;
            }
            catch (Exception)
            {
                extResult.Result = "An unexpected error occurred";
                extResult.StatusCode = response?.StatusCode != null ? ((int)response.StatusCode).ToString() : "500";
                extResult.StatusDescription = response?.ReasonPhrase ?? "Internal Server Error";
                return extResult.Result;
            }
        }

        private static void validateParams(ParamsName pname, string paramVal)
        {
            string[] methods = { "POST", "GET", "PUT", "PATCH", "DELETE" };

            switch (pname)
            {
                case ParamsName.webMethod:
                    if (string.IsNullOrEmpty(paramVal))
                        throw new ArgumentNullException(pname.ToString(), "HTTP method cannot be null or empty.");
                
                // Ensure method is uppercase and in valid list
                string upperMethod = paramVal.ToUpperInvariant();
                if (!ContainAnyOf(upperMethod, methods))
                    throw new ArgumentException("Please provide a valid HTTP method (GET, POST, PUT, PATCH, DELETE).", pname.ToString());
                break;
            
                case ParamsName.URL:
                    if (string.IsNullOrEmpty(paramVal))
                        throw new ArgumentNullException(pname.ToString(), "URL cannot be null or empty.");
                
                // Validate URL format
                if (!Uri.IsWellFormedUriString(paramVal, UriKind.Absolute))
                    throw new ArgumentException("Please provide a valid absolute URL.", pname.ToString());
                break;
            }
        }

        /// <summary>
        /// Created this method to avoid using Contains from Linq library. (Less dependencies)
        /// </summary>
        /// <param name="word"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        private static bool ContainAnyOf(string word, string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (word.ToUpper() == array[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static void SetSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback = null; // Use default validation
        }
    }
}
