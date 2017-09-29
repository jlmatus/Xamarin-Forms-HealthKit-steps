using System;
using Xamarin.Forms;

namespace StepsCounterApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DependencyService.Get<IHealthData>().GetHealthPermissionAsync((result) =>
           {
           var a = result;
                if (result)  {
                    DependencyService.Get<IHealthData>().FetchSteps((totalSteps) => {						
							Device.BeginInvokeOnMainThread(() =>
							{
                            this.Label1.Text = "Total steps today: " + Math.Floor(totalSteps).ToString();
							});
                    });
                }            
            });
        }
    }
}
