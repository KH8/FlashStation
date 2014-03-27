/* ------------------------------------------------------------------------
  File:	  DockConfig
  Module: FlashStationApp
---------------------------------------------------------------------------
  Configuration for one flash dock that contains the project handle,
  the project path, the channel id, the status- and progress callbacks 
---------------------------------------------------------------------------
  Copyright (c) Vector Informatik GmbH. All rights reserved.
------------------------------------------------------------------------ */

using Vector.vFlash.Automation;

namespace _3880_80_FlashStation.Vector
{
  /// <summary>
  /// DockConfig
  /// </summary>
  internal class VFlashChannelConfigurator
  {
    private long _projectHandle;
    private string _flashProjectPath;
    private uint _channelId;
    private CallbackProgressDelegate _progressDelegate;
    private CallbackStatusDelegate _statusDelegate;

    public VFlashChannelConfigurator()
    {
      _projectHandle = -1;
    }

    public VFlashChannelConfigurator(string flashProjectPath, uint channelId, CallbackProgressDelegate progressDelegate, CallbackStatusDelegate statusDelegate)
    {
      _progressDelegate = progressDelegate;
      _statusDelegate = statusDelegate;
      _flashProjectPath = flashProjectPath;
      _channelId = channelId;
      _projectHandle = -1;
    }

    public uint ChannelId
    {
      get { return _channelId; }
      set { _channelId = value; }
    }

    public long ProjectHandle
    {
      get { return _projectHandle; }
      set { _projectHandle = value; }
    }

    public string FlashProjectPath
    {
      get { return _flashProjectPath; }
      set { _flashProjectPath = value; }
    }

    public CallbackProgressDelegate ProgressDelegate
    {
      get { return _progressDelegate; }
      set { _progressDelegate = value; }
    }

    public CallbackStatusDelegate StatusDelegate
    {
      get { return _statusDelegate; }
      set { _statusDelegate = value; }
    }
  }
}
