using System;
using System.Collections.Generic;

using System.Text;
using TeamDev.Redis.Interface;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TeamDev.Redis.LanguageItems
{
  public class LanguageMessaging : ILanguageItem, IComplexItem
  {
    internal volatile string _name;
    internal volatile RedisDataAccessProvider _provider;

    private volatile bool _wasbalancingcalls = false;
    private volatile Task _readingThread = null;


    public int Publish(string channel, string message)
    {
      return _provider.ReadInt(_provider.SendCommand(RedisCommand.PUBLISH, channel, message));
    }


    public void Subscribe(params string[] channels)
    {
      _provider.SendCommand(RedisCommand.SUBSCRIBE, channels);

      if (_readingThread == null || _readingThread.Status != TaskStatus.Running)
      {
        _wasbalancingcalls = _provider.Configuration.LogUnbalancedCommands;

        _readingThread = Task.Factory.StartNew(() => ChannelsReadingThread(new ProviderState() { Provider = _provider, Stream = _provider.GetBStream() }));

        _provider.ShareConnectionWithThread(_readingThread.Id);

        _readingThread.Start();

        _provider.RemoveConnectionFromThread(Task.CurrentId ?? 0);
      }
    }

    async void ChannelsReadingThread(object state)
    {
      var provider = state as ProviderState;

      if (provider != null)
      {
        var stream = provider.Stream;
        while (true)
        {
          //_provider.Connect();

          while (!stream.DataAvailable)
            await Task.Delay(this._provider.Configuration.ReceiveDelayms);

          var operation = _provider.ReadString();

          switch (operation)
          {
            case "subscribe":
              var channel = _provider.ReadString();
              var clients = _provider.ReadInt();
              provider.Provider.RaiseChannelSubscribedEvent(channel);
              break;
            case "unsubscribe":
              var uchannel = _provider.ReadString();
              var uclients = _provider.ReadInt();
              provider.Provider.RaiseChannelUnsubscribedEvent(uchannel);

              if (uclients == 0) return;
              break;
            case "message":
              var mchannel = _provider.ReadString();
              var message = _provider.ReadString();
              provider.Provider.RaiseMessageReceivedEvend(mchannel, message);
              break;
            default:
              break;
          }
        }
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

  public class ProviderState
  {
    public RedisDataAccessProvider Provider { get; set; }
    public NetworkStream Stream { get; set; }
  }
}
