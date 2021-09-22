using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataObjects
{
    public class Test
    {
        private string test = "{\"Type\":\"Server.Chat\",\"sender\":\"doc\",\"data\":{\"message\":\"hallo\"},\"target\":\"henk\"}";

        public Test()
        {
            Root root = JsonConvert.DeserializeObject<Root>(test);

            Chat o = (root.data as JObject).ToObject<Chat>();

            Console.WriteLine(o.message);
        }
    }
}
