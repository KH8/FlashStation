﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _ttAgent.Vector {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class VFlashHandlerInterfaceAssignmentFile : global::System.Configuration.ApplicationSettingsBase {
        
        private static VFlashHandlerInterfaceAssignmentFile defaultInstance = ((VFlashHandlerInterfaceAssignmentFile)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new VFlashHandlerInterfaceAssignmentFile())));
        
        public static VFlashHandlerInterfaceAssignmentFile Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfArrayOfString>
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                            <ArrayOfString />
                                                                        </ArrayOfArrayOfString>")]
        public string[][] Assignment {
            get {
                return ((string[][])(this["Assignment"]));
            }
            set {
                this["Assignment"] = value;
            }
        }
    }
}
