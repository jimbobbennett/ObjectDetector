using System.Threading.Tasks;

namespace ObjectDetector
{
    public static class KeyService
    {
        const string PredictionKey = "PredictionKey";
        const string ProjectId = "ProjectId";

        public static Task<string> GetPredictionKey() => Xamarin.Essentials.SecureStorage.GetAsync(PredictionKey);
        public static Task SetPredictionKey(string value) => Xamarin.Essentials.SecureStorage.SetAsync(PredictionKey, value);

        public static Task<string> GetProjectId() => Xamarin.Essentials.SecureStorage.GetAsync(ProjectId);
        public static Task SetProjectId(string value) => Xamarin.Essentials.SecureStorage.SetAsync(ProjectId, value);
    }
}
