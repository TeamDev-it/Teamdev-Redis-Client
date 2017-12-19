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

    
    public string Get()
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GET, _name));
    }

    
    public string GetRange(int start, int stop)
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GETRANGE, _name, start.ToString(), stop.ToString()));
    }

    
    public string SubString(int start, int stop)
    {
      return _provider.ReadString(_provider.SendCommand(RedisCommand.GETRANGE, _name, start.ToString(), stop.ToString()));
    }

    
    public int GetBit(int offset)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.GETBIT, _name, offset.ToString()));
    }

    
    public int SetBit(int offset, short value)
    {
      if (value != 0 && value != 1)
        throw new ArgumentOutOfRangeException("value must be 0 or 1");

      return _provider.ReadInt(_provider.SendCommand(RedisCommand.SETBIT, _name, offset.ToString(), value.ToString()));
    }

    
    public void Set(string value)
    {
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.SET, _name, value));
    }

    
    public int Append(string value)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.APPEND, _name, value));
    }

    
    public int Decrement()
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.DECR, _name));
    }

    
    public int Decrement(int value)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.DECRBY, _name, value.ToString()));
    }

    
    public int Increment()
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.INCR, _name));
    }

    
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
