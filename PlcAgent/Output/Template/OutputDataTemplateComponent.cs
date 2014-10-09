﻿using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Output.Template
{
    public abstract class OutputDataTemplateComponent : UserControl
    {
        protected OutputDataTemplate OutputDataTemplate;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        protected OutputDataTemplateComponent(OutputDataTemplate outputDataTemplate)
        {
            OutputDataTemplate = outputDataTemplate;
            OutputDataTemplate.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
