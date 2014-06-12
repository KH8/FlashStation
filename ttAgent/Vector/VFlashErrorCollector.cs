/* ------------------------------------------------------------------------
  File:	  ErrorCollector.cs
  Module: FlashStationApp
---------------------------------------------------------------------------
  Collects error messages and returns a report of error messages.
---------------------------------------------------------------------------
  Copyright (c) Vector Informatik GmbH. All rights reserved.
------------------------------------------------------------------------ */

using System.Collections.Generic;
using System.Text;

namespace _ttAgent.Vector
{
  /// <summary>
  /// ErrorCollector
  /// </summary>
  internal class VFlashErrorCollector
  {
    private readonly List<string> _messages = new List<string>();

    public void Clear()
    {
      lock (_messages)
      {
        _messages.Clear();
      }
    }

    public void AddMessage(string message)
    {
      lock (_messages)
      {
        _messages.Add(message);
      }
    }

    public void AddMessage(string formatString, params object[] parameters)
    {
      lock (_messages)
      {
        _messages.Add(string.Format(formatString, parameters));
      }
    }

    public bool HasErrors()
    {
      lock (_messages)
      {
        return _messages.Count > 0;
      }
    }

    public string CreateReport()
    {
      var reportBuilder = new StringBuilder();

      lock (_messages)
      {
        foreach (string message in _messages)
        {
          reportBuilder.AppendLine(message);
        }
        
      }
      return reportBuilder.ToString();
    }
  }
}
