using System;
using Xamarin.Forms;
using StepsCounterTestProject.iOS;
using StepsCounterApp;
using Foundation;
using HealthKit;
using System.Threading.Tasks;

[assembly: Dependency(typeof(HealthData))]
namespace StepsCounterTestProject.iOS
{
    public class HealthData : IHealthData
    {
        NSNumberFormatter numberFormatter;
        List<Task> tasks = new List<Task>();
        public HKHealthStore HealthStore { get; set; }

        NSSet DataTypesToWrite
        {
            get
            {
                return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {

                });
            }
        }

        NSSet DataTypesToRead
        {
            get
            {
                return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {
                    HKQuantityType.Create(HKQuantityTypeIdentifier.Height),
                    HKCharacteristicType.Create(HKCharacteristicTypeIdentifier.DateOfBirth),
                    HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount)
                });
            }
        }

        public void GetHealthPermissionAsync(Action<bool> completion)
        {
            if (HKHealthStore.IsHealthDataAvailable)
            {
                HealthStore = new HKHealthStore();
                HealthStore.RequestAuthorizationToShare(DataTypesToWrite, DataTypesToRead, (bool authorized, NSError error) =>
                {
                    completion(authorized);
                });
            }
            else
            {
                completion(false);
            }
        }

        public void FetchSteps(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0.0f);

                                var totalSteps = results.SumQuantity();
                                if (totalSteps == null)
                                    totalSteps = HKQuantity.FromQuantity(HKUnit.Count, 0.0);

                                completionHandler(totalSteps.GetDoubleValue(HKUnit.Count));
                            });
            HealthStore.ExecuteQuery(query);
        }

        void FetchMetersWalked(Action<double> completionHandler)
        {
            var calendar = NSCalendar.CurrentCalendar;
            var startDate = DateTime.Today;
            var endDate = DateTime.Now;
            var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning);

            var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

            var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
                            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
                            {
                                if (error != null && completionHandler != null)
                                    completionHandler(0);

                                var distance = results.SumQuantity();
                                if (distance == null)
                                    distance = HKQuantity.FromQuantity(HKUnit.Meter, 0);

                                completionHandler(distance.GetDoubleValue(HKUnit.Meter));
                            });
            HealthStore.ExecuteQuery(query);
        }

		void FetchActiveMinutes(Action<double> completionHandler)
		{
			var calendar = NSCalendar.CurrentCalendar;
			var startDate = DateTime.Today;
			var endDate = DateTime.Now;
			var stepsQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime);

			var predicate = HKQuery.GetPredicateForSamples((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

			var query = new HKStatisticsQuery(stepsQuantityType, predicate, HKStatisticsOptions.CumulativeSum,
							(HKStatisticsQuery resultQuery, HKStatistics results, NSError error) =>
							{
								if (error != null && completionHandler != null)
									completionHandler(0);

								var totalMinutes = results.SumQuantity();
								if (totalMinutes == null)
									totalMinutes = HKQuantity.FromQuantity(HKUnit.Minute, 0);

								completionHandler(totalMinutes.GetDoubleValue(HKUnit.Minute));
							});
			HealthStore.ExecuteQuery(query);
		}
    }
}
