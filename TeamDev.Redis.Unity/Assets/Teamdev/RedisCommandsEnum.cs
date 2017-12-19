using System;
using System.Collections.Generic;

using System.Text;

namespace TeamDev.Redis
{
  public enum RedisCommand
  {
    APPEND,// key value Append a value to a key 

    AUTH,//password Authenticate to the server 

    BGREWRITEAOF,//Asynchronously rewrite the append-only file 

    BGSAVE,//Asynchronously save the dataset to disk 

    BITCOUNT, // key [start end]
    BITFIELD, // key [GET type offset] [SET type offset value] [INCRBY type offset increment] [OVERFLOW WRAP|SAT|FAIL]
    BITOP, // operation destkey key [key...]
    BITPOS, // key bit [start] [end]

    BLPOP,//key [key ...] timeout Remove and get the first element in a list, or block until one is available 

    BRPOP,//key [key ...] timeout Remove and get the last element in a list, or block until one is available 

    BRPOPLPUSH,//source destination timeout Pop a value from a list, push it to another list and return it; or block until one is available 

    CLIENT_GETNAME,
    CLIENT_KILL, //  [ip:port] [ID client-id] [TYPE normal|master|slave|pubsub] [ADDR ip:port] [SKIPME yes/no]
    CLIENT_LIST,
    CLIENT_PAUSE, //  timeout
    CLIENT_SETNAME, // connection-name

    COMMAND,
    COMMAND_COUNT,
    COMMAND_GETKEYS,
    COMMAND_INFO, 


    CONFIG_GET,//parameter Get the value of a configuration parameter 

    CONFIG_REWRITE, 

    CONFIG_RESETSTAT,//Reset the stats returned by INFO 
    CONFIG_SET,//parameter value Set a configuration parameter to the given value 

    

    DBSIZE,//Return the number of keys in the selected database 

    DEBUG_OBJECT,//key Get debugging information about a key 

    DEBUG_SEGFAULT,//Make the server crash 

    DECR,//key Decrement the integer value of a key by one 

    DECRBY,//key decrement Decrement the integer value of a key by the given number 

    DEL,//key [key ...] Delete a key 

    DUMP, // Serialize the value stored at key in a Redis-specific format and return it to the user. 

    DISCARD,//Discard all commands issued after MULTI 

    ECHO,//message Echo the given string 

    EVAL, // script numkeys key [key...] arg [arg...]
    EVALSHA, // sha1 numkeys key [key...] arg [arg...]


    EXEC,//Execute all commands issued after MULTI 

    EXISTS,//key Determine if a key exists 

    EXPIRE,//key seconds Set a key's time to live in seconds 

    EXPIREAT,//key timestamp Set the expiration for a key as a UNIX timestamp 

    FLUSHALL,//Remove all keys from all databases 

    FLUSHDB,//Remove all keys from the current database 


    GEOADD, // key longitude latitude member [longitude latitude member...]
    GEODIST, //  key member1 member2 [unit]
    GEOHASH, // key member [member...]
    GEOPOS, //  key member [member...]
    GEORADIUS, // key longitude latitude radius m|km|ft|mi [WITHCOORD] [WITHDIST] [WITHHASH] [COUNT count] [ASC|DESC] [STORE key] [STOREDIST key]
    GEORADIUSBYMEMBER, // key member radius m|km|ft|mi [WITHCOORD] [WITHDIST] [WITHHASH] [COUNT count] [ASC|DESC] [STORE key] [STOREDIST key]

    GET,//key Get the value of a key 

    GETBIT,//key offset Returns the bit value at offset in the string value stored at key 

    GETRANGE,//key start end Get a substring of the string stored at a key 

    GETSET,//key value Set the string value of a key and return its old value 

    HDEL,//key field Delete a hash field 

    HEXISTS,//key field Determine if a hash field exists 

    HGET,//key field Get the value of a hash field 

    HGETALL,//key Get all the fields and values in a hash 

    HINCRBY,//key field increment Increment the integer value of a hash field by the given number 
    HINCRBYFLOAT, // key field increment

    HKEYS,//key Get all the fields in a hash 

    HLEN,//key Get the number of fields in a hash 

    HMGET,//key field [field ...] Get the values of all the given hash fields 

    HMSET,//key field value [field value ...] Set multiple hash fields to multiple values 

    HSCAN, // key cursor [MATCH pattern] [COUNT count]

    HSET,//key field value Set the string value of a hash field 

    HSETNX,//key field value Set the value of a hash field, only if the field does not exist 

    HSTRLEN, // key field

    HVALS,//key Get all the values in a hash 

    INCR,//key Increment the integer value of a key by one 

    INCRBY,//key increment Increment the integer value of a key by the given number 
    INCRBYFLOAT, // key increment

    INFO,//Get information and statistics about the server 

    KEYS,//pattern Find all keys matching the given pattern 

    LASTSAVE,//Get the UNIX time stamp of the last successful save to disk 

    LINDEX,//key index Get an element from a list by its index 

    LINSERT,//key BEFORE|AFTER pivot value Insert an element before or after another element in a list 

    LLEN,//key Get the length of a list 

    LPOP,//key Remove and get the first element in a list 

    LPUSH,//key value Prepend a value to a list 

    LPUSHX,//key value Prepend a value to a list, only if the list exists 

    LRANGE,//key start stop Get a range of elements from a list 

    LREM,//key count value Remove elements from a list 

    LSET,//key index value Set the value of an element in a list by its index 

    LTRIM,//key start stop Trim a list to the specified range 

    MGET,//key [key ...] Get the values of all the given keys 

    MIGRATE, // host port key - Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance.

    MONITOR,//Listen for all requests received by the server in real time 

    MOVE,//key db Move a key to another database 

    MSET,//key value [key value ...] Set multiple keys to multiple values 

    MSETNX,//key value [key value ...] Set multiple keys to multiple values, only if none of the keys exist 

    MULTI,//Mark the start of a transaction block 

    OBJECT,//subcommand [arguments [arguments ...]] Inspect the internals of Redis objects 

    PERSIST,//key Remove the expiration from a key 

    PEXPIRE, // key milliseconds

    PEXPIREAT, // key milliseconds-timestamp

    PFADD, // key element [element...]

    PFCOUNT, // key [key...]

    PFMERGE, // destkey sourcekey [sourcekey...]

    PTTL, // key

    PING,//Ping the server 
    PSETEX, // key milliseconds value

    PSUBSCRIBE,//pattern [pattern ...] Listen for messages published to channels matching the given patterns 

    PUBSUB, // subcommand [argument [argument...]]

    PUBLISH,//channel message Post a message to a channel 

    PUNSUBSCRIBE,//[pattern [pattern ...]] Stop listening for messages posted to channels matching the given patterns 

    QUIT,//Close the connection 

    RANDOMKEY,//Return a random key from the keyspace 

    RENAME,//key newkey Rename a key 

    RENAMENX,//key newkey Rename a key, only if the new key does not exist 

    RESTORE, // key ttl serialized-value [REPLACE]
    ROLE, 

    RPOP,//key Remove and get the last element in a list 

    RPOPLPUSH,//source destination Remove the last element in a list, append it to another list and return it 

    RPUSH,//key value Append a value to a list 

    RPUSHX,//key value Append a value to a list, only if the list exists 

    SADD,//key member Add a member to a set 

    SAVE,//Synchronously save the dataset to disk 

    SCARD,//key Get the number of members in a set 
    SCAN, // cursor [MATCH pattern] [COUNT count]

    SCRIPT, // DEBUG YES|SYNC|NO
            // EXISTS sha1 [sha1...]
            // FLUSH
            // KILL
            // LOAD

    SDIFF,//key [key ...] Subtract multiple sets 

    SDIFFSTORE,//destination key [key ...] Subtract multiple sets and store the resulting set in a key 

    SELECT,//index Change the selected database for the current connection 

    SET,//key value Set the string value of a key 

    SETBIT,//key offset value Sets or clears the bit at offset in the string value stored at key 

    SETEX,//key seconds value Set the value and expiration of a key 

    SETNX,//key value Set the value of a key, only if the key does not exist 

    SETRANGE,//key offset value Overwrite part of a string at key starting at the specified offset 

    SHUTDOWN,//Synchronously save the dataset to disk and then shut down the server 

    SINTER,//key [key ...] Intersect multiple sets 

    SINTERSTORE,//destination key [key ...] Intersect multiple sets and store the resulting set in a key 

    SISMEMBER,//key member Determine if a given value is a member of a set 

    SLAVEOF,//host port Make the server a slave of another instance, or promote it as master 
    SLOWLOG, 

    SMEMBERS,//key Get all the members in a set 

    SMOVE,//source destination member Move a member from one set to another 

    SORT,//key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination] Sort the elements in a list, set or sorted set 

    SPOP,//key Remove and return a random member from a set 

    SRANDMEMBER,//key Get a random member from a set 

    SREM,//key member Remove a member from a set 
    SSCAN, // key cursor [MATCH pattern] [COUNT count]
    STRLEN,//key Get the length of the value stored in a key 

    SUBSCRIBE,//channel [channel ...] Listen for messages published to the given channels 

    SUNION,//key [key ...] Add multiple sets 

    SUNIONSTORE,//destination key [key ...] Add multiple sets and store the resulting set in a key 

    SWAPDB, // index index

    SYNC,//Internal command used for replication 
    TIME, 
    TOUCH,// key [key...]

    TTL,//key Get the time to live for a key 

    TYPE,//key Determine the type stored at key 

    UNLINK, // key [key...]

    UNSUBSCRIBE,//[channel [channel ...]] Stop listening for messages posted to the given channels 

    UNWATCH,//Forget about all watched keys 

    WAIT, // numslaves timeout

    WATCH,//key [key ...] Watch the given keys to determine execution of the MULTI/EXEC block 

    ZADD,//key score member Add a member to a sorted set, or update its score if it already exists 

    ZCARD,//key Get the number of members in a sorted set 

    ZCOUNT,//key min max Count the members in a sorted set with scores within the given values 

    ZINCRBY,//key increment member Increment the score of a member in a sorted set 

    ZINTERSTORE,//destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX] Intersect multiple sorted sets and store the resulting sorted set in a new key 
    ZLEXCOUNT, //  key min max

    ZRANGE,//key start stop [WITHSCORES] Return a range of members in a sorted set, by index 
    ZRANGEBYLEX, // key min max [LIMIT offset count]
    ZRANGEBYSCORE,//key min max [WITHSCORES] [LIMIT offset count] Return a range of members in a sorted set, by score 

    ZRANK,//key member Determine the index of a member in a sorted set 

    ZREM,//key member Remove a member from a sorted set 


    ZREMRANGEBYLEX, // key min max
    ZREMRANGEBYRANK,//key start stop Remove all members in a sorted set within the given indexes 
    ZREMRANGEBYSCORE,//key min max Remove all members in a sorted set within the given scores 

    ZREVRANGE,//key start stop [WITHSCORES] Return a range of members in a sorted set, by index, with scores ordered from high to low 
    ZREVRANGEBYLEX, // key max min [LIMIT offset count]
    ZREVRANGEBYSCORE,//key max min [WITHSCORES] [LIMIT offset count] Return a range of members in a sorted set, by score, with scores ordered from high to low 
    ZREVRANK,//key member Determine the index of a member in a sorted set, with scores ordered from high to low 

    ZSCORE,//key member Get the score associated with the given member in a sorted set 
    ZSCAN, // key cursor [MATCH pattern] [COUNT count]
    ZUNIONSTORE,//destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX] Add multiple sorted sets and store the resulting sorted set in a new key 
  }
}
