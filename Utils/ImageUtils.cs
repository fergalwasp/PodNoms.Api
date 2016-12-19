using System.Threading.Tasks;

namespace PodNoms.Api.Utils
{
    public class ImageUtils
    {
        public static async Task<string> GetRemoteImageAsBase64(string url)
        {

            var file = await HttpUtils.DownloadFile(url);
            if (System.IO.File.Exists(file))
            {
                byte[] data = System.IO.File.ReadAllBytes(file);
                string base64 = System.Convert.ToBase64String(data);
                return $"data:image/jpeg;base64,{base64}";
            }
            return string.Empty;
        }
    }
}