﻿using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;
using TeamDev.Redis.Interface;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageSet : ILanguageItem, IComplexItem
  {
    internal string _name;
    internal RedisDataAccessProvider _provider;

    
    public bool Add(string value)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.SADD, _name, value)) == 1;
    }

    
    public void Clear()
    {      
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.DEL, _name));
    }

    
    public bool IsMember(string value)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.SISMEMBER, _name, value)) == 1;
    }

    
    public string[] Members
    {
      get
      {        
        return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.SMEMBERS, _name));
      }
    }

    
    public void Remove(string value)
    {      
      _provider.WaitComplete(_provider.SendCommand(RedisCommand.SREM, _name, value));
    }

    
    public string[] Union(params string[] sets)
    {
      List<string> args = new List<string>();
      args.Add(_name);
      args.AddRange(sets);
      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.SUNION, args.ToArray()));
    }

    
    public string[] Intersect(params string[] sets)
    {
      List<string> args = new List<string>();
      args.Add(_name);
      args.AddRange(sets);
      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.SINTER, args.ToArray()));
    }

    
    public bool Move(string destination, string value)
    {      
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.SMOVE, _name, destination, value)) == 1;
    }

    
    public string[] Subtract(params string[] sets)
    {
      List<string> args = new List<string>();
      args.Add(_name);
      args.AddRange(sets);
      
      return _provider.ReadMultiString(_provider.SendCommand(RedisCommand.SDIFF, args.ToArray()));
    }

    
    public int Count
    {
      get
      {        
        return _provider.ReadInt(_provider.SendCommand(RedisCommand.SCARD, _name));
      }
    }

    
    public string Pop()
    {      
      return _provider.ReadString(_provider.SendCommand(RedisCommand.SPOP, _name));
    }

    
    public string Random
    {
      get
      {        
        return _provider.ReadString(_provider.SendCommand(RedisCommand.SRANDMEMBER, _name));
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
