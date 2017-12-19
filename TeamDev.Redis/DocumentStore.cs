using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;

namespace TeamDev.Redis
{
  public class DocumentStore
  {
    public static string Host { get; set; }
    public static int HostPort { get; set; }

    public static DocumentStore New { get { return new DocumentStore(); } }
    public Redis.RedisDataAccessProvider Provider { get; private set; }

    static DocumentStore()
    {
      Host = "localhost";
      HostPort = 6379;
    }

    public DocumentStore()
    {
      Provider = new Redis.RedisDataAccessProvider();
      Provider.Configuration.Host = DocumentStore.Host;
      Provider.Configuration.Port = DocumentStore.HostPort;
    }

    public virtual void Set<T>(IEnumerable<T> items)
    {
      foreach (var item in items)
        Set<T>(item);
    }

    [Description("Persist the entire object in the Document Store")]
    public virtual void Set<T>(T item)
    {
      var key = GetItemKey(item);

      // Write value
      Provider.Hash[key].Set(Serialize(item));

      // Write indexes
      var keyvalue = GetStringValue(StoreEntityTypesCache.GetTypeKey(typeof(T)).GetValue(item, null));

      var indexes = StoreEntityTypesCache.GetTypeIndexes(typeof(T));
      if (indexes != null)
        foreach (var idx in indexes.Values)
          Provider.Set[GetIndexKey(item, idx)].Add(keyvalue);
    }

    [Description("Retrieve the entire object from the Document Store")]
    public virtual T Get<T>(string keyvalue, bool computekey = true) where T : new()
    {
      var key = computekey ? GetItemKey<T>(keyvalue) : keyvalue;
      var values = Provider.Hash[key].Items;
      var newitem = Deserialize<T>(values);
      return newitem;
    }

    public virtual void PartialSet<T>(IEnumerable<T> items)
    {
      foreach (var item in items)
        PartialSet<T>(item);
    }

    [Description("Persist properties marked with DocumentValueAttribure")]
    public virtual void PartialSet<T>(T item)
    {
      var key = GetItemKey(item);
      Provider.Hash[key].Set(Serialize(item, true));
    }

    [Description("")]
    public virtual void PartialGet<T>(T item)
    {
      var key = GetItemKey<T>(item);
      var values = Provider.Hash[key].Items;
      InternalDeserializeValue(item, values, true);
    }

    #region De/Serialize

    private T Deserialize<T>(KeyValuePair<string, string>[] values) where T : new()
    {
      var newitem = new T();
      InternalDeserializeValue<T>(newitem, values, false);
      return newitem;
    }

    private void InternalDeserializeValue<T>(T newitem, KeyValuePair<string, string>[] values, bool partial)
    {
      if (values != null)
      {
        var properties = partial ?
            StoreEntityTypesCache.GetTypePartialValueProperties(typeof(T)) :
            StoreEntityTypesCache.GetTypeProperties(typeof(T));

        if (properties != null)
        {
          foreach (var kvp in values)
          {
            if (!string.IsNullOrEmpty(kvp.Value) && properties.ContainsKey(kvp.Key))
            {
              var pi = properties[kvp.Key];
              pi.SetValue(newitem, ConvertTo(pi.PropertyType, kvp.Value), null);
            }
          }
        }
      }
    }

    private object ConvertTo(Type type, string value)
    {
      if (string.IsNullOrEmpty(value)) return null;

      // Is a generico of a valuetype. Probably is a nullable
      if (type.IsGenericType && type.IsValueType)
        type = type.GetGenericArguments()[0];

      if (type == typeof(Guid)) return new Guid(value);
      if (type == typeof(DateTime) || type == typeof(Nullable<DateTime>)) return DateTime.Parse(value, CultureInfo.InvariantCulture);
      if (type == typeof(string)) return value;

      if (type.IsValueType)
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

      return null;
    }

    private static string GetStringValue(object value)
    {
      if (value is string)
        return (string)value;

      if (value is Guid)
        return value.ToString();

      if (value is char)
        return value.ToString();

      if (value is Int16)
        return ((Int16)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is Int32)
        return ((Int32)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is Int64)
        return ((Int64)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is Double)
        return ((Double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is float)
        return ((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is decimal)
        return ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is UInt16)
        return ((UInt16)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is UInt32)
        return ((UInt32)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is UInt64)
        return ((UInt64)value).ToString(System.Globalization.CultureInfo.InvariantCulture);

      if (value is bool)
        return value.ToString().ToLower();

      if (value is DateTime)
        return ((DateTime)value).ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);

      return value.ToString();
    }

    private Dictionary<string, string> Serialize<T>(T item, bool partial = false)
    {
      var result = new Dictionary<string, string>();

      var properties = partial ?
          StoreEntityTypesCache.GetTypePartialValueProperties(typeof(T)) :
          StoreEntityTypesCache.GetTypeProperties(typeof(T));

      if (properties != null)
        foreach (var pi in properties.Values)
          result.Add(pi.Name, GetStringValue((pi.GetValue(item, null) ?? string.Empty)).ToString());

      return result;
    }
    #endregion

    public virtual string GetItemKey<T>(T item)
    {
      if (item == null) throw new ArgumentNullException("Item cannot be null");

      var pi = StoreEntityTypesCache.GetTypeKey(typeof(T));
      var key = string.Format("[{0}_{1}:{2}]", typeof(T).Name, pi.Name, GetStringValue(pi.GetValue(item, null)));
      return key;
    }

    public virtual string GetIndexKey<T>(string fieldname, string value)
    {
      return string.Format("[IX_{0}_{1}:{2}]", typeof(T).Name, fieldname, value);
    }

    public virtual string GetIndexKey<T>(T item, PropertyInfo field)
    {
      return string.Format("[IX_{0}_{1}:{2}]", typeof(T).Name, field.Name, GetStringValue(field.GetValue(item, null)));
    }

    public virtual string GetItemKey<T>(string value)
    {
      var pi = StoreEntityTypesCache.GetTypeKey(typeof(T));
      var key = string.Format("[{0}_{1}:{2}]", typeof(T).Name, pi.Name, value);
      return key;
    }
  }
}
