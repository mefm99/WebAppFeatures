using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebAppFeatures.Helpers
{
    public class IpInfo
    {
        public string Country { get; set; }
        public string City { get; set; }
    }
    public static class GetInfoByIP
    {
        public static string GetCountery()
        {
            IpInfo ipInfo = new();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io");
                JavaScriptSerializer jsonObject = new ();
                ipInfo = jsonObject.Deserialize<IpInfo>(info);
                CultureInfo ci = new (ipInfo.Country);
                RegionInfo regionInfo = new(ci.Name);
                return ipInfo.Country = regionInfo.EnglishName;


            }
            catch (Exception)
            {

                return ipInfo.Country="";
            }
        }
        public static string GetCity()
        {
            IpInfo ipInfo = new();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io");
                JavaScriptSerializer jsonObject = new ();
                ipInfo = jsonObject.Deserialize<IpInfo>(info);
                return ipInfo.City;
            }
            catch (Exception)
            {
                return ipInfo.City="";
            }
        }
    }

}
