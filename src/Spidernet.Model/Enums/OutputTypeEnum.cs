using System;

namespace Spidernet.Model.Enums {
  [Flags]
  public enum OutputTypeEnum {
    None = 0,
    Text = 1 << 0,
    Array = 1 << 1
  }
}
