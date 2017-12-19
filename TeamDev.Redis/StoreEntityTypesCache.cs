using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;

namespace TeamDev.Redis
{
  public static class StoreEntityTypesCache
  {
    private static Dictionary<Type, Dictionary<string, PropertyInfo>> _typesProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    private static Dictionary<Type, Dictionary<string, PropertyInfo>> _indexedProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
    private static Dictionary<Type, Dictionary<string, PropertyInfo>> _partialvalues = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    private static Dictionary<Type, PropertyInfo> _keyproperties = new Dictionary<Type, PropertyInfo>();

    public static Dictionary<string, PropertyInfo> GetTypeProperties(Type itemtype)
    {
      if (!_typesProperties.ContainsKey(itemtype))
        PrepareType(itemtype);

      return _typesProperties[itemtype];
    }

    public static Dictionary<string, PropertyInfo> GetTypePartialValueProperties(Type itemtype)
    {
      if (!_partialvalues.ContainsKey(itemtype))
        PrepareType(itemtype);

      return _partialvalues[itemtype];
    }

    public static PropertyInfo GetTypeKey(Type itemtype)
    {
      if (!_keyproperties.ContainsKey(itemtype))
        PrepareType(itemtype);

      return _keyproperties[itemtype];
    }

    public static Dictionary<string, PropertyInfo> GetTypeIndexes(Type itemtype)
    {
      if (!_indexedProperties.ContainsKey(itemtype))
        PrepareType(itemtype);

      return _indexedProperties[itemtype];
    }

    public static void PrepareType(Type itemtype)
    {
      lock (_typesProperties)
      {
        if (!_typesProperties.ContainsKey(itemtype))
        {

          var keys = new Dictionary<string, PropertyInfo>();

          foreach (var p in itemtype.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
            keys.Add(p.Name, p);

          _typesProperties.Add(itemtype, keys);

          foreach (var pi in _typesProperties[itemtype].Values)
          {
            // Search for property marked as key
            var result = pi.GetCustomAttributes(typeof(DocumentStoreKeyAttribute), true);
            if (result != null && result.Length > 0)
            {
              if (_keyproperties.ContainsKey(itemtype))
                throw new InvalidOperationException(string.Format("Entity {0} has more than 1 property marked with DocumentStoreKey attribute.", itemtype.FullName));
              _keyproperties.Add(itemtype, pi);
            }

            if (!_indexedProperties.ContainsKey(itemtype))
              _indexedProperties.Add(itemtype, new Dictionary<string, PropertyInfo>());

            // Search for indexable properties
            result = pi.GetCustomAttributes(typeof(DocumentStoreIndexAttribute), true);
            if (result != null && result.Length > 0)
              _indexedProperties[itemtype].Add(pi.Name, pi);

            // Search for Partial Values properties
            if (!_partialvalues.ContainsKey(itemtype))
              _partialvalues.Add(itemtype, new Dictionary<string, PropertyInfo>());

            result = pi.GetCustomAttributes(typeof(DocumentValueAttribute), true);
            if (result != null && result.Length > 0)
              _partialvalues[itemtype].Add(pi.Name, pi);
          }


          // Check that entity has a property defined with DocumentStoreKey attribute
          if (!_keyproperties.ContainsKey(itemtype))
            throw new InvalidOperationException(string.Format("Entity {0} must have one property marked with DocumentStoreKey attribute.", itemtype.FullName));
        }
      }
    }
  }
}
