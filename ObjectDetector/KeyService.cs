using System.Threading.Tasks;

namespace ObjectDetector
{
    public static class KeyService
    {
        const string PredictionKey = "PredictionKey";
        const string ProjectId = "ProjectId";
        const string PublishName = "PublishName";
        const string Endpoint = "Endpoint";

        public static Task<string> GetPredictionKey() => Xamarin.Essentials.SecureStorage.GetAsync(PredictionKey);
        public static Task SetPredictionKey(string value) => Xamarin.Essentials.SecureStorage.SetAsync(PredictionKey, value);

        public static Task<string> GetProjectId() => Xamarin.Essentials.SecureStorage.GetAsync(ProjectId);
        public static Task SetProjectId(string value) => Xamarin.Essentials.SecureStorage.SetAsync(ProjectId, value);

        public static Task<string> GetPublishName() => Xamarin.Essentials.SecureStorage.GetAsync(PublishName);
        public static Task SetPublishName(string value) => Xamarin.Essentials.SecureStorage.SetAsync(PublishName, value);

        public static Task<string> GetEndpoint() => Xamarin.Essentials.SecureStorage.GetAsync(Endpoint);
        public static Task SetEndpoint(string value) => Xamarin.Essentials.SecureStorage.SetAsync(Endpoint, value);
    }
}
