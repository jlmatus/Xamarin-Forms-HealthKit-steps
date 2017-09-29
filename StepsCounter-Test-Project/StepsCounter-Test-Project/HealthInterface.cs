using System;
using System.Threading.Tasks;

namespace StepsCounterApp
{
	public interface IHealthData
	{		
        void FetchSteps(Action<double> completionHandler);
        void GetHealthPermissionAsync(Action<bool> completion);
	}
}
