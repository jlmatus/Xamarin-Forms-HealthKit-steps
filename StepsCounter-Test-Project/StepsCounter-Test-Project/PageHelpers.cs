using System;
using Xamarin.Forms;

namespace StepsCounterApp
{
    public static class PageHelpers
    {
        static PageHelpers()
        {
        }

        public static void PresentUILoginScreen(this Page page, Xamarin.Auth.Authenticator authenticator)
        {

            Xamarin.Auth.Presenters.OAuthLoginPresenter presenter = null;
            presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);

        }
    }
}