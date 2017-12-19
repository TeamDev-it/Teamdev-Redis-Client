using System;
using System.Collections.Generic;

using System.Text;

using System.ComponentModel;
using TeamDev.Redis.Interface;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageKey : ILanguageItem
  {
    internal RedisDataAccessProvider _provider;

    [Description(CommandDescriptions.DEL)]
    public bool Remove(params string[] keys)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.DEL, keys)) == 1;
    }

    [Description(CommandDescriptions.EXISTS)]
    public bool Exists(string key)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.EXISTS, key)) == 1;
    }

    [Description(CommandDescriptions.KEYS)]
    public string[] this[string pattern]
    {
      get
      {
        return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.KEYS, pattern));
      }
    }

    [Description(CommandDescriptions.TYPE)]
    public string Type(string key)
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.TYPE, key));
    }

    [Description(CommandDescriptions.RENAMENX)]
    public bool Rename(string oldkey, string newkey)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.RENAMENX, oldkey, newkey)) == 1;
    }

    void ILanguageItem.Configure(string name, RedisDataAccessProvider provider)
    {
      _provider = provider;
    }
  }
}
