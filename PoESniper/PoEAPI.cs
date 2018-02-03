using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace PoESniper
{
    class PoEAPI
    {
        private readonly string apiURL = "http://www.pathofexile.com/api/public-stash-tabs?id=";
        private string nextChangeID;
        public PoEAPI()
        {
            //get latest ChangeID
            updateNextChangeID();
        }

        public string getNextChangeID()
        {
            return nextChangeID;
        }

        public void updateNextChangeID()
        {
            WebClient c = new WebClient();
            nextChangeID = "50905812-53995321-50464881-58726103-54612698";  // ToDo: Read last
            byte[] responseFromNinja = c.DownloadData("http://api.poe.ninja/api/Data/GetStats");
            JObject responseNinja = JObject.Parse(Encoding.ASCII.GetString(responseFromNinja));
            JToken lastRequestID = responseNinja["nextChangeId"];
            nextChangeID = lastRequestID.ToString().Replace('"', '\0');
        }

        public JObject doUpdate()
        {
            try
            { 
            MyWebClient c = new MyWebClient();
            c.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            c.Encoding = Encoding.UTF8;
            byte[] response = c.DownloadData(apiURL + nextChangeID);
            JObject responseJson = JObject.Parse(Encoding.ASCII.GetString(response));
            JToken lastRequestID = responseJson["next_change_id"];
            nextChangeID = lastRequestID.ToString().Replace('"', '\0');
            return responseJson;
            }catch (Exception e)
            {

            }
            return null;
        }
    }
    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
}
