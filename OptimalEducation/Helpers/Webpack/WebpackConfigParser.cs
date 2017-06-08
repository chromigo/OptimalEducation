using System.IO;
using Newtonsoft.Json;
using OptimalEducation.Helpers.Webpack.AssetsModel;

namespace OptimalEducation.Helpers.Webpack
{
    public static class WebpackConfigParser
    {
        public static WebpackAssets GetWebpackAssetsJson(string webpackAssetsFilePath)
        {
            var packageJson = File.ReadAllText(webpackAssetsFilePath);
            return JsonConvert.DeserializeObject<WebpackAssets>(packageJson);
        }
    }
}