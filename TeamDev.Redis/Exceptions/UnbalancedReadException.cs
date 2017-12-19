using System;
using System.Collections.Generic;

using System.Text;

namespace TeamDev.Redis
{
  public class UnbalancedReadException : Exception
  {
    public UnbalancedReadException()
      : base()
    { }

    public UnbalancedReadException(string message)
      : base(message)
    { }
  }
}
