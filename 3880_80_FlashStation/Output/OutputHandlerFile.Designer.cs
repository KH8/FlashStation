﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _3880_80_FlashStation.Output {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class OutputHandlerFile : global::System.Configuration.ApplicationSettingsBase {
        
        private static OutputHandlerFile defaultInstance = ((OutputHandlerFile)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OutputHandlerFile())));
        
        public static OutputHandlerFile Default {
            get {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfString>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                        <string>""noName""</string>
                                                                    </ArrayOfString>")]
        public string[] FileNameSuffix
        {
            get
            {
                return ((string[])(this["FileNameSuffix"]));
            }
            set
            {
                this["FileNameSuffix"] = value;
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
        public int[] StartAddress
        {
            get {
                return ((int[])(this["StartAddress"]));
            }
            set {
                this["StartAddress"] = value;
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
        public int[] EndAddress
        {
            get {
                return ((int[])(this["EndAddress"]));
            }
            set {
                this["EndAddress"] = value;
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
        public int[] SelectedIndex {
            get {
                return ((int[])(this["SelectedIndex"]));
            }
            set {
                this["SelectedIndex"] = value;
            }
        }
    }
}
