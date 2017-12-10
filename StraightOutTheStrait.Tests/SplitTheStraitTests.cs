using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Xbehave;
using Xunit;

namespace StraightOutTheStrait.Tests
{
    public class SplitTheStraitTests
    {
        [Scenario]
        public void Split(
            XDocument faultyCommuteWithIncludedStrait,
            StraitSplitter straitSplitter,
            XDocument swedishRide,
            XDocument danishRide)
        {
            "Given the faulty commute"
                .x(() => faultyCommuteWithIncludedStrait = XDocument.Load(@"GPX-files\original.gpx"));

            "And a strait splitter"
                .x(() => straitSplitter = new StraitSplitter());
                
            "When I split the faulty commute"
                .x(() => (swedishRide, danishRide) = straitSplitter.Split(faultyCommuteWithIncludedStrait));

            "Then the Swedish ride is the Swedish legs"
                .x(() => Assert.Equal(XDocument.Load(@"GPX-files\Sweden-lon12.9.gpx").ToString(), swedishRide.ToString()));
            
            "Then the Danish ride is the Danish legs"
                .x(() => Assert.Equal(XDocument.Load(@"GPX-files\Denmark-lon12.6.gpx"), danishRide));
        }

        public class StraitSplitter
        {
            public (XDocument swedishRide, XDocument danishRide) Split(XDocument faultyCommuteWithIncludedStrait)
            {
                var swedishRide = new XDocument(faultyCommuteWithIncludedStrait);
                var danishRide = new XDocument(faultyCommuteWithIncludedStrait);
                
                swedishRide
                    .Descendants()
                    .Where(_ =>
                    {
                        bool hasLon;
                        try
                        {
                            hasLon = decimal.Parse(_.Attribute("lon").Value) < 12.9m;
                        }
                        catch
                        {
                            hasLon = false;
                        }

                        return hasLon;
                    }).Remove();
                
                danishRide
                    .Descendants()
                    .Where(_ =>
                    {
                        bool hasLon;
                        try
                        {
                            hasLon = decimal.Parse(_.Attribute("lon").Value) > 12.7m;
                        }
                        catch
                        {
                            hasLon = false;
                        }

                        return hasLon;
                    }).Remove();
                
                //Console.WriteLine(trkptNodes.Count());
                
                swedishRide.Save("swedish_leg.gpx");
                danishRide.Save("danish_leg.gpx");

                //throw new NotImplementedException($"original count: {faultyCommuteWithIncludedStrait.Descendants().Count()}, swedish count: {swedishRide.Descendants().Count()}, danish count: {danishRide.Descendants().Count()}");
                //    faultyCommuteWithIncludedStrait.Descendants().Count(_ => _.Name.LocalName == "trkpt").ToString());
                
                return (swedishRide, danishRide);
            }
        }
    }
}
