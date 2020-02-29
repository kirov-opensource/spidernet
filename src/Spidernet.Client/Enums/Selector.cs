using System;

namespace Spidernet.Client.Enums {

  [Flags]
  public enum Selector {
    None = 0,
    XPath = 1 << 0,
    CSS = 1 << 1
  }
}
