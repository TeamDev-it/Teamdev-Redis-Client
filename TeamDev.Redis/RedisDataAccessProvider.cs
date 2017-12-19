using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using TeamDev.Redis.LanguageItems;
using TeamDev.Redis.Interface;
using System.Threading;
using System.Net.Mail;
using System.ComponentModel;

namespace TeamDev.Redis
{

  public delegate void ChannelSubscribedHandler(string channelname);
  public delegate void ChannelUnsubscribedHandler(string channelname);
  public delegate void MessageReceivedHandler(string channelname, string message);

  public class RedisDataAccessProvider : DataAccessProvider, IDisposable
  {
    private StringBuilder errorlog = new StringBuilder();

    #region private fields
    //private Socket _socket;

    private volatile ReaderWriterLock _socketslock = new ReaderWriterLock();
    private volatile Dictionary<int, Socket> _sockets = new Dictionary<int, Socket>();
    //private volatile Dictionary<int, BufferedStream> _bstreams = new Dictionary<int, BufferedStream>();
    private volatile Dictionary<int, NetworkStream> _bstreams = new Dictionary<int, NetworkStream>();
    private volatile byte[] _end_data = new byte[] { (byte)'\r', (byte)'\n' };
    private volatile CommandTracing _tracer = new CommandTracing();
    private volatile Dictionary<int, DateTime> _lastcommandcompleted = new Dictionary<int, DateTime>();
    private volatile bool _checkingconnectionstatus = false;


    #endregion

    #region Events

    public event ChannelSubscribedHandler ChannelSubscribed;
    public event ChannelUnsubscribedHandler ChannelUnsubscribed;
    public event MessageReceivedHandler MessageReceived;

    #endregion

    #region Properties
    public LanguageItemCollection<LanguageList> List { get; private set; }
    public LanguageItemCollection<LanguageSet> Set { get; private set; }
    public LanguageItemCollection<LanguageSortedSet> SortedSet { get; private set; }
    public LanguageItemCollection<LanguageHash> Hash { get; private set; }
    public LanguageItemCollection<LanguageString> Strings { get; private set; }
    public LanguageKey Key { get; private set; }
    public LanguageTransactions Transaction { get; private set; }
    public LanguageMessaging Messaging { get; private set; }
    public int RenewConnectionPeriod { get; set; }

    public static bool ActivateDebugLog { get; set; }
    #endregion

    public void CheckConnectionStatus()
    {
      var socket = GetSocket();

      if (socket != null)
      {
        socket.Close();
        RemoveSocket();
      }

      //SendCommand(RedisCommand.PING);

      //try
      //{
      //  var nstream = GetBStream();

      //  if (nstream.ReadByte() == -1)
      //  {
      //    GetSocket().Close();
      //    RemoveSocket();        
      //  }
      //}
      //catch (Exception ex)
      //{
      //  GetSocket().Close();
      //  RemoveSocket();
      //}
    }

    #region constructor

    public RedisDataAccessProvider()
      : base()
    {
      base.Configuration.Host = "localhost";
      base.Configuration.Port = 6379;
      base.Configuration.ReceiveTimeout = -1;
      RenewConnectionPeriod = 20;

      List = new LanguageItemCollection<LanguageList>() { Provider = this };
      Set = new LanguageItemCollection<LanguageSet>() { Provider = this };
      SortedSet = new LanguageItemCollection<LanguageSortedSet>() { Provider = this };
      Hash = new LanguageItemCollection<LanguageHash>() { Provider = this };
      this.Strings = new LanguageItemCollection<LanguageString>() { Provider = this };
      Key = new LanguageKey();
      Transaction = new LanguageTransactions();
      Messaging = new LanguageMessaging();

      ((ILanguageItem)Key).Configure(string.Empty, this);
      ((ILanguageItem)Transaction).Configure(string.Empty, this);
      ((ILanguageItem)Messaging).Configure(string.Empty, this);
    }

    #endregion

    #region destructor

    ~RedisDataAccessProvider()
    {
      Dispose(false);
    }

    #endregion

    #region EventRaiseMethods

    internal void RaiseChannelSubscribedEvent(string channelname)
    {
      if (ChannelSubscribed != null) ChannelSubscribed(channelname);
    }

    internal void RaiseChannelUnsubscribedEvent(string channelname)
    {
      if (ChannelUnsubscribed != null) ChannelUnsubscribed(channelname);
    }

    internal void RaiseMessageReceivedEvend(string channelname, string message)
    {
      if (MessageReceived != null) MessageReceived(channelname, message);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        Close();
      }
    }

    #endregion

    #region connection methods
    public override Socket Connect()
    {
      var tid = Thread.CurrentThread.ManagedThreadId;

      if (!_checkingconnectionstatus)
        if (RenewConnectionPeriod > 0)
          if (DateTime.Now.Subtract(GetLastCompletedCommandDate()).TotalSeconds > RenewConnectionPeriod)
          {
            _checkingconnectionstatus = true;
            CheckConnectionStatus();
            _checkingconnectionstatus = false;
          }

      var result = GetSocket();
      if (result != null)
        if (result.Connected)
          return result;

      //_socketslock.AcquireReaderLock(1000);
      //if (_sockets.ContainsKey(tid))
      //{
      //  var result = _sockets[tid];
      //  _socketslock.ReleaseReaderLock();
      //  if (result.Connected)
      //    return result;
      //}

      var newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      newsocket.NoDelay = Configuration.NoDelaySocket;
      if (Configuration.SendTimeout > 0)
        newsocket.SendTimeout = Configuration.SendTimeout;

      if (Configuration.ReceiveTimeout > 0)
        newsocket.ReceiveTimeout = Configuration.ReceiveTimeout;

      newsocket.Connect(Configuration.Host, Configuration.Port);
      if (!newsocket.Connected)
      {
        newsocket.Disconnect(false);
        newsocket.Close();
        return null;
      }

      _socketslock.UpgradeToWriterLock(1000);

      if (_sockets.ContainsKey(tid))
        _sockets[tid] = newsocket;
      else
        _sockets.Add(tid, newsocket);

      if (_bstreams.ContainsKey(tid))
        _bstreams[tid] = new NetworkStream(newsocket);
      //_bstreams[tid] = new BufferedStream(new NetworkStream(newsocket), 16 * 1024);
      else
        _bstreams.Add(tid, new NetworkStream(newsocket));
      //_bstreams.Add(tid, new BufferedStream(new NetworkStream(newsocket), 16 * 1024));

      if (Configuration.ReceiveTimeout > 0)
        _bstreams[tid].ReadTimeout = Configuration.ReceiveTimeout;

      _socketslock.ReleaseLock();

      if (Configuration.Password != null)
      {
        SendCommand(RedisCommand.AUTH, Configuration.Password);
        WaitComplete();
      }

      return newsocket;
    }

    private void Quit()
    {
      if (GetSocket() != null) SendCommand(RedisCommand.QUIT);
    }

    internal Socket GetSocket()
    {
      var tid = Thread.CurrentThread.ManagedThreadId;

      try
      {
        _socketslock.AcquireReaderLock(1000);
        if (_sockets.ContainsKey(tid))
          return _sockets[tid];
        return null;
      }
      finally
      {
        _socketslock.ReleaseLock();
      }
    }

    //private BufferedStream GetBStream()
    internal NetworkStream GetBStream()
    {
      var tid = Thread.CurrentThread.ManagedThreadId;
      try
      {
        _socketslock.AcquireReaderLock(1000);
        var result = _bstreams[tid];
        return result;
      }
      finally
      {
        _socketslock.ReleaseLock();
      }
    }

    private DateTime GetLastCompletedCommandDate()
    {
      var tid = Thread.CurrentThread.ManagedThreadId;
      try
      {
        _socketslock.AcquireReaderLock(1000);
        if (_lastcommandcompleted.ContainsKey(tid))
          return _lastcommandcompleted[tid];
        return DateTime.MinValue;
      }
      finally
      {
        _socketslock.ReleaseLock();
      }
    }

    private void SetLastCompletedCommandDate(DateTime value)
    {
      var tid = Thread.CurrentThread.ManagedThreadId;
      try
      {
        if (_lastcommandcompleted.ContainsKey(tid))
          _lastcommandcompleted[tid] = value;
        else
          _lastcommandcompleted.Add(tid, value);
      }
      finally
      {
      }
    }

    private void RemoveSocket()
    {
      var tid = Thread.CurrentThread.ManagedThreadId;

      _socketslock.AcquireReaderLock(1000);
      if (_sockets.ContainsKey(tid))
      {
        _socketslock.UpgradeToWriterLock(1000);

        _bstreams.Remove(tid);
        _sockets.Remove(tid);

      }
      _socketslock.ReleaseLock();
    }

    public override void Close()
    {
      Quit();
      var socket = GetSocket();
      if (socket != null)
      {
        socket.Disconnect(false);
        socket.Close();
      }
      RemoveSocket();
    }

    internal void ShareConnectionWithThread(int threadid)
    {
      var socket = GetSocket();
      var nstream = GetBStream();

      var tid = Thread.CurrentThread.ManagedThreadId;

      if (_bstreams.ContainsKey(threadid))
        _bstreams[threadid] = nstream;
      else
        _bstreams.Add(threadid, nstream);

      if (_sockets.ContainsKey(threadid))
        _sockets[threadid] = socket;
      else
        _sockets.Add(threadid, socket);
    }

    internal void RemoveConnectionFromThread(int threadid)
    {
      if (_sockets.ContainsKey(threadid))
        _sockets.Remove(threadid);

      if (_bstreams.ContainsKey(threadid))
        _bstreams.Remove(threadid);
    }
    #endregion

    #region communication methods

    public int SendCommand(RedisCommand command, params string[] args)
    {

      // http://redis.io/topics/protocol
      // new redis communication protocol specifications

      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("*{0}\r\n", args != null ? args.Length + 1 : 1);

      var cmd = command.ToString();
      sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

      if (args != null)
        foreach (var arg in args)
        {
          sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
        }

      byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
      try
      {
        Log("S: " + sb.ToString());
        this.Connect().Send(r);
      }
      catch (SocketException e)
      {
        Log("Exception: " + e.Message);
        // timeout;
        GetSocket().Close();
        RemoveSocket();
        return 0;
      }
      if (Configuration.LogUnbalancedCommands)
        return _tracer.TraceCommand(command);
      return 0;
    }

    public int SendCommand(RedisCommand command, byte[] datas, params string[] args)
    {
      // http://redis.io/topics/protocol
      // new redis communication protocol specifications

      StringBuilder sb = new StringBuilder();

      int acount = args != null ? args.Length + 1 : 1;
      acount += datas != null && datas.Length > 0 ? 1 : 0;

      sb.AppendFormat("*{0}\r\n", acount);

      var cmd = command.ToString();
      sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

      if (args != null)
        foreach (var arg in args)
        {
          sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
        }

      byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
      var socket = this.Connect();

      try
      {
        Log("S: " + sb.ToString());
        // Send command and args. 
        socket.Send(r);

        // Send data
        if (datas != null && datas.Length > 0)
        {
          socket.Send(datas);
          socket.Send(Encoding.UTF8.GetBytes("\r\n"));
        }

      }
      catch (SocketException e)
      {
        // timeout;
        Log("Exception: " + e.Message);
        socket.Close();
        RemoveSocket();
        return 0;
      }
      if (Configuration.LogUnbalancedCommands)
        return _tracer.TraceCommand(command);
      return 0;
    }

    public int SendCommand(RedisCommand command, IDictionary<string, byte[]> datas, params string[] args)
    {

      // http://redis.io/topics/protocol
      // new redis communication protocol specifications

      StringBuilder sb = new StringBuilder();

      int acount = args != null ? args.Length + 1 : 1;
      acount += datas != null && datas.Count > 0 ? datas.Count * 2 : 0;

      sb.AppendFormat("*{0}\r\n", acount);

      var cmd = command.ToString();
      sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

      if (args != null)
        foreach (var arg in args)
        {
          sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
        }

      MemoryStream ms = new MemoryStream();

      byte[] r = Encoding.UTF8.GetBytes(sb.ToString());
      ms.Write(r, 0, r.Length);

      if (datas != null && datas.Count > 0)
      {
        foreach (var data in datas)
        {
          r = Encoding.UTF8.GetBytes(string.Format("${0}\r\n{1}\r\n", data.Key.Length, data.Key));
          ms.Write(r, 0, r.Length);
          r = Encoding.UTF8.GetBytes(string.Format("${0}\r\n", data.Value.Length));
          ms.Write(r, 0, r.Length);
          ms.Write(data.Value, 0, data.Value.Length);
          ms.Write(_end_data, 0, 2);
        }
      }

      var socket = this.Connect();
      try
      {
        Log("S: " + sb.ToString(), datas);
        socket.Send(ms.ToArray());
      }
      catch (SocketException e)
      {
        Log("Exception: " + e.Message);
        // timeout;
        socket.Close();
        RemoveSocket();

        throw;
      }
      if (Configuration.LogUnbalancedCommands)
        return _tracer.TraceCommand(command);
      return 0;
    }

    public int SendCommand(RedisCommand command, IDictionary<string, string> datas, params string[] args)
    {
      var result = new Dictionary<string, byte[]>();

      foreach (var kv in datas)
        result.Add(kv.Key, Encoding.UTF8.GetBytes(kv.Value));

      return SendCommand(command, result, args);
    }

    public void WaitComplete(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      int c = GetBStream().ReadByte();
      if (c == -1)
        throw new Exception("No more data");

      var s = ReadLine();
      Log((char)c + s);
      if (c == '-')
        throw new Exception(s.StartsWith("ERR") ? s.Substring(4) : s);
    }

    public int ReadInt(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      var s = ReadLine();
      Log(string.Format("R: {0}", s));

      var c = s[0];
      if (s[0] == ':')
      {
        int n = 0;
        if (int.TryParse(s.Substring(1), out n))
          return n;
      }

      if (c == '-')
        throw new Exception(s.StartsWith("ERR") ? s.Substring(4) : s);

      throw new Exception("Unexpected response ");
    }

    public byte[] ReadData(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      string r = ReadLine();
      Log(string.Format("R: {0}", r));

      if (string.IsNullOrEmpty(r))
        return null;

      char c = r[0];
      if (c == '-')
        throw new Exception(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));

      if (c == '$')
      {
        if (r == "$-1")
          return null;
        int n;

        var bstream = GetBStream();
        if (Int32.TryParse(r.Substring(1), out n))
        {
          byte[] retbuf = new byte[n];
          if (n > 0)
          {

            int bytesRead = 0;
            do
            {
              int read = bstream.Read(retbuf, bytesRead, n - bytesRead);
              if (read < 1)
                throw new Exception("Invalid termination mid stream");
              bytesRead += read;
            }
            while (bytesRead < n);
          }
          if (bstream.ReadByte() != '\r' || bstream.ReadByte() != '\n')
            throw new Exception("Invalid termination");
          Log(string.Format("R: {0}, {1}", r, Encoding.UTF8.GetString(retbuf)));

          return retbuf;
        }
        throw new Exception("Invalid length");
      }

      //returns the number of matches
      if (c == '*')
      {
        int n;
        if (Int32.TryParse(r.Substring(1), out n))
          return n <= 0 ? new byte[0] : ReadData();

        throw new Exception("Unexpected length parameter" + r);
      }

      throw new Exception("Unexpected reply: " + r);
    }

    public byte[][] ReadMulti(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      string r = ReadLine();
      Log(string.Format("R: {0}", r));

      if (string.IsNullOrEmpty(r))
        return null;

      char c = r[0];
      if (c == '-')
        throw new Exception(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));

      List<byte[]> result = new List<byte[]>();

      if (c == '*')
      {
        int n;
        if (Int32.TryParse(r.Substring(1), out n))
          for (int i = 0; i < n; i++)
          {
            result.Add(ReadData());
          }
      }
      return result.ToArray();
    }

    public string[] ReadMultiString(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      string r = ReadLine();
      Log(string.Format("R: {0}", r));
      if (r.Length == 0)
        throw new Exception("Zero length respose");

      char c = r[0];
      if (c == '-')
        throw new Exception(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));

      List<string> result = new List<string>();

      if (c == '*')
      {
        int n;
        if (Int32.TryParse(r.Substring(1), out n))
          for (int i = 0; i < n; i++)
          {
            result.Add(ReadString());
          }
      }
      return result.ToArray();
    }

    public string ReadString(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      var result = ReadData();
      if (result != null)
        return Encoding.UTF8.GetString(result);
      return null;
    }

    private string ReadLine(int commandid = 0)
    {
      if (commandid != 0 && Configuration.LogUnbalancedCommands)
        _tracer.CheckBalancing(commandid);

      var sb = new StringBuilder();
      int c;
      var bstream = GetBStream();

      while (!bstream.DataAvailable)
        Thread.Sleep(this.Configuration.ReceiveDelayms);
      //while (bstream.Length == 0)
      //  Thread.Sleep(10);

      if (bstream.DataAvailable)
        while ((c = bstream.ReadByte()) != -1)
        {
          if (c == '\r')
            continue;
          if (c == '\n')
            break;
          sb.Append((char)c);
        }

      SetLastCompletedCommandDate(DateTime.Now);
      return sb.ToString();
    }

    [Conditional("DEBUG")]
    private void Log(string fmt, IDictionary<string, byte[]> datas = null)
    {
      if (ActivateDebugLog)
      {
        Debug.WriteLine(fmt);
        if (datas != null)
          foreach (var kv in datas)
          {
            Debug.WriteLine(kv.Key, Encoding.UTF8.GetString(kv.Value));
          }

        errorlog.AppendFormat("{1} {0}\r\n", fmt.Trim(), DateTime.Now.ToString("hh:mm:ss"));
      }
    }

    [Conditional("DEBUG")]
    public void SendRedisClientExceptionLogToAuthor()
    {
      SmtpClient client = new SmtpClient("uhura.teamdev.it");
      MailMessage mm = new MailMessage();
      mm.To.Add("paolo@teamdev.it");
      mm.Subject = "Redis' Client AutomatedException";
      mm.From = new MailAddress("redis@teamdev.it");
      mm.Body = errorlog.ToString();
      mm.IsBodyHtml = false;

      client.Send(mm);
    }

    #endregion

    #region Expiration and Time Management

    [Description(CommandDescriptions.EXPIRE)]
    public bool Expire(string keyname, int seconds)
    {
      return ReadInt(SendCommand(RedisCommand.EXPIRE, keyname, seconds.ToString())) == 1;
    }

    [Description(CommandDescriptions.PERSIST)]
    public bool Persist(string keyname)
    {
      return ReadInt(SendCommand(RedisCommand.EXPIRE, keyname)) == 1;
    }

    [Description(CommandDescriptions.TTL)]
    public int TTL(string keyname)
    {
      return ReadInt(SendCommand(RedisCommand.TTL, keyname));
    }

    #endregion

  }
}
