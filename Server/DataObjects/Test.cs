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
        //private string test = "{\"Type\":\"Server.Chat\",\"sender\":\"doc\",\"data\":{\"message\":\"hallo\"},\"target\":\"henk\"}";

        public Test()
        {
            Object obj = new Chat { message = "Hi" };

            Root root = new Root() {Type = obj.GetType().FullName, sender = "me", data = obj, target = "you" };

            string test = JsonConvert.SerializeObject(root);

            Root serializedRoot = JsonConvert.DeserializeObject<Root>(test);

            Chat serializedChat = (serializedRoot.data as JObject).ToObject<Chat>();

            Console.WriteLine(serializedChat.message);
        }
    }
}
