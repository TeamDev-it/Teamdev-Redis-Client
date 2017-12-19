using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.Interface;
using System.ComponentModel;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageString : ILanguageItem, IComplexItem
  {
    internal string _name;
    internal RedisDataAccessProvider _provider;

    [Description(CommandDescriptions.GET)]
    public string Get()
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GET, _name));
    }

    [Description(CommandDescriptions.GETRANGE)]
    public string GetRange(int start, int stop)
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GETRANGE, _name, start.ToString(), stop.ToString()));
    }

    [Description(CommandDescriptions.GETRANGE)]
    public string SubString(int start, int stop)
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GETRANGE, _name, start.ToString(), stop.ToString()));
    }

    [Description(CommandDescriptions.GETBIT)]
    public int GetBit(int offset)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.GETBIT, _name, offset.ToString()));
    }

    [Description(CommandDescriptions.SETBIT)]
    public int SetBit(int offset, short value)
    {
      if (value != 0 && value != 1)
        throw new ArgumentOutOfRangeException("value must be 0 or 1");

      return _provider.ReadInt(_provider.SendCommand(RedisCommand.SETBIT, _name, offset.ToString(), value.ToString()));
    }

    [Description(CommandDescriptions.SET)]
    public void Set(string value)
    {
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.SET, _name, value));
    }

    [Description(CommandDescriptions.APPEND)]
    public int Append(string value)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.APPEND, _name, value));
    }

    [Description(CommandDescriptions.DECR)]
    public int Decrement()
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.DECR, _name));
    }

    [Description(CommandDescriptions.DECRBY)]
    public int Decrement(int value)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.DECRBY, _name, value.ToString()));
    }

    [Description(CommandDescriptions.INCR)]
    public int Increment()
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.INCR, _name));
    }

    [Description(CommandDescriptions.INCRBY)]
    public int Increment(int value)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.INCRBY, _name, value.ToString()));
    }

    void ILanguageItem.Configure(string name, RedisDataAccessProvider provider)
    {
      _name = name;
      _provider = provider;
    }

    string IComplexItem.KeyName
    {
      get { return _name; }
    }

    RedisDataAccessProvider IComplexItem.Provider
    {
      get { return _provider; }
    }
  }
}
