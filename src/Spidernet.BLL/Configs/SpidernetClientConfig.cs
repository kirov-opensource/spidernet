﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spidernet.BLL.Configs {
  /// <summary>
  /// MQ配置文件
  /// </summary>
  public class MQConfig {
    /// <summary>
    /// 
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string VirtualHost { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string HostName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Queue { get; set; }
  }

  /// <summary>
  /// 
  /// </summary>
  public class SpidernetClientConfig {
    /// <summary>
    /// 
    /// </summary>
    public MQConfig MQConfig { get; set; }
  }
}