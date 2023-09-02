using NRedisStack;
using NRedisStack.RedisStackCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devarc
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisConnection : IDisposable
    {
        public IDatabase Database => mRedis.GetDatabase();
        public ConnectionMultiplexer Multiplexer => mRedis;
        public int Index => mIndex;

        ConnectionMultiplexer mRedis;
        Mutex mMutex;
        int mIndex;

        public RedisConnection(ConnectionMultiplexer redis, int index)
        {
            mMutex = new Mutex(false, "Devarc.RedisServer.Connection");
            mRedis = redis;
            mIndex = index;
            mMutex.WaitOne();
        }

        public void Dispose()
        {
            mRedis.Close();
            mMutex.ReleaseMutex();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class RedisClient
    {
        string mAddress = string.Empty;
        int mPort;

        public RedisConnection GetConnection(int index = 0)
        {
            var option = new ConfigurationOptions
            {
                EndPoints = {
                    { mAddress, mPort },
                },
            };
            var redis = ConnectionMultiplexer.Connect(option);
            var database = redis.GetDatabase(index);
            var connection = new RedisConnection(redis, index);
            return connection;
        }

        public void Init(string address, int port = 6379)
        {
            mAddress = address;
            mPort = port;
        }

        public bool Flush(RedisConnection connection)
        {
            connection.Multiplexer.GetServer(mAddress, mPort).FlushDatabase(connection.Index);
            return true;
        }

        public bool Remove(string key, RedisConnection conn)
        {
            return conn.Database.KeyDelete(key);
        }


        public void SetHash(string key, object obj, RedisConnection connection)
        {
            var value = obj.ToHashEntries();
            connection.Database.HashSet(key, value);
        }

        public bool GetHash(string key, string field, RedisConnection connection, out string value)
        {
            value = string.Empty;
            var result = connection.Database.HashGet(key, field);

            if (result.HasValue)
            {
                value = result.ToString();
                return true;
            }
            {
                return false;
            }
        }


        public void SetString(string key, string value, RedisConnection connection)
        {
            connection.Database.StringSet(key, value);
        }

        public bool GetString(string key, RedisConnection connection, out string value)
        {
            value = string.Empty;
            var result = connection.Database.StringGet(key);
            if (result.HasValue == false)
            {
                return false;
            }
            value = result.ToString();
            return true;
        }



        public void PushContent(string key, string value, RedisConnection connection)
        {
            connection.Database.SetAdd(key, value);
        }

        public IEnumerable<RedisValue> ScanContent(string key, string pattern, RedisConnection connection)
        {
            return connection.Database.SetScan(key, pattern);
        }

        public bool PopContent(string key, RedisConnection connection, out string value)
        {
            value = string.Empty;
            var result = connection.Database.SetPop(key);
            if (result.HasValue)
            {
                value = result.ToString();
                return true;
            }
            {
                return false;
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class RedisUtil
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            //var properties = obj.GetType().GetProperties();
            var properties = obj.GetType().GetFields();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      info =>
                      {
                          object propertyValue = info.GetValue(obj);
                          string hashValue;

                          if (propertyValue is IEnumerable<object>)
                          {
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }
                          return new HashEntry(info.Name, hashValue);
                      }
                )
                .ToArray();
        }
    }

}
