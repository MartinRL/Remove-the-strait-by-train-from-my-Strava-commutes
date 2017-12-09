using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;

namespace StraitOutTheStrait
{
    public class Program
    {
        internal static IStravaApi StravaApi = RestService.For<IStravaApi>("https://www.strava.com");

        public static async Task Main(string[] args)
        {
            Configure();

            var activities = GetActivities();

            var flaggedActivities = activities.Result.Where(_ => _.Flagged);
            foreach (var activity in flaggedActivities)
            {
                Console.WriteLine($"{activity.Id}, {activity.Start_Date_Local}");
            }
            
            Console.WriteLine(flaggedActivities.Count());
        }

        private static void Configure()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = {new StringEnumConverter()}
            };
        }
        
        private static async Task<IEnumerable<Activity>> GetActivities()
        {
            var pageIndex = 1;
            IEnumerable<Activity> activities = new List<Activity>();
            var apiResponse = await StravaApi.Activities();
            activities = activities.Concat(apiResponse);

            while (apiResponse.Count > 0)
            {
                pageIndex++;
                apiResponse = await StravaApi.Activities(pageIndex);
                activities = activities.Concat(apiResponse);
            }
            
            return activities;
        }
    }

    [Headers("Authorization: Bearer 08d451ee1e92376908306a2f9b92237ee0ae8ab9", "per_page: 1000")]
    public interface IStravaApi
    {
        [Get("/api/v3/athlete/activities")]
        Task<List<Activity>> Activities([AliasAs("page")] int pageIndex = 1);
    }

    public class Activity
    {
        public int Id { get; set; }
        public bool Flagged { get; set; }
        public DateTime Start_Date_Local { get; set; }
    }
}