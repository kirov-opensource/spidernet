﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Client.Enums {
  public enum OutputFrom {
    None = 0,
    Attribute = 1 << 0,
    InnerText = 1 << 1,
    InnerHtml = 1 << 2,
    OuterHtml = 1 << 3,
    InnerLength = 1 << 4,
    OuterLength = 1 << 5
  }
}
