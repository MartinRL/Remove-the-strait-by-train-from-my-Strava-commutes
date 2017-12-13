using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SplitTheStrait
{
    static class Program
    {
        static void Main(string[] args)
        {
            foreach (var file in Directory.EnumerateFiles(@"C:\strava", "*.gpx"))
            {
                Console.WriteLine($"Splitting: {file}");
                StraitSplitter.Split(XDocument.Load(file));
            }
        }
    }
    
    public static class StraitSplitter
    {
        public static void Split(XDocument faultyCommuteWithIncludedStrait)
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

            var date = DateTime
                .Parse(faultyCommuteWithIncludedStrait.Descendants().First(_ => _.Name.LocalName == "time").Value).Date
                .ToString("yy-MM-dd");
            
            swedishRide.Save(@"C:\strava\output\swedish_leg-"+date+".gpx");
            danishRide.Save(@"C:\strava\output\danish_leg-"+date+".gpx");
        }
    }
}
