using System;
using System.Collections.Generic;

using System.Text;

namespace TeamDev.Redis
{
  internal class CommandDescriptions
  {

    internal const string APPEND = "Append a value to a key";

    internal const string AUTH = "Authenticate to the server ";

    internal const string BGREWRITEAOF = "Asynchronously rewrite the append-only file ";

    internal const string BGSAVE = "Asynchronously save the dataset to disk ";

    internal const string BLPOP = "Remove and get the first element in a list, or block until one is available ";

    internal const string BRPOP = "Remove and get the last element in a list, or block until one is available ";

    internal const string BRPOPLPUSH = "Pop a value from a list, push it to another list and return it; or block until one is available ";

    internal const string CONFIG_GET = "Get the value of a configuration parameter ";

    internal const string CONFIG_SET = "Set a configuration parameter to the given value ";

    internal const string CONFIG_RESETSTAT = "Reset the stats returned by INFO ";

    internal const string DBSIZE = "Return the number of keys in the selected database ";

    internal const string DEBUG_OBJECT = "Get debugging information about a key ";

    internal const string DEBUG_SEGFAULT = "Make the server crash ";

    internal const string DECR = "Decrement the integer value of a key by one ";

    internal const string DECRBY = "Decrement the integer value of a key by the given number ";

    internal const string DEL = "Delete a key ";

    internal const string DISCARD = "Discard all commands issued after MULTI ";

    internal const string ECHO = "Echo the given string ";

    internal const string EXEC = "Execute all commands issued after MULTI ";

    internal const string EXISTS = "Determine if a key exists ";

    internal const string EXPIRE = "Set a key's time to live in seconds ";

    internal const string EXPIREAT = "Set the expiration for a key as a UNIX timestamp ";

    internal const string FLUSHALL = "Remove all keys from all databases ";

    internal const string FLUSHDB = "Remove all keys from the current database ";

    internal const string GET = "Get the value of a key ";

    internal const string GETBIT = "Returns the bit value at offset in the string value stored at key ";

    internal const string GETRANGE = "Get a substring of the string stored at a key ";

    internal const string GETSET = "Set the string value of a key and return its old value ";

    internal const string HDEL = "Delete a hash field ";

    internal const string HEXISTS = "Determine if a hash field exists ";

    internal const string HGET = "Get the value of a hash field ";

    internal const string HGETALL = "Get all the fields and values in a hash ";

    internal const string HINCRBY = "Increment the integer value of a hash field by the given number ";

    internal const string HKEYS = "Get all the fields in a hash ";

    internal const string HLEN = "Get the number of fields in a hash ";

    internal const string HMGET = "Get the values of all the given hash fields ";

    internal const string HMSET = "Set multiple hash fields to multiple values ";

    internal const string HSET = "Set the string value of a hash field ";

    internal const string HSETNX = "Set the value of a hash field, only if the field does not exist ";

    internal const string HVALS = "Get all the values in a hash ";

    internal const string INCR = "Increment the integer value of a key by one ";

    internal const string INCRBY = "Increment the integer value of a key by the given number ";

    internal const string INFO = "Get information and statistics about the server ";

    internal const string KEYS = "Find all keys matching the given pattern ";

    internal const string LASTSAVE = "Get the UNIX time stamp of the last successful save to disk ";

    internal const string LINDEX = "Get an element from a list by its index ";

    internal const string LINSERT = "Insert an element before or after another element in a list ";

    internal const string LLEN = "Get the length of a list ";

    internal const string LPOP = "Remove and get the first element in a list ";

    internal const string LPUSH = "Prepend a value to a list ";

    internal const string LPUSHX = "Prepend a value to a list, only if the list exists ";

    internal const string LRANGE = "Get a range of elements from a list ";

    internal const string LREM = "Remove elements from a list ";

    internal const string LSET = "Set the value of an element in a list by its index ";

    internal const string LTRIM = "Trim a list to the specified range ";

    internal const string MGET = "Get the values of all the given keys ";

    internal const string MONITOR = "Listen for all requests received by the server in real time ";

    internal const string MOVE = "Move a key to another database ";

    internal const string MSET = "Set multiple keys to multiple values ";

    internal const string MSETNX = "Set multiple keys to multiple values, only if none of the keys exist ";

    internal const string MULTI = "Mark the start of a transaction block ";

    internal const string OBJECT = "Inspect the internals of Redis objects ";

    internal const string PERSIST = "Remove the expiration from a key ";

    internal const string PING = "Ping the server ";

    internal const string PSUBSCRIBE = "Listen for messages published to channels matching the given patterns ";

    internal const string PUBLISH = "Post a message to a channel ";

    internal const string PUNSUBSCRIBE = "Stop listening for messages posted to channels matching the given patterns ";

    internal const string QUIT = "Close the connection ";

    internal const string RANDOMKEY = "Return a random key from the keyspace ";

    internal const string RENAME = "Rename a key ";

    internal const string RENAMENX = "Rename a key, only if the new key does not exist ";

    internal const string RPOP = "Remove and get the last element in a list ";

    internal const string RPOPLPUSH = "Remove the last element in a list, append it to another list and return it ";

    internal const string RPUSH = "Append a value to a list ";

    internal const string RPUSHX = "Append a value to a list, only if the list exists ";

    internal const string SADD = "Add a member to a set ";

    internal const string SAVE = "Synchronously save the dataset to disk ";

    internal const string SCARD = "Get the number of members in a set ";

    internal const string SDIFF = "Subtract multiple sets ";

    internal const string SDIFFSTORE = "Subtract multiple sets and store the resulting set in a key ";

    internal const string SELECT = "Change the selected database for the current connection ";

    internal const string SET = "Set the string value of a key ";

    internal const string SETBIT = "Sets or clears the bit at offset in the string value stored at key ";

    internal const string SETEX = "Set the value and expiration of a key ";

    internal const string SETNX = "Set the value of a key, only if the key does not exist ";

    internal const string SETRANGE = "Overwrite part of a string at key starting at the specified offset ";

    internal const string SHUTDOWN = "Synchronously save the dataset to disk and then shut down the server ";

    internal const string SINTER = "Intersect multiple sets ";

    internal const string SINTERSTORE = "Intersect multiple sets and store the resulting set in a key ";

    internal const string SISMEMBER = "Determine if a given value is a member of a set ";

    internal const string SLAVEOF = "Make the server a slave of another instance, or promote it as master ";

    internal const string SMEMBERS = "Get all the members in a set ";

    internal const string SMOVE = "Move a member from one set to another ";

    internal const string SORT = "Sort the elements in a list, set or sorted set ";

    internal const string SPOP = "Remove and return a random member from a set ";

    internal const string SRANDMEMBER = "Get a random member from a set ";

    internal const string SREM = "member Remove a member from a set ";

    internal const string STRLEN = "Get the length of the value stored in a key ";

    internal const string SUBSCRIBE = "Listen for messages published to the given channels ";

    internal const string SUNION = "Add multiple sets ";

    internal const string SUNIONSTORE = "Add multiple sets and store the resulting set in a key ";

    internal const string SYNC = "Internal command used for replication ";

    internal const string TTL = "Get the time to live for a key ";

    internal const string TYPE = "Determine the type stored at key ";

    internal const string UNSUBSCRIBE = "Stop listening for messages posted to the given channels ";

    internal const string UNWATCH = "Forget about all watched keys ";

    internal const string WATCH = "Watch the given keys to determine execution of the MULTI/EXEC block ";

    internal const string ZADD = "Add a member to a sorted set, or update its score if it already exists ";

    internal const string ZCARD = "Get the number of members in a sorted set ";

    internal const string ZCOUNT = "Count the members in a sorted set with scores within the given values ";

    internal const string ZINCRBY = "Increment the score of a member in a sorted set ";

    internal const string ZINTERSTORE = "Intersect multiple sorted sets and store the resulting sorted set in a new key ";

    internal const string ZRANGE = "Return a range of members in a sorted set, by index ";

    internal const string ZRANGEBYSCORE = "Return a range of members in a sorted set, by score ";

    internal const string ZRANK = "Determine the index of a member in a sorted set ";

    internal const string ZREM = "Remove a member from a sorted set ";

    internal const string ZREMRANGEBYRANK = "Remove all members in a sorted set within the given indexes ";

    internal const string ZREMRANGEBYSCORE = "Remove all members in a sorted set within the given scores ";

    internal const string ZREVRANGE = "Return a range of members in a sorted set, by index, with scores ordered from high to low ";

    internal const string ZREVRANGEBYSCORE = "Return a range of members in a sorted set, by score, with scores ordered from high to low ";

    internal const string ZREVRANK = "Determine the index of a member in a sorted set, with scores ordered from high to low ";

    internal const string ZSCORE = "Get the score associated with the given member in a sorted set ";

    internal const string ZUNIONSTORE = "Add multiple sorted sets and store the resulting sorted set in a new key ";

  }
}
