namespace Uber_Rider.DataModels
{
    public static class LetsRideCredentials
    {
        // public static string WebUrl = "http://192.168.254.38:45457/";
        // public static string WebUrl = "http://letsride-001-site1.btempurl.com/";
        public static string WebUrl = "http://192.168.0.10:45457/";
      //  public static string WebUrl = "http://192.168.43.2:45457/";
        public static string HubUrl = WebUrl + "hub/letsride";
        public static string Token = "";
        public static string SessionName = "userinfo";

    }
}