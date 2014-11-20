using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Vector.vFlash.Automation;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.Vector;

namespace _PlcAgent.Vector
{
    public class VFlashTypeBank : Module
    {
        #region Subclasses

        public class VFlashTypeComponent
        {
            public class Step
            {
                public int Id { get; set; }
                public string Path { get; set; }
                public int TransitionDelay { get; set; }
                public Dictionary<VFlashStationResult, Boolean> TransitionConditions;

                public Step(int id)
                {
                    Id = id;
                    Path = "no path assigned";
                    TransitionDelay = 100;
                    TransitionConditions = new Dictionary<VFlashStationResult, bool>();
                    foreach (VFlashStationResult result in Enum.GetValues(typeof(VFlashStationResult)))
                    {
                        TransitionConditions.Add(result, false);
                    }
                }
            }
            
            public string Version { get; set; }
            public List<Step> Steps { get; set; } 

            public VFlashTypeComponent(string version)
            {
                Version = version;
                Steps = new List<Step> {new Step(1)};
            }
        }

        /*public static class VFlashTypeConverter
        {
            public static string[] VFlashTypesToStrings(List<VFlashTypeComponent> list)
            {
                var output = new string[list.Count];
                uint i = 0;
                foreach (var type in list.Cast<VFlashTypeComponent>())
                {
                    output[i] = type.Type + "=" + type.Version + "+" + type.Path;
                    i++;
                }
                return output;
            }

            public static void StringsToVFlashChannels(string[] types, VFlashTypeBank bank)
            {
                try
                {
                    var dictionary =
                        types.Select(type => type.Split('='))
                            .ToDictionary<string[], uint, string>(words => Convert.ToUInt16(words[0]), words => words[1]);
                    var sortedDict = from entry in dictionary orderby entry.Key ascending select entry;
                    foreach (var type in sortedDict)
                    {
                        var words = type.Value.Split('+');
                        bank.Add(new VFlashTypeComponent(type.Key, words[0], words[1]));
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Configuration is wrong : " + e.Message);
                }
            }
        }*/

        #endregion


        #region Properties

        public List<VFlashTypeComponent> Children { get; set; }

        public VFlashTypeBankFile VFlashTypeBankFile;

        public override string Description
        {
            get { return Header.Name; }
        }

        #endregion


        #region Constructors

        public VFlashTypeBank(uint id, string name, VFlashTypeBankFile vFlashTypeBankFile) : base(id, name)
        {
            Children = new List<VFlashTypeComponent>();

            ReferencePosition = 3;
            VFlashTypeBankFile = vFlashTypeBankFile;
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
        }

        public override void Deinitialize()
        {
            Logger.Log("ID: " + Header.Id + " vFlashTypeBank Deinitialized");
        }

        public override void TemplateGuiUpdate(TabControl mainTabControl, TabControl outputTabControl,
            TabControl connectionTabControl, Grid footerGrid)
        {
            var newtabItem = new TabItem { Header = Header.Name };
            mainTabControl.Items.Add(newtabItem);
            mainTabControl.SelectedItem = newtabItem;

            var newGrid = new Grid();
            newtabItem.Content = newGrid;

            newGrid.Height = Limiter.DoubleLimit(mainTabControl.Height - 32.0, 0);
            newGrid.Width = Limiter.DoubleLimit(mainTabControl.Width - 10, 0);

            var gridGuiVFlashPathBank = (GuiComponent)RegistryContext.Registry.GuiVFlashPathBanks.ReturnComponent(Header.Id);
            gridGuiVFlashPathBank.Initialize(0, 0, newGrid);

            var guiComponent = (GuiVFlashPathBank)gridGuiVFlashPathBank.UserControl;
            guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
        }

        public override void TemplateRegistryComponentUpdateRegistryFile()
        {
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id] = new uint[9];
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id][0] = Header.Id;
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id][1] = 0;
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id][2] = 0;
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id][3] = 0;
            MainRegistryFile.Default.VFlashTypeBanks[Header.Id][4] = 0;
        }

        public override void TemplateRegistryComponentCheckAssignment(RegistryComponent component)
        {
            if (MainRegistryFile.Default.VFlashTypeBanks[Header.Id][component.ReferencePosition] == component.Header.Id) throw new Exception("The component is still assigned to another one");
        }

        public void Add(VFlashTypeComponent c)
        {
            var child = Children.FirstOrDefault(typeFound => typeFound.Version == c.Version);
            if (child != null) return;
            Children.Add(c);
        }

        public void Remove(VFlashTypeComponent c)
        {
            Children.Remove(c);
        }

        //temp
        public string ReturnPath(string version)
        {
            return "";
        }

        #endregion
    }
}
