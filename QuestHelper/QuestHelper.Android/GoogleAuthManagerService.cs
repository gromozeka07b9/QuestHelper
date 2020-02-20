using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Analytics;
using Plugin.CurrentActivity;
using QuestHelper.Droid;
using QuestHelper.Model;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(GoogleAuthManagerService))]
namespace QuestHelper.Droid
{
    public class GoogleAuthManagerService : Java.Lang.Object, QuestHelper.IGoogleAuthManagerService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
		public Action<GoogleUser, string> _onLoginComplete;
		public static GoogleApiClient _googleApiClient { get; set; }
		public static GoogleAuthManagerService Instance { get; private set; }

		public GoogleAuthManagerService()
		{
			Instance = this;
			GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
																.RequestEmail()
																.Build();

			_googleApiClient = new GoogleApiClient.Builder(Android.App.Application.Context)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
				.AddScope(new Scope(Scopes.Profile))
				.Build();
		}

		public void Login(Action<GoogleUser, string> onLoginComplete)
		{
			_onLoginComplete = onLoginComplete;
			Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(_googleApiClient);
			CrossCurrentActivity.Current.Activity.StartActivityForResult(signInIntent, 1);
			_googleApiClient.Connect();
		}

		public void Logout()
		{
			_googleApiClient.Disconnect();
		}

		public void OnAuthCompleted(GoogleSignInResult result)
		{
			if (result.IsSuccess)
			{
				GoogleSignInAccount account = result.SignInAccount;
				Analytics.TrackEvent("GoogleAuthManager", new Dictionary<string, string> { { "account", account.DisplayName } });
				_onLoginComplete?.Invoke(new GoogleUser()
				{
					Name = account.DisplayName,
					Email = account.Email,
					Id = account.Id,
					ImgUrl = new Uri((account.PhotoUrl != null ? $"{account.PhotoUrl}" : $"http://igosh.pro/images/avatar.png"))
				}, string.Empty);
			}
			else
			{
				string errorCodeText = result.Status?.StatusCode.ToString();
				HandleError.Process("GoogleAuthManager", "OnAuthCompleted", new Exception(string.IsNullOrEmpty(errorCodeText) ? errorCodeText : "unknown error"), false);
				_onLoginComplete?.Invoke(null, string.Empty);
			}
		}

		public void OnConnected(Bundle connectionHint)
		{

		}

		public void OnConnectionSuspended(int cause)
		{
			HandleError.Process("GoogleAuthManager", "OnConnectionSuspended", new Exception("Cancelled"), false);
			_onLoginComplete?.Invoke(null, "Canceled!");
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			HandleError.Process("GoogleAuthManager", "OnConnectionFailed", new Exception(result.ErrorMessage), false);
			_onLoginComplete?.Invoke(null, result.ErrorMessage);
		}
	}
}