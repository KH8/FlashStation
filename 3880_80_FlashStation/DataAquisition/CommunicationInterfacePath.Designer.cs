﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _3880_80_FlashStation.DataAquisition {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    public sealed partial class CommunicationInterfacePath : global::System.Configuration.ApplicationSettingsBase {
        
        private static CommunicationInterfacePath defaultInstance = ((CommunicationInterfacePath)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new CommunicationInterfacePath())));
        
        public static CommunicationInterfacePath Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfString>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                        <string>""DataAquisition\\DB1000_NEW.csv""</string>
                                                                    </ArrayOfString>")]
        public string[] Path {
            get {
                return ((string[])(this["Path"]));
            }
            set {
                this["Path"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfInt>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                    </ArrayOfInt>")]
        public int[] ConfigurationStatus {
            get {
                return ((int[])(this["ConfigurationStatus"]));
            }
            set {
                this["ConfigurationStatus"] = value;
            }
        }
    }
}
