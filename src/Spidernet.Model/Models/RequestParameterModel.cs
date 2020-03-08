using System;
using System.Collections.Generic;
using System.Text;

namespace Spidernet.Model.Models {
  public class RequestParameterModel {
    public IDictionary<string,string> Headers { get; set; }
    public string Body { get; set; }
  }
}
