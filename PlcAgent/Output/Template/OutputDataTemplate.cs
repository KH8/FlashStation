using System;
using _PlcAgent.Log;

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
