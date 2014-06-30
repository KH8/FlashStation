using System;
using System.Collections.Generic;
using System.Linq;
using _ttAgent.Log;
using _ttAgent.MainRegistry;

namespace _ttAgent.Vector
{
    public class VFlashDisplayProjectData
    {
        public uint Type { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
    }

    class VFlashTypeBank : RegistryComponent
    {
        private List<VFlashDisplayProjectData> _children = new List<VFlashDisplayProjectData>();

        public VFlashTypeBankFile VFlashTypeBankFile;

        public VFlashTypeBank(uint id, string name, VFlashTypeBankFile vFlashTypeBankFile) : base(id, name)
        {
            VFlashTypeBankFile = vFlashTypeBankFile;
        }

        public List<VFlashDisplayProjectData> Children
        {
            get { return _children; }
            set { _children = value; }
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

        public string ReturnVersion(uint type)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == type);
            return child != null ? child.Version : null;
        }
    }

    class VFlashTypeComponent : VFlashDisplayProjectData
    {
        public VFlashTypeComponent(uint type, string version, string path)
        {
            Type = type;
            Version = version;
            Path = path;
        }
    }

    static class VFlashTypeConverter
    {
        public static string[] VFlashTypesToStrings(List<VFlashDisplayProjectData> list)
        {
            var output = new string[list.Count];
                uint i = 0;
                foreach (var vFlashType in list){
                    var type = (VFlashTypeComponent) vFlashType;
                    output[i] = type.Type +"=" + type.Version + "+" + type.Path; i++; }
            return output;
        }

        public static void StringsToVFlashChannels(string[] types, VFlashTypeBank bank)
        {
            try
            {
                var dictionary = types.Select(type => type.Split('=')).ToDictionary<string[], uint, string>(words => Convert.ToUInt16(words[0]), words => words[1]);
                var sortedDict = from entry in dictionary orderby entry.Key ascending select entry;
                foreach (KeyValuePair<uint, string> type in sortedDict)
                {
                    var words = type.Value.Split('+');
                    bank.Add(new VFlashTypeComponent(type.Key, words[0], words[1]));
                }
            }
            catch (Exception e) { Logger.Log("Configuration is wrong : " + e.Message);}
        }
    }
}
