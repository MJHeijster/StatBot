using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBotConfiguration
{
    public class ComboboxValue
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ComboboxValue(string iD, string name)
        {
            ID = iD;
            Name = name;
        }
        public ComboboxValue(string name)
        {
            ID = name;
            Name = name;
        }
    }
}
