using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.Interface;
using System.ComponentModel;


namespace TeamDev.Redis.LanguageItems
{
  public static class TimeExtensions
  {
    
    public static bool Expire(IComplexItem item, int seconds)
    {      
      return item.Provider.ReadInt(item.Provider.SendCommand(RedisCommand.EXPIRE, item.KeyName, seconds.ToString())) == 1;
    }

    
    public static bool Persist(IComplexItem item)
    {      
      return item.Provider.ReadInt(item.Provider.SendCommand(RedisCommand.EXPIRE, item.KeyName)) == 1;
    }

    
    public static int TTL(IComplexItem item)
    {      
      return item.Provider.ReadInt(item.Provider.SendCommand(RedisCommand.TTL, item.KeyName));
    }
  }
}
