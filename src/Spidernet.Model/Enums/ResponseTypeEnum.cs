using System;

namespace Spidernet.Model.Enums {
  [Flags]
  public enum ResponseTypeEnum {
    None = 0,
    Text = 1 << 0,
    JSON = 1 << 1,
    HTML = 1 << 2,
    XML = 1 << 3
  }
}
