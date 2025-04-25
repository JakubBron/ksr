using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;

namespace wydawca
{
    record Message1 : IMessage1
    {
        public string? messageIM1 { get; set; }
    }

    record Message2 : IMessage2
    {
        public string? messageIM2 { get; set; }
    }

    record Message3 : IMessage3
    {
        public string? messageIM1 { get; set; }
        public string? messageIM2 { get; set; }
    }

}
