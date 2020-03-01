using System;

namespace Spidernet.Model.Enums {
  [Flags]
  public enum OutputFromEnum {
    None = 0,
    Attribute = 1 << 0,
    InnerText = 1 << 1,
    InnerHtml = 1 << 2,
    OuterHtml = 1 << 3,
    InnerLength = 1 << 4,
    OuterLength = 1 << 5
  }
}
