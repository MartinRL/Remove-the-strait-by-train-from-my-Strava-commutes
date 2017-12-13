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
                .x(() => Assert.Equal(XDocument.Load(@"GPX-files\Denmark-lon12.6.gpx").ToString(), danishRide.ToString()));
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
                
                return (swedishRide, danishRide);
            }
        }
    }
}
