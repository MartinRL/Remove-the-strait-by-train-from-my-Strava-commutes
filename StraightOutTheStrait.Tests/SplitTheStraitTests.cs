using System.IO;
using System.Xml.Linq;
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
                .x(() => faultyCommuteWithIncludedStrait = XDocument.Parse(File.ReadAllText(@"GPX-files\original.gpx")));

            "And a strait splitter"
                .x(() => straitSplitter = new StraitSplitter());
                
            "When I split the faulty commute"
                .x(() => (swedishRide, danishRide) = straitSplitter.Split(faultyCommuteWithIncludedStrait));

            "Then the Swedish ride is the Swedish legs"
                .x(() => Assert.Equal(XDocument.Parse(File.ReadAllText(@"GPX-files\Sweden-lon12.9.gpx")), swedishRide));
            
            "Then the Danish ride is the Danish legs"
                .x(() => Assert.Equal(XDocument.Parse(File.ReadAllText(@"GPX-files\Denmark-lon12.6.gpx")), danishRide));
        }

        public class StraitSplitter
        {
            public (XDocument swedishRide, XDocument danishRide) Split(XDocument faultyCommuteWithIncludedStrait)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
