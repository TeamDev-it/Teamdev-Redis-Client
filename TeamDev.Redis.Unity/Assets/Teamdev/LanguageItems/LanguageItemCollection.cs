using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.Interface;


namespace TeamDev.Redis.LanguageItems
{
  public class LanguageItemCollection<T> where T : ILanguageItem, new()
  {
    internal RedisDataAccessProvider Provider { get; set; }

    public T this[string name]
    {
      get
      {
        var result = new T();
        result.Configure(name, Provider);
        return result;
      }
    }
  }
}
