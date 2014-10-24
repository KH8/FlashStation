using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
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

        public class VFlashDisplayProjectData
        {
            public uint Type { get; set; }
            public string Version { get; set; }
            public string Path { get; set; }
        }

        public class VFlashTypeComponent : VFlashDisplayProjectData
        {
            public VFlashTypeComponent(uint type, string version, string path)
            {
                Type = type;
                Version = version;
                Path = path;
            }
        }

        public static class VFlashTypeConverter
        {
            public static string[] VFlashTypesToStrings(List<VFlashDisplayProjectData> list)
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
        }

        #endregion


        #region Variables

        private List<VFlashDisplayProjectData> _children = new List<VFlashDisplayProjectData>();

        #endregion


        #region Properties

        public List<VFlashDisplayProjectData> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public VFlashTypeBankFile VFlashTypeBankFile;

        public override string Description
        {
            get { return Header.Name; }
        }

        #endregion


        #region Constructors

        public VFlashTypeBank(uint id, string name, VFlashTypeBankFile vFlashTypeBankFile) : base(id, name)
        {
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

        public override void GuiUpdateTemplate(TabControl mainTabControl, TabControl outputTabControl,
            TabControl connectionTabControl, Grid footerGrid)
        {
            var newtabItem = new TabItem { Header = Header.Name };
            outputTabControl.Items.Add(newtabItem);
            outputTabControl.SelectedItem = newtabItem;

            var newScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
            };
            newtabItem.Content = newScrollViewer;

            var newGrid = new Grid();
            newScrollViewer.Content = newGrid;

            newGrid.Height = Limiter.DoubleLimit(outputTabControl.Height - 50.0, 0);
            newGrid.Width = Limiter.DoubleLimit(outputTabControl.Width - 10, 0);

            var gridGuiVFlashPathBank = (GuiComponent)RegistryContext.Registry.GuiVFlashPathBanks.ReturnComponent(Header.Id);
            gridGuiVFlashPathBank.Initialize(0, 0, newGrid);
            var guiComponent = (GuiVFlashPathBank)gridGuiVFlashPathBank.UserControl;
            guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
        }

        public void Add(VFlashDisplayProjectData c)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == c.Type);
            if (child == null) _children.Add(c);
            else
            {
                child.Path = c.Path;
                child.Version = c.Version;
            }
        }

        public void Remove(VFlashDisplayProjectData c)
        {
            _children.Remove(c);
        }

        public string ReturnPath(uint type)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == type);
            return child != null ? child.Path : null;
        }

        public string ReturnPath(string version)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Version == version);
            return child != null ? child.Path : null;
        }

        public string ReturnVersion(uint type)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == type);
            return child != null ? child.Version : null;
        }

        #endregion
    }
}
