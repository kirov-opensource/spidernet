using System;

namespace Spidernet.Client.Enums {

  [Flags]
  public enum Parser {
    None = 0,
    String = 1 << 0,
    Raw = 1 << 1,
    Array = 1 << 2
  }
}
