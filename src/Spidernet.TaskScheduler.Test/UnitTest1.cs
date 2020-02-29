using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Spidernet.TaskScheduler.Test {
  public class UnitTest1 {
    /// <summary>
    /// 自己写的
    /// </summary>
    [Fact]
    public void ConvertObjectToDictionaryTest() {
      var jsonString = "{\"A\":\"A\",\"B\":\"B\",\"C\":\"C\",\"D\":\"D\",\"E\":{\"EA\":\"EA\",\"EB\":\"EB\",\"EC\":\"EC\",\"ED\":\"ED\",\"EE\":{\"EEA\":\"EEA\",\"EEB\":\"EEB\",\"EEC\":\"EEC\",\"EED\":\"EED\"}}}";
      var deSerObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);
      var deSerObjResult = ConvertObjectToDictionary(null, (IDictionary<string, object>)deSerObj);
    }

    /// <summary>
    /// 官方的
    /// </summary>
    [Fact]
    public void ConvertObjectToDictionaryTest1() {
      var jsonString = "{\"A\":\"A\",\"B\":\"B\",\"C\":\"C\",\"D\":\"D\",\"E\":{\"EA\":\"EA\",\"EB\":\"EB\",\"EC\":\"EC\",\"ED\":\"ED\",\"EE\":{\"EEA\":\"EEA\",\"EEB\":\"EEB\",\"EEC\":\"EEC\",\"EED\":\"EED\"}}}";
      var deSerObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(jsonString);
      var aaa = UnitTest1.Parse(deSerObj);
    }

    #region ConvertObjectToDictionaryTest1 需要



    private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new Stack<string>();
    private string _currentPath;

    private JsonTextReader _reader;

    public static IDictionary<string, string> Parse(JObject input) => new UnitTest1().ParseJObject(input);

    public static IDictionary<string, string> Parse(Stream input) => new UnitTest1().ParseStream(input);

    private IDictionary<string, string> ParseJObject(JObject input) {

      VisitJObject(input);
      return _data;
    }

    private IDictionary<string, string> ParseStream(Stream input) {
      _data.Clear();
      _reader = new JsonTextReader(new StreamReader(input));
      _reader.DateParseHandling = DateParseHandling.None;

      var jsonConfig = JObject.Load(_reader);

      VisitJObject(jsonConfig);

      return _data;
    }

    private void VisitJObject(JObject jObject) {
      foreach (var property in jObject.Properties()) {
        EnterContext(property.Name);
        VisitProperty(property);
        ExitContext();
      }
    }

    private void VisitProperty(JProperty property) {
      VisitToken(property.Value);
    }

    private void VisitToken(JToken token) {
      switch (token.Type) {
        case JTokenType.Object:
          VisitJObject(token.Value<JObject>());
          break;

        case JTokenType.Array:
          VisitArray(token.Value<JArray>());
          break;

        case JTokenType.Integer:
        case JTokenType.Float:
        case JTokenType.String:
        case JTokenType.Boolean:
        case JTokenType.Bytes:
        case JTokenType.Raw:
        case JTokenType.Null:
          VisitPrimitive(token.Value<JValue>());
          break;

        default:
          throw new FormatException();
      }
    }

    private void VisitArray(JArray array) {
      for (int index = 0; index < array.Count; index++) {
        EnterContext(index.ToString());
        VisitToken(array[index]);
        ExitContext();
      }
    }

    private void VisitPrimitive(JValue data) {
      var key = _currentPath;

      if (_data.ContainsKey(key)) {
        throw new FormatException();
      }
      _data[key] = data.ToString(CultureInfo.InvariantCulture);
    }
    private void EnterContext(string context) {
      _context.Push(context);
      _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext() {
      _context.Pop();
      _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    #endregion

    #region ConvertObjectToDictionaryTest 需要
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
    #endregion
  }
}
