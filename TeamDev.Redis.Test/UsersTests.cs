using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace TeamDev.Redis.Test
{
  /// <summary>
  /// Summary description for UsersTests
  /// </summary>
  [TestClass]
  public class UsersTests
  {
    private RedisDataAccessProvider _redis = new RedisDataAccessProvider();

    public UsersTests()
    {
      _redis.Configuration.Host = "127.0.0.1";
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void TestPUBSUB()
    {
      RedisDataAccessProvider pp = new RedisDataAccessProvider();

      pp.ChannelSubscribed += new ChannelSubscribedHandler(pp_ChannelSubscribed);
      pp.MessageReceived += new MessageReceivedHandler(pp_MessageReceived);

      pp.Messaging.Subscribe("message");

      pp.Messaging.Publish("message", "prova1");
      pp.Messaging.Publish("message", "prova2");
      pp.Messaging.Publish("message", "prova3");
      pp.Messaging.Publish("message", "prova4");
      pp.Messaging.Publish("message", "prova5");
      pp.Messaging.Publish("message", "prova6");

      Thread.Sleep(1000000);
    }

    void pp_MessageReceived(string channelname, string message)
    {
      Debug.WriteLine(string.Format("{0} - {1} ", channelname, message));
    }

    void pp_ChannelSubscribed(string channelname)
    {
      Debug.WriteLine(channelname);
    }

    [TestMethod]
    public void TestPUBSUB_Bug1()
    {
      RedisDataAccessProvider pp = new RedisDataAccessProvider();
      RedisDataAccessProvider.ActivateDebugLog = true;

      pp.ChannelSubscribed += new ChannelSubscribedHandler(pp_ChannelSubscribed);
      pp.MessageReceived += new MessageReceivedHandler(pp_MessageReceived);

      pp.Messaging.Subscribe("message", "message2");

      pp.Messaging.Publish("message", "test1");
      pp.Messaging.Publish("message", "test2");

      Thread.Sleep(1000000);

    }

    [TestMethod]
    public void ExistsAfter_DEL_SREM_Bug_SIMULATION1()
    {
      _redis.Configuration.LogUnbalancedCommands = true;

      _redis.WaitComplete(_redis.SendCommand(RedisCommand.FLUSHALL));

      _redis.Key.Exists("TEST");

      Dictionary<string, string> _values = new Dictionary<string, string>();
      _values.Add("v1", "testvalue1");
      _values.Add("v2", "testvalue2");
      _values.Add("v3", "testvalue3");
      _values.Add("v4", "testvalue4");

      _redis.Hash["TEST"].Set(_values);

      _redis.Key.Exists("TEST");
    }

    [TestMethod]
    public void ExistsAfter_DEL_SREM_Bug_SIMULATION2()
    {
      _redis.Configuration.LogUnbalancedCommands = true;
      _redis.WaitComplete(_redis.SendCommand(RedisCommand.FLUSHALL));


      _redis.Key.Exists("TEST");
      _redis.Set["MySet"].Add("setvalue1");
      _redis.Set["MySet"].Add("setvalue1");
      _redis.Set["MySet"].Add("setvalue2");
      _redis.Set["MySet"].Add("setvalue3");
      _redis.Set["MySet"].Add("setvalue4");

      Dictionary<string, string> _values = new Dictionary<string, string>();
      _values.Add("v1", "testvalue1");
      _values.Add("v2", "testvalue2");
      _values.Add("v3", "testvalue3");
      _values.Add("v4", "testvalue4");

      _redis.Hash["TEST"].Set(_values);

      _redis.Hash["TEST"].Get("v1", "v2", "v3", "v4");

      _redis.Key.Exists("TEST");

      foreach (var v in _redis.Set["MySet"].Members)
      {
        _redis.Set["MySet"].Remove(v);
      }

      _redis.Key.Exists("TEST");
      _redis.Key.Exists("MySet");

      _redis.Set["MySet"].Clear();

      _redis.Key.Exists("MySet");
    }

    [TestMethod]
    public void ExistsAfter_DEL_SREM_Bug_SIMULATION3()
    {
      _redis.Configuration.LogUnbalancedCommands = true;
      _redis.WaitComplete(_redis.SendCommand(RedisCommand.FLUSHALL));

      Random rnd = new Random();

      for (int x = 0; x < 10000; x++)
      {

        string key = "MySet" + x.ToString();
        string key2 = "TEST" + x.ToString();

        _redis.Key.Exists(key2);

        _redis.Set[key].Add("setvalue1");
        _redis.Set[key].Add("setvalue1");
        _redis.Set[key].Add("setvalue2");
        _redis.Set[key].Add("setvalue3");
        _redis.Set[key].Add("setvalue4");

        Dictionary<string, string> _values = new Dictionary<string, string>();
        _values.Add("v1", "testvalue1");
        _values.Add("v2", "testvalue2");
        _values.Add("v3", "testvalue3");
        _values.Add("v4", "<372189371,389217838921,3218372189>");

        _redis.Hash[key2].Set(_values);

        _redis.Hash[key2].Get("v1", "v2", "v3", "v4");

        Thread.Sleep(rnd.Next(30 * 1000));

        _redis.Key.Exists(key2);

        foreach (var v in _redis.Set[key].Members)
        {
          _redis.Set[key].Remove(v);
          _redis.Set[key].Remove(v);
          _redis.Key.Remove(key2);
        }

        Thread.Sleep(rnd.Next(30 * 1000));
        _redis.Key.Exists(key2);

        Thread.Sleep(rnd.Next(30 * 1000));
        _redis.Key.Exists(key);

        _redis.Set[key].Clear();
        _redis.Key.Remove(key2);

        Thread.Sleep(rnd.Next(30 * 1000));
        _redis.Key.Exists(key);

        Debug.WriteLine(" Now at position : " + x.ToString());
      }
    }
  }
}
