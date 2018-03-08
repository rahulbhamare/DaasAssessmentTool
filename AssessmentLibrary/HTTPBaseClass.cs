using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class HTTPBaseClass
    {
        private string UserName;
        private string UserPwd;
        private string ProxyServer;
        private string RequestUri;
        private string RequestMethod;
        NameValueCollection Headers = new NameValueCollection();
        public HTTPBaseClass(string HttpUserName, string HttpUserPwd, string HttpProxyServer,
           string HttpRequest, string HttpRequestMethod, NameValueCollection HttpHeader)
        {
            UserName = HttpUserName;
            UserPwd = HttpUserPwd;
            ProxyServer = HttpProxyServer;
            RequestUri = HttpRequest;
            RequestMethod = HttpRequestMethod;
            Headers = HttpHeader;
        }

        /// &lt;summary>
        /// This method creates secure/non secure web
        /// request based on the parameters passed.
        /// &lt;/summary>
        /// &lt;param name="NwCred">In case of secure request this would be true&lt;/param>
        /// &lt;returns>&lt;/returns>
        public virtual HttpWebRequest CreateWebRequest(bool NwCred, bool allowRedirect)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(RequestUri);
            webrequest.Accept = "*/*";
            webrequest.Method = RequestMethod;
            //webrequest.MaximumAutomaticRedirections = 1;
            webrequest.AllowAutoRedirect = allowRedirect;
            int iCount = Headers.Count;
            string key;
            string keyvalue;

            for (int i = 0; i < iCount; i++)
            {
                key = Headers.Keys[i];
                keyvalue = Headers[i];
                webrequest.Headers.Add(key, keyvalue);
            }
            webrequest.ContentType = "text/html";
            WebProxy webProxy = new WebProxy(ProxyServer);
            if (NwCred)
            {
                //CredentialCache wrCache = new CredentialCache();
                //wrCache.Add(new Uri(RequestUri), "Basic", new NetworkCredential(UserName, UserPwd));
                //webrequest.Credentials = CredentialCache.DefaultCredentials;

                //Set proxy credentials                 
                webProxy.Credentials = new NetworkCredential(UserName, UserPwd);                
            }
            webrequest.Proxy = webProxy;
            //Remove collection elements
            Headers.Clear();
            return webrequest;
        }//End of secure CreateWebRequest

        /// &lt;summary>
        /// This method retreives redirected URL from
        /// response header and also passes back
        /// any cookie (if there is any)
        /// &lt;/summary>
        /// &lt;param name="webresponse">&lt;/param>
        /// &lt;param name="Cookie">&lt;/param>
        /// &lt;returns>&lt;/returns>
        public virtual string GetRedirectURL(HttpWebResponse
             webresponse, ref string Cookie)
        {
            string uri = "";

            WebHeaderCollection headers = webresponse.Headers;

            if ((webresponse.StatusCode == HttpStatusCode.Found) ||
              (webresponse.StatusCode == HttpStatusCode.Redirect) ||
              (webresponse.StatusCode == HttpStatusCode.Moved) ||
              (webresponse.StatusCode == HttpStatusCode.MovedPermanently))
            {
                // Get redirected uri
                uri = headers["Location"];
                uri = uri.Trim();
            }

            //Check for any cookies
            if (headers["Set-Cookie"] != null)
            {
                Cookie = headers["Set-Cookie"];
            }
            //                string StartURI = "http:/";
            //                if (uri.Length > 0 && uri.StartsWith(StartURI)==false)
            //                {
            //                      uri = StartURI + uri;
            //                }
            return uri;
        }//End of GetRedirectURL method


        public virtual HttpWebResponse GetFinalResponse(string Cookie, bool NwCred, bool autoRedirect)
        {
            NameValueCollection collHeader = new NameValueCollection();

            if (Cookie.Length > 0)
            {
                collHeader.Add("Cookie", Cookie);
            }

            HttpWebRequest webrequest = CreateWebRequest(NwCred, autoRedirect);
            BuildReqStream(ref webrequest);
            HttpWebResponse webresponse;
            webresponse = (HttpWebResponse)webrequest.GetResponse();
            return webresponse;

            //    Encoding enc = System.Text.Encoding.GetEncoding(1252);
            //StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            //string Response = loResponseStream.ReadToEnd();
        }

        private void BuildReqStream(ref HttpWebRequest webrequest)
        //This method build the request stream for WebRequest
        {
            byte[] bytes = Encoding.ASCII.GetBytes(RequestUri);
            webrequest.ContentLength = bytes.Length;

            Stream oStreamOut = webrequest.GetRequestStream();
            oStreamOut.Write(bytes, 0, bytes.Length);
            oStreamOut.Close();
        }    
    }
}
