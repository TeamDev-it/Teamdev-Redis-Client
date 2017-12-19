using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;
using TeamDev.Redis.Interface;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageSortedSet : ILanguageItem, IComplexItem
  {
    internal string _name;
    internal RedisDataAccessProvider _provider;

    [Obsolete("Please use the overload with double typed score parameter")]
    
    public bool Add(string score, string member)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZADD, _name, score, member)) == 1;
    }

    
    public bool Add(double score, string member)
    {
      
      return _provider.ReadInt(
             _provider.SendCommand(RedisCommand.ZADD, _name, score.ToString(System.Globalization.CultureInfo.InvariantCulture), member)) == 1;
    }


    
    public int Cardinality
    {
      get
      {        
        return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZCARD, _name));
      }
    }

    
    public int Count(string min, string max)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZCOUNT, _name, min, max));
    }

    
    public string[] IncrementBy(string member, int incrementvalue)
    {      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.ZINCRBY, _name, incrementvalue.ToString(), member));
    }

    
    public string[] Range(string min, string max)
    {      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.ZRANGE, _name, min, max));
    }

    
    public int Rank(string member)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZRANK, _name, member));
    }

    
    public void Clear()
    {      
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.DEL, _name));
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
