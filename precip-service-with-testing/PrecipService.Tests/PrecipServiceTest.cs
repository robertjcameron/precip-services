using System;
using Precip.Services;
using Xunit;

namespace PrecipService.Tests
{
    public class PrecipServiceTest
    {
        
    private readonly Precip.Services.PrecipService _precipService;


    public PrecipServiceTest()
    {
        _precipService = new Precip.Services.PrecipService();
    }

        [Fact]
        /*
        This test is valid with code executed on September 16, 2019. When the code is changed on a different day, the average should be determined by-hand from the spreadsheet and
        the value replaced here. For running in a test suite used every day, another solution would be needed.
        */
        public void TestEmptyString()
        {
            var result = _precipService.GetPrecip();
            Assert.True(result.Equals("{\"date\":\"2019-09-16\",\"type\":\"predict\",\"value\":0.547142857142857}"),"today's result should be 0.547142857142857 but was " + result);        
        }

        [Fact]
        public void TestKnownDate()
        {
            var result = _precipService.GetPrecip("2016-09-02");
            Assert.True(result.Equals("{\"date\":\"2016-09-02\",\"type\":\"actual\",\"value\":0.13}"),"Sept 2 2016 result should be 0.13, was " + result);
        
        }
        [Fact]
        public void TestInvalidDate()
        {
            var result = _precipService.GetPrecip("I don't know, some time in July");
            Assert.True(result.Equals("{\"error\":\"invalid date\"}"),"should be invalid, was " + result);
        
        }

        [Fact]
        public void TestFutureDate()
        {
            var result = _precipService.GetPrecip("2020-12-23");
            Assert.True(result.Equals("{\"date\":\"2020-12-23\",\"type\":\"predict\",\"value\":0.428333333333333}"),"Dec 23 2019 result should be 0.428333333333333, was " + result);        
        }

        [Fact]
        public void TestDateBeforeDataSet()
        {
            var result = _precipService.GetPrecip("1994-12-23");
            Assert.True(result.Equals("{\"date\":\"1994-12-23\",\"type\":\"predict\",\"value\":0.428333333333333}"),"Dec 23 1994 result should be 0.428333333333333, was " + result);
        
        }

        [Fact]
        public void TestDateWithNoData()
        {
            var result = _precipService.GetPrecip("2020-02-29");
            Assert.True(result.Equals("{\"error\":\"no data found for date\"}"),"should be no data for leap day, was " + result);
                                      
        
        }

    }
}
