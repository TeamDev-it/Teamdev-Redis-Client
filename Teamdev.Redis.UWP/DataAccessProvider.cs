using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.LanguageItems;
using System.Net.Sockets;

namespace TeamDev.Redis
{
  public abstract class DataAccessProvider
  {
    public Configuration Configuration { get; set; }

    public DataAccessProvider()
    {
      Configuration = new LanguageItems.Configuration();
    }

    public abstract Socket Connect();
    public abstract void Close();

  }
}
