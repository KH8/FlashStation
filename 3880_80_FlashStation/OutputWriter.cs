using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3880_80_FlashStation
{
    abstract class OutputWriter
    {
        public string FilePath { get; set; }

        public abstract void CreateOutput(string fixedName, List<string> elementsList);
    }

    class OutputXmlWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            throw new NotImplementedException();
        }
    }

    class OutputCsvWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            throw new NotImplementedException();
        }
    }

    class OutputXlsWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            throw new NotImplementedException();
        }
    }
}
