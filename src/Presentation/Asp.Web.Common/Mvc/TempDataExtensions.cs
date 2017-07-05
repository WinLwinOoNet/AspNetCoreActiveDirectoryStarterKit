using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Asp.Web.Common.Mvc
{
    /// <summary>
    /// http://stackoverflow.com/questions/34638823/store-complex-object-in-tempdata-in-mvc-6
    /// </summary>
    public static class TempDataExtensions
    {
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            try
            {
                object o;
                tempData.TryGetValue(key, out o);
                return o == null ? null : JsonConvert.DeserializeObject<T>((string) o);
            }
            catch
            {
                return null;
            }
        }
    }
}
