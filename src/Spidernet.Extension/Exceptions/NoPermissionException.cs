using System;

namespace Spidernet.Extension.Exceptions {
  public class NoPermissionException : ApplicationException {

    public NoPermissionException() { }

    public NoPermissionException(string message) : base(message) {

    }
  }
}
