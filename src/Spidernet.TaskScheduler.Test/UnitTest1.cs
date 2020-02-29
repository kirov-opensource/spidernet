using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Xunit;

namespace Spidernet.TaskScheduler.Test {
  public class UnitTest1 {
    [Fact]
    public void ConvertObjectToDictionaryTest() {
      dynamic obj = new ExpandoObject();


      obj.A = "A";
      obj.B = "B";
      obj.C = "C";
      obj.D = "D";

      obj.E = new ExpandoObject();
      obj.E.EA = "EA";
      obj.E.EB = "EB";
      obj.E.EC = "EC";
      obj.E.ED = "ED";

      obj.E.EE = new ExpandoObject();
      obj.E.EE.EEA = "EEA";
      obj.E.EE.EEB = "EEB";
      obj.E.EE.EEC = "EEC";
      obj.E.EE.EED = "EED";


      var result = ConvertObjectToDictionary(null, (IDictionary<string, object>)obj);
      var objStr =Newtonsoft.Json.JsonConvert.SerializeObject(obj);
      var deSerObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(objStr);
      var deSerObjResult = ConvertObjectToDictionary(null, (IDictionary<string, object>)obj);
    }

    private IDictionary<string, object> ConvertObjectToDictionary(string prefixKey, IDictionary<string, object> obj) {
      IDictionary<string, object> keyValues = new Dictionary<string, object>();
      foreach (var property in obj) {
        var propertyType = property.Value.GetType();
        if ((propertyType.IsValueType && !propertyType.IsEnum) || propertyType == typeof(string)) {
          keyValues[$"{prefixKey}{property.Key}"] = property.Value;
        }
        else {
          var tempKeyValues = ConvertObjectToDictionary($"{prefixKey}{property.Key}.", (IDictionary<string, object>)property.Value);
          foreach (var tempPropertyInfo in tempKeyValues) {
            keyValues.Add(tempPropertyInfo);
          }
        }
      }
      return keyValues;
    }
  }
}
