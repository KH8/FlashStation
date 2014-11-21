using System;
using System.Collections.Generic;
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

        public class VFlashTypeComponentStepCondition
        {
            public VFlashStationResult Result { get; set; }
            public Boolean Condition { get; set; }

            public VFlashTypeComponentStepCondition(VFlashStationResult result, Boolean condition)
            {
                Result = result;
                Condition = condition;
            }
        }

        public class VFlashTypeComponent
        {
            public class Step
            {
                public int Id { get; set; }
                public string Path { get; set; }
                public int TransitionDelay { get; set; }

                public List<VFlashTypeComponentStepCondition> TransitionConditions;

                public string TransitionSignature
                {
                    get
                    {
                        return TransitionConditions.OrderBy(o => o.Result).Aggregate("", (current, vFlashTypeComponentStepCondition) => current + Convert.ToInt32(vFlashTypeComponentStepCondition.Condition));
                    }
                }

                public Step(int id)
                {
                    Id = id;
                    Path = "no path assigned";
                    TransitionDelay = 100;
                    TransitionConditions = new List<VFlashTypeComponentStepCondition>();
                    foreach (VFlashStationResult result in Enum.GetValues(typeof(VFlashStationResult)))
                    {
                        var status = result == VFlashStationResult.Success;
                        TransitionConditions.Add(new VFlashTypeComponentStepCondition(result, status));
                    }
                }

                public void RestoreConditions(string signature)
                {
                    var conditions = signature.ToCharArray();
                    var i = 0;
                    foreach (var condition in TransitionConditions.OrderBy(o => o.Result))
                    {
                        condition.Condition = (conditions[i] == '1');
                        i++;
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

        public static class VFlashTypeBuilder
        {
            public static List<VFlashTypeComponent> Build(uint id, VFlashTypeBankFile vFlashTypeBankFile)
            {
                var newlist = new List<VFlashTypeComponent>();
                var i = 0;

                foreach (var newType in vFlashTypeBankFile.TypeBank[id].Select(type => new VFlashTypeComponent(type)))
                {
                    newType.Steps.Clear();

                    foreach (var step in vFlashTypeBankFile.Steps[id][i])
                    {
                        var properties = step.Split(';');
                        var newStep = new VFlashTypeComponent.Step(Convert.ToInt32(properties[0]))
                        {
                            Path = properties[1],
                            TransitionDelay = Convert.ToInt32(properties[2])
                        };
                        newStep.RestoreConditions(properties[3]);
                        newType.Steps.Add(newStep);
                    }

                    newlist.Add(newType);
                    i++;
                }

                return newlist;
            }

            public static void UpdateConfiguration(uint id, VFlashTypeBankFile vFlashTypeBankFile, List<VFlashTypeComponent> children)
            {
                var i = 0;
                vFlashTypeBankFile.TypeBank[id] = new string[children.Count];
                vFlashTypeBankFile.Steps[id] = new string[children.Count][];
                foreach (var vFlashTypeComponent in children)
                {
                    vFlashTypeBankFile.TypeBank[id][i] = vFlashTypeComponent.Version;
                    vFlashTypeBankFile.Steps[id][i] = new string[vFlashTypeComponent.Steps.Count];

                    var j = 0;
                    foreach (var step in vFlashTypeComponent.Steps)
                    {
                        vFlashTypeBankFile.Steps[id][i][j] = step.Id + ";" + step.Path + ";" + step.TransitionDelay + ";" + step.TransitionSignature;
                        j++;
                    }
                    i++;
                }

                vFlashTypeBankFile.Save();
            }
        }

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
            ReferencePosition = 3;
            VFlashTypeBankFile = vFlashTypeBankFile;

            Children = VFlashTypeBuilder.Build(Header.Id, vFlashTypeBankFile);
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

        public void Update()
        {
            VFlashTypeBuilder.UpdateConfiguration(Header.Id, VFlashTypeBankFile.Default, Children);
        }

        //temp
        public string ReturnPath(string version)
        {
            return "";
        }

        #endregion
    }
}
