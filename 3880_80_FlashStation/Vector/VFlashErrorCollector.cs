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

namespace _3880_80_FlashStation.Vector
{
  /// <summary>
  /// ErrorCollector
  /// </summary>
  internal class VFlashErrorCollector
  {
    private readonly List<string> _mMessages = new List<string>();

    public void Clear()
    {
      lock (_mMessages)
      {
        _mMessages.Clear();
      }
    }

    public void AddMessage(string message)
    {
      lock (_mMessages)
      {
        _mMessages.Add(message);
      }
    }

    public void AddMessage(string formatString, params object[] parameters)
    {
      lock (_mMessages)
      {
        _mMessages.Add(string.Format(formatString, parameters));
      }
    }

    public bool HasErrors()
    {
      lock (_mMessages)
      {
        return _mMessages.Count > 0;
      }
    }

    public string CreateReport()
    {
      StringBuilder reportBuilder = new StringBuilder();

      lock (_mMessages)
      {
        foreach (string message in _mMessages)
        {
          reportBuilder.AppendLine(message);
        }
        
      }
      return reportBuilder.ToString();
    }
  }
}
