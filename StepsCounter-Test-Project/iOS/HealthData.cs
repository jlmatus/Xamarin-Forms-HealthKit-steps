using System;
using Xamarin.Forms;
using StepsCounterTestProject.iOS;
using StepsCounterApp;
using Foundation;
using HealthKit;

[assembly: Dependency(typeof(HealthData))]
namespace StepsCounterTestProject.iOS
{
    public class HealthData : IHealthData
    {
        NSNumberFormatter numberFormatter;

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
                HealthStore.RequestAuthorizationToShare(DataTypesToWrite, DataTypesToRead, (bool authorized, NSError error) => {
                    completion(authorized);
                });               
            } else {
                completion(false);
            }
        }

		public void FetchSteps(Action<double> completionHandler)
		{
			var calendar = NSCalendar.CurrentCalendar;
			var startDate = DateTime.Today;
			var endDate = DateTime.Now;
			//stepsQuantityType = HKQuantityType.quantityType(forIdentifier: .stepCount);
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

								if (completionHandler != null)
									completionHandler(totalSteps.GetDoubleValue(HKUnit.Count));
							});

			HealthStore.ExecuteQuery(query);
		}
    }
}
