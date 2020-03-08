using System;

namespace Spidernet.Model.Enums {
  [Flags]
  public enum SelectorEnum {
    None = 0,
    XPath = 1 << 0,
    CSS = 1 << 1
  }
}
