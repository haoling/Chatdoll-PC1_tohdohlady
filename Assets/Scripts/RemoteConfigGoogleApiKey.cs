using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using ChatdollKit.Extension.Google;

public class RemoteConfigGoogleApiKey : MonoBehaviour
{
    public struct userAttributes {}
    public struct appAttributes {}

    async Task InitializeRemoteConfigAsync()
    {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
    }

    async Task Start()
    {
        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        if (GetComponent<GoogleTTSLoader>() != null)
        {
            GetComponent<GoogleTTSLoader>().ApiKey = RemoteConfigService.Instance.appConfig.GetString("GOOGLE_API_KEY");
        }
        if (GetComponent<GoogleVoiceRequestProvider>() != null)
        {
            GetComponent<GoogleVoiceRequestProvider>().ApiKey = RemoteConfigService.Instance.appConfig.GetString("GOOGLE_API_KEY");
        }
        if (GetComponent<GoogleWakeWordListener>() != null)
        {
            GetComponent<GoogleWakeWordListener>().ApiKey = RemoteConfigService.Instance.appConfig.GetString("GOOGLE_API_KEY");
        }
    }   
}