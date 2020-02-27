using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System.Globalization;
using System.IO;
using System.Text;

namespace DaemonApp
{
    internal class JsonNetSerializer : IJsonConverter
    {

        private readonly JsonSerializerSettings settings;

        public JsonNetSerializer()
        {
            this.settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public bool SerializeObject(object value, StringBuilder builder)
        {
            try
            {
                var serializer = JsonSerializer.CreateDefault(settings);
                var stringWriter = new StringWriter(builder, CultureInfo.InvariantCulture);
                using (var writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = serializer.Formatting;
                    serializer.Serialize(writer, value, null);
                }
                return true;
            }
            catch (System.Exception excp)
            {
                NLog.Common.InternalLogger.Error(excp, "Unable to serialize from the custom Json Serializer");
                return false;
            }
        }
    }
}