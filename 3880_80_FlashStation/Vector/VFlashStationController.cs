/* ------------------------------------------------------------------------
  File:	  VFlashStationController
  Module: FlashStationApp
---------------------------------------------------------------------------
  Controller handles the access to the vFlash Station API
---------------------------------------------------------------------------
  Copyright (c) Vector Informatik GmbH. All rights reserved.
------------------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using Vector.vFlash.Automation;

namespace _3880_80_FlashStation.Vector
{
  /// <summary>
  /// VFlashStationController
  /// </summary>
  internal class VFlashStationController
  {
    private readonly ReportErrorDelegate _reportErrorDelegate;

    internal delegate void ReportErrorDelegate(long handle, string message);

    /// <summary>
    /// Initializes a new instance of the <see cref="VFlashStationController"/> class.
    /// </summary>
    /// <param name="reportErrorDelegate">The report error delegate.</param>
    internal VFlashStationController(ReportErrorDelegate reportErrorDelegate)
    {
      _reportErrorDelegate = reportErrorDelegate;
    }

    internal bool InitializeAndLoadProjects(List<VFlashChannelConfigurator> dockConfigs)
    {
      //--- Initialize vFlash Station ---
      VFlashStationResult res = VFlashStationAPI.Initialize();
      if (res != VFlashStationResult.Success)
      {
        string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
        _reportErrorDelegate(-1, String.Format("Initialization of vFlash Station failed ({0}).", errMsg));            
 
        return false;
      }
      
      //--- Load projects ---
      foreach (VFlashChannelConfigurator dockConfig in dockConfigs)
      {
        long projectHandle;
        res = VFlashStationAPI.LoadProjectForChannel(dockConfig.FlashProjectPath, dockConfig.ChannelId, out projectHandle);

        if (res != VFlashStationResult.Success)
        {
          string errMsg = VFlashStationAPI.GetLastErrorMessage(projectHandle);
          _reportErrorDelegate(projectHandle, String.Format("Loading project failed ({0}) -> Rewind!", errMsg));
          UnloadProjectsAndDeinitialize(dockConfigs);
          return false;
        }

        dockConfig.ProjectHandle = projectHandle;
      }
    
      return true;
    }


    internal bool UnloadProjectsAndDeinitialize(List<VFlashChannelConfigurator> dockConfigs)
    {
      foreach (VFlashChannelConfigurator dockConfig in dockConfigs)
      {
        if (dockConfig.ProjectHandle != -1)
        {
          VFlashStationResult res = VFlashStationAPI.UnloadProject(dockConfig.ProjectHandle);
          if (res != VFlashStationResult.Success)
          {
            string errMsg = VFlashStationAPI.GetLastErrorMessage(dockConfig.ProjectHandle);
            _reportErrorDelegate(dockConfig.ProjectHandle, String.Format("Unload project failed ({0}).", errMsg));
          }
          dockConfig.ProjectHandle = -1;
        }
      }

      VFlashStationResult resDeinit = VFlashStationAPI.Deinitialize();
      if (resDeinit != VFlashStationResult.Success)
      {
        string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
        _reportErrorDelegate(-1, String.Format("Deinitialization of vFlash Station during rewinding failed ({0}).", errMsg));            
      } 
      
      return true;
    }


    internal bool StartFlashing(List<VFlashChannelConfigurator> dockConfigs)
    {
      foreach (VFlashChannelConfigurator dockConfig in dockConfigs)
      {
        if (dockConfig.ProjectHandle != -1)
        {
          VFlashStationResult res = VFlashStationAPI.Start(dockConfig.ProjectHandle, dockConfig.ProgressDelegate, dockConfig.StatusDelegate);
          
          if (res != VFlashStationResult.Success)
          {
            string errMsg = VFlashStationAPI.GetLastErrorMessage(dockConfig.ProjectHandle);
            _reportErrorDelegate(dockConfig.ProjectHandle, String.Format("Start reprogramming failed ({0}).", errMsg));
          }
        }
      }

      return true;
    }

    internal bool AbortFlashing(List<VFlashChannelConfigurator> dockConfigs)
    {
      bool errorOccurredButContinued = false;

      foreach (VFlashChannelConfigurator dockConfig in dockConfigs)
      {
        if (dockConfig.ProjectHandle != -1)
        {
          VFlashStationResult res = VFlashStationAPI.Stop(dockConfig.ProjectHandle);
          if (res != VFlashStationResult.Success)
          {
            string errMsg = VFlashStationAPI.GetLastErrorMessage(dockConfig.ProjectHandle);
            _reportErrorDelegate(dockConfig.ProjectHandle, String.Format("Stop reprogramming failed ({0}).", errMsg));
            errorOccurredButContinued = true;
          }
        }
      }
      return !errorOccurredButContinued;
    }
  }
}
