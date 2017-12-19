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
    [Description(CommandDescriptions.ZADD)]
    public bool Add(string score, string member)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZADD, _name, score, member)) == 1;
    }

    [Description(CommandDescriptions.ZADD)]
    public bool Add(double score, string member)
    {
      
      return _provider.ReadInt(
             _provider.SendCommand(RedisCommand.ZADD, _name, score.ToString(System.Globalization.CultureInfo.InvariantCulture), member)) == 1;
    }


    [Description(CommandDescriptions.ZCARD)]
    public int Cardinality
    {
      get
      {        
        return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZCARD, _name));
      }
    }

    [Description(CommandDescriptions.ZCOUNT)]
    public int Count(string min, string max)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZCOUNT, _name, min, max));
    }

    [Description(CommandDescriptions.ZINCRBY)]
    public string[] IncrementBy(string member, int incrementvalue)
    {      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.ZINCRBY, _name, incrementvalue.ToString(), member));
    }

    [Description(CommandDescriptions.ZRANGE)]
    public string[] Range(string min, string max)
    {      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.ZRANGE, _name, min, max));
    }

    [Description(CommandDescriptions.ZRANK)]
    public int Rank(string member)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.ZRANK, _name, member));
    }

    [Description(CommandDescriptions.DEL)]
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
