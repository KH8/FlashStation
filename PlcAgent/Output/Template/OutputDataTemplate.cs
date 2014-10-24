using System;
using System.Windows.Controls;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.Output;

namespace _PlcAgent.Output.Template
{
    public class OutputDataTemplate : MainRegistry.Module
    {
        #region Properties

        public DataTemplateComposite Composite = new DataTemplateComposite("empty", null, null);
        public OutputDataTemplateFile OutputDataTemplateFile;

        public delegate void TemplateUpdateDelegate();

        public TemplateUpdateDelegate OnTemplateUpdateDelegate;

        public override string Description
        {
            get { return Header.Name; }
        }

        #endregion


        #region Constructors

        public OutputDataTemplate(uint id, string name, OutputDataTemplateFile outputDataTemplateFile) : base(id, name)
        {
            ReferencePosition = 4;
            OutputDataTemplateFile = outputDataTemplateFile;
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            try
            {
                Composite =
                    (DataTemplateComposite)
                        OutputDataTemplateBuilder.XmlFileToTemplate(OutputDataTemplateFile.TemplateFiles[Header.Id]);
                Logger.Log("ID: " + Header.Id + " Output Data Template Initialized");
            }
            catch (Exception)
            {
                OutputDataTemplateFile.TemplateFiles[Header.Id] = "Output\\Template\\Empty_Template.xml";
                OutputDataTemplateFile.Save();

                throw new Exception("Composite could not be retrived from XML or file was not found");
            }
        }

        public override void Deinitialize()
        {
            Logger.Log("ID: " + Header.Id + " Output Data Template Deinitialized");
        }

        public override void GuiUpdateTemplate(TabControl mainTabControl, TabControl outputTabControl,
            TabControl connectionTabControl, Grid footerGrid)
        {
            var newtabItem = new TabItem { Header = Header.Name };
            mainTabControl.Items.Add(newtabItem);
            mainTabControl.SelectedItem = newtabItem;

            var newGrid = new Grid();
            newtabItem.Content = newGrid;

            newGrid.Height = Limiter.DoubleLimit(mainTabControl.Height - 32, 0);
            newGrid.Width = Limiter.DoubleLimit(mainTabControl.Width - 10, 0);

            var gridGuiOutputDataTemplate = (GuiComponent)RegistryContext.Registry.GuiOutputDataTemplates.ReturnComponent(Header.Id);
            gridGuiOutputDataTemplate.Initialize(0, 0, newGrid);

            var gridGuiOutputDataTemplateGrid = (GuiOutputDataTemplate)gridGuiOutputDataTemplate.UserControl;
            gridGuiOutputDataTemplateGrid.UpdateSizes(newGrid.Height, newGrid.Width);
        }

        public void Clear()
        {
            Composite.Clear();
            Logger.Log("ID: " + Header.Id + " Output Data Template cleared");

            OutputDataTemplateFile.TemplateFiles[Header.Id] = "Output\\Template\\Empty_Template.xml";
            OutputDataTemplateFile.Save();

            if (OnTemplateUpdateDelegate != null) OnTemplateUpdateDelegate();
        }

        public void Import(string fileName)
        {
            Composite.Clear();
            Composite = (DataTemplateComposite)OutputDataTemplateBuilder.XmlFileToTemplate(fileName);
            Logger.Log("ID: " + Header.Id + " Output Data Template has been imported from file: " + fileName);

            OutputDataTemplateFile.TemplateFiles[Header.Id] = fileName;
            OutputDataTemplateFile.Save();

            if (OnTemplateUpdateDelegate != null) OnTemplateUpdateDelegate();
        }

        public void Export(string fileName)
        {
            Composite.Export(fileName);
            Logger.Log("ID: " + Header.Id + " Output Data Template has been exported to file: " + fileName);
        }

        #endregion

    }
}
