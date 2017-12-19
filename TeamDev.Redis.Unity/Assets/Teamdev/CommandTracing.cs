using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace TeamDev.Redis
{
  internal class CommandTracing
  {
    private Queue<CommandTrace> _commandsequence = new Queue<CommandTrace>();
    private int _nextcommandid = 0;

    public int TraceCommand(RedisCommand command)
    {
      return TraceCommand(command.ToString());
    }

    public int TraceCommand(string command)
    {
      _nextcommandid++;
      _commandsequence.Enqueue(new CommandTrace() { Id = _nextcommandid, Command = command });
      return _nextcommandid;
    }

    public void CheckBalancing(int commandid)
    {
      var c = _commandsequence.Dequeue();
      if (c.Id == commandid)
        return;

      throw new UnbalancedReadException(string.Format("Discovered an unbalanced Read for command : {1} {0} ", c.Command, c.Id));
    }
  }

  internal struct CommandTrace
  {
    public int Id;
    public string Command;
  }
}
