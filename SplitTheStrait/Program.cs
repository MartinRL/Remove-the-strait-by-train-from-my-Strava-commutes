using System;
using System.Linq;
using System.Xml.Linq;

namespace SplitTheStrait
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    
    public class StraitSplitter
    {
        public void Split(XDocument faultyCommuteWithIncludedStrait)
        {
            var swedishRide = new XDocument(faultyCommuteWithIncludedStrait);
            var danishRide = new XDocument(faultyCommuteWithIncludedStrait);
                
            swedishRide
                .Descendants()
                .Where(_ =>
                {
                    bool swedishBorderLon;
                    try
                    {
                        swedishBorderLon = decimal.Parse(_.Attribute("lon").Value) < 12.9m;
                    }
                    catch
                    {
                        swedishBorderLon = false;
                    }

                    return swedishBorderLon;
                }).Remove();
                
            danishRide
                .Descendants()
                .Where(_ =>
                {
                    bool danishBorderLon;
                    try
                    {
                        danishBorderLon = decimal.Parse(_.Attribute("lon").Value) > 12.7m;
                    }
                    catch
                    {
                        danishBorderLon = false;
                    }

                    return danishBorderLon;
                }).Remove();
                
            danishRide.Descendants().First(_ => _.Name.LocalName == "time").Value =
                danishRide.Descendants().Where(_ => _.Name.LocalName == "time").ElementAt(1).Value;
                
            swedishRide.Save("swedish_leg.gpx");
            danishRide.Save("danish_leg.gpx");
        }
    }
}
