using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects.DataObjects
{

    // __CR__ [PSMG] Plaats dit in aparte unit test code.
    public class Test
    {
        //private string test = "{\"Type\":\"Server.Chat\",\"sender\":\"doc\",\"data\":{\"message\":\"hallo\"},\"target\":\"henk\"}";

        public Test()
        {
            object[] obj = new object[] { 
                new Chat { message = "Hi" }, 
                new Connection() { connect = true }, 
                new HealthData() { AccWatt = 100, Heartbeat = 90, RPM = 40, Speed = 15, CurWatt = 500 }, 
                new Setting() { res = 80 }
                };
            Root[] root =new Root[obj.Length];
            string[] serialized = new string[obj.Length];
            Root[] deserializedRoot = new Root[obj.Length];
            object[] deserializedobj = new object[obj.Length];

            for (int i = 0; i < obj.Length; i++)
           {
                root[i] = new Root() { Type = obj[i].GetType().FullName, sender = "me", data = obj[i], target = "server" };

                serialized[i] = JsonConvert.SerializeObject(root[i]);

                deserializedRoot[i] = JsonConvert.DeserializeObject<Root>(serialized[i]);

                deserializedobj[i] = (deserializedRoot[i].data as JObject).ToObject(Type.GetType(deserializedRoot[i].Type));
            }



            Console.WriteLine(deserializedobj);
        }
    }
}
