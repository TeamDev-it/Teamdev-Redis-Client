using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.Interface;
using System.ComponentModel;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageHash : ILanguageItem, IComplexItem
  {
    internal string _name;
    internal RedisDataAccessProvider _provider;

    
    public void Clear()
    {
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.DEL, _name));
    }

    
    public string this[string field]
    {
      
      get
      {        
        return _provider.ReadString(_provider.SendCommand(RedisCommand.HGET, _name, field));
      }
      
      set
      {        
        _provider.WaitComplete(_provider.SendCommand(RedisCommand.HSET, _name, field, value));
      }
    }

    
    public string Get(string field)
    {      
      return _provider.ReadString(_provider.SendCommand(RedisCommand.HGET, _name, field));
    }

    
    public bool Set(string field, string value)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.HSET, _name, field, value)) == 1;
    }

    
    public KeyValuePair<string, string>[] Items
    {
      get
      {        
        var result = _provider.ReadMultiString(_provider.SendCommand(RedisCommand.HGETALL, _name));

        var values = new List<KeyValuePair<string, string>>();
        if (result != null)
        {
          if (result.Length % 2 > 0) throw new InvalidOperationException("Invalid number of results");

          for (int x = 0; x < result.Length; x += 2)
            values.Add(new KeyValuePair<string, string>(result[x], result[x + 1]));

        }
        return values.ToArray();
      }
    }

    
    public string[] Keys
    {
      get
      {        
        return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.HKEYS, _name));
      }
    }

    
    public string[] Values
    {
      get
      {        
        return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.HVALS, _name));
      }
    }

    
    public bool ContainsKey(string key)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.HEXISTS, _name, key)) == 1;
    }

    
    public bool Delete(string field)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.HDEL, _name, field)) == 1;
    }

    
    public void Set(IDictionary<string, string> datas)
    {      
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.HMSET, datas, _name));
    }

    
    public string[] Get(params string[] keys)
    {
      List<string> args = new List<string>();
      args.Add(_name);
      args.AddRange(keys);
      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.HMGET, args.ToArray()));
    }

    
    public int Lenght
    {
      get
      {        
        return _provider.ReadInt(_provider.SendCommand(RedisCommand.HLEN, _name));
      }
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
