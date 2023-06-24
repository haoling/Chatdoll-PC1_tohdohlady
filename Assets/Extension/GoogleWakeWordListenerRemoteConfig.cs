using ChatdollKit.Extension.Google;

public class GoogleWakeWordListenerRemoteConfig : GoogleWakeWordListener
{
    protected override void Start()
    {
    }

    public void OnFetchRemoteConfig(string ApiKey)
    {
        this.ApiKey = ApiKey;
        
        // call Start of parent class
        base.Start();
    }
}