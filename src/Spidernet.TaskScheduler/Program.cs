using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Spidernet.TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = "{\"int\":11,\"float\":20.20,\"str\":\"strrrr\",\"array\":[\"s\",\"c\"]}";
            var parserObject = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            foreach (var c in parserObject.Children())
            {
                switch (c.Type)
                {
                    case Newtonsoft.Json.Linq.JTokenType.None:
                    case Newtonsoft.Json.Linq.JTokenType.Constructor:
                    case Newtonsoft.Json.Linq.JTokenType.Comment:
                    case Newtonsoft.Json.Linq.JTokenType.Undefined:
                    case Newtonsoft.Json.Linq.JTokenType.Bytes:
                    default:
                        break;

                    case Newtonsoft.Json.Linq.JTokenType.Object:
                        break;
                    case Newtonsoft.Json.Linq.JTokenType.Array:
                        break;
                    case Newtonsoft.Json.Linq.JTokenType.Property:
                    case Newtonsoft.Json.Linq.JTokenType.Integer:
                    case Newtonsoft.Json.Linq.JTokenType.Float:
                    case Newtonsoft.Json.Linq.JTokenType.String:
                    case Newtonsoft.Json.Linq.JTokenType.Boolean:
                    case Newtonsoft.Json.Linq.JTokenType.Null:
                    case Newtonsoft.Json.Linq.JTokenType.Date:
                    case Newtonsoft.Json.Linq.JTokenType.Raw:
                    case Newtonsoft.Json.Linq.JTokenType.Guid:
                    case Newtonsoft.Json.Linq.JTokenType.Uri:
                    case Newtonsoft.Json.Linq.JTokenType.TimeSpan:
                        c.FirstOrDefault().ToString();
                        break;
                }
            }



            Console.WriteLine("Hello World!");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
