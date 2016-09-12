using Android.App;
using Android.OS;
using Android.Widget;
using System.Json;
using System.Threading.Tasks;
using Xamarin.Auth;
using System;
using Android.Content;

namespace SampleFbLoginApp
{
    [Activity(Label = "SampleFbLoginApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Intent activity2;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var twitter = FindViewById<Button>(Resource.Id.TwitterButton);
            twitter.Click += delegate { LoginToTwitter(true); };

            activity2 = new Intent(this, typeof(Userdata));
        }

        private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        void LoginToTwitter(bool allowCancel)
        {
            var auth = new OAuth1Authenticator(
                            Constants.TwitterCosnumerKey,
                            Constants.TwitterConsumerSecret,
                            new Uri(Constants.reqToken),
                            new Uri(Constants.twitterAuthorize),
                            new Uri(Constants.accessToken),
                            new Uri(Constants.sight));
            //We should give redirectUrl in redirect URl option in products in facebook-developers options
            
            auth.Completed += twitter_auth_Completed;
            progressDialog = ProgressDialog.Show(this, Constants.wait, Constants.info, true);
            progressDialog.Show();
            StartActivity(auth.GetUI(this));
        }

        private async void twitter_auth_Completed(object sender, AuthenticatorCompletedEventArgs eventArgs)
        {
            if (eventArgs.IsAuthenticated)
            {
                Toast.MakeText(this, "Authenticated.!!", ToastLength.Long).Show();
                progressDialog.Hide();
                //use the account object and make the desired API call
                var request = new OAuth1Request(
                                      "GET",
                                      new Uri(Constants.TwitterUrl),
                                      null,
                                      eventArgs.Account);

                await request.GetResponseAsync().ContinueWith(t =>
                {
                    var resString = t.Result.GetResponseText();
                    var data = JsonValue.Parse(resString);
                    DataClass.userData = data;
                    StartActivity(activity2);
                });
            }
        }
    }
}