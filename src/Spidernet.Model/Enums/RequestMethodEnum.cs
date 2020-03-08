using System;
using System.Collections.Generic;
using System.Text;

namespace Spidernet.Model.Enums {
  public enum RequestMethodEnum {
    None = 0,
    Get = 1 << 0,
    Post = 1 << 1,
    Put = 1 << 2,
    Patch = 1 << 3,
    Delete = 1 << 4,
    Copy = 1 << 5,
    Merge = 1 << 6,
    Options = 1 << 7
  }
}
