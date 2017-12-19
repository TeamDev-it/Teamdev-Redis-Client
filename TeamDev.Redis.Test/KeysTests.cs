using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;

namespace TeamDev.Redis.Test
{
  /// <summary>
  /// Summary description for KeysTests
  /// </summary>
  [TestClass]
  public class KeysTests
  {
    private TeamDev.Redis.RedisDataAccessProvider redis = new RedisDataAccessProvider();

    public KeysTests()
    {
      redis.Configuration.LogUnbalancedCommands = true;
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
    public void SET_Test()
    {
      Guid value = Guid.NewGuid();
      redis.Strings["test"].Set(value.ToString());
      Assert.AreEqual(value.ToString(), redis.Strings["test"].Get());
    }

    [TestMethod]
    public void SET_10000_Test()
    {
      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));

      for (int i = 0; i < 10000; i++)
      {
        Guid value = Guid.NewGuid();
        string key = string.Format("test_{0}", i);
        redis.Strings[key].Set(value.ToString());
        Assert.AreEqual(value.ToString(), redis.Strings[key].Get());
      }

      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));
    }

    [TestMethod]
    public void Exists()
    {
      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));

      Random rnd = new Random();

      for (int i = 0; i < 10000; i++)
      {
        string key = string.Format("test_{0}", i);
        redis.Key.Exists(key);

        Guid value = Guid.NewGuid();

        redis.Strings[key].Set(value.ToString());
        redis.Key.Exists(key);

        Assert.AreEqual(value.ToString(), redis.Strings[key].Get());

        redis.Key.Exists(string.Format("test_{0}", rnd.Next(10000)));
      }

      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));
    }

    [TestMethod]
    public void UnbalancedWithException()
    {
      try
      {
        redis.Configuration.LogUnbalancedCommands = true;
        redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));
        redis.SendCommand(RedisCommand.SET, "ppp", "aaa");
        redis.Key.Exists("ppp");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex.GetType() == typeof(UnbalancedReadException), "Expected UnbalancedReadException but an exception of type " + ex.GetType() + " has throw");
        return;
      }
      Assert.Fail("Expected Exception");
    }

    [TestMethod]
    public void UnbalancedWithOutException()
    {
      try
      {
        redis.Configuration.LogUnbalancedCommands = false;
        redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));
        redis.SendCommand(RedisCommand.SET, "ppp", "aaa");
        redis.Key.Exists("ppp");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex.GetType() == typeof(Exception), "Expected Exception but an exception of type " + ex.GetType() + " has throw");
        return;
      }
      Assert.Fail("Expected Exception");
    }


    [TestMethod]
    public void MultiThead()
    {
      var threads = new List<Thread>();

      redis.Configuration.LogUnbalancedCommands = false;
      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));

      for (int x = 0; x < 3; x++)
      {
        threads.Add(new Thread(() =>
        {
          Random rnd = new Random();

          try
          {
            for (int i = 0; i < 1000; i++)
            {
              Guid value = Guid.NewGuid();

              string key = string.Format("test_{0}", value.ToString());

              var r = new RedisDataAccessProvider();

              r.Key.Exists(key);

              r.Strings[key].Set(value.ToString());
              r.Key.Exists(key);

              Assert.AreEqual(value.ToString(), r.Strings[key].Get());

              r.Key.Exists(string.Format("test_{0}", rnd.Next(10000)));

              Debug.WriteLine(string.Format("Thread {0} -> {1} ", Thread.CurrentThread.ManagedThreadId, i));
            }
          }
          catch (Exception ex)
          {
            Debug.WriteLine(ex.Message);
          }

        }));
      }

      foreach (var t in threads)
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

      foreach (var t in threads)
        t.Start();

      foreach (var t in threads)
        t.Join();

      redis.WaitComplete(redis.SendCommand(RedisCommand.FLUSHALL));
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Debug.WriteLine(((Exception)e.ExceptionObject).Message);
    }
  }
}
