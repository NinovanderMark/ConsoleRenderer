using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples
{
    internal class ProgramDefinition
    {
        public object Instance { get; set; }
        public Action Tick { get; set; }

        public ProgramDefinition(object instance, Action tick)
        {
            Instance = instance;
            Tick = tick;
        }
    }
}
