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
  internal class VFlashDockConfig
  {
    private long _mProjectHandle;
    private string _mFlashProjectPath;
    private uint _mChannelId;
    private CallbackProgressDelegate _mProgressDelegate;
    private CallbackStatusDelegate _mStatusDelegate;

    public VFlashDockConfig()
    {
      _mProjectHandle = -1;
    }

    public VFlashDockConfig(string flashProjectPath, uint channelId, CallbackProgressDelegate progressDelegate, CallbackStatusDelegate statusDelegate)
    {
      _mProgressDelegate = progressDelegate;
      _mStatusDelegate = statusDelegate;
      _mFlashProjectPath = flashProjectPath;
      _mChannelId = channelId;
      _mProjectHandle = -1;
    }

    public uint ChannelId
    {
      get { return _mChannelId; }
      set { _mChannelId = value; }
    }

    public long ProjectHandle
    {
      get { return _mProjectHandle; }
      set { _mProjectHandle = value; }
    }

    public string FlashProjectPath
    {
      get { return _mFlashProjectPath; }
      set { _mFlashProjectPath = value; }
    }

    public CallbackProgressDelegate ProgressDelegate
    {
      get { return _mProgressDelegate; }
      set { _mProgressDelegate = value; }
    }

    public CallbackStatusDelegate StatusDelegate
    {
      get { return _mStatusDelegate; }
      set { _mStatusDelegate = value; }
    }
  }
}
