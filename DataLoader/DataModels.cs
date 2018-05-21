using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoader
{
    public class Message
    {
        public ulong ID { get; set; }
        public string Text { get; set; }
        public DateTime Loaded { get; set; }
        public DateTime Processed { get; set; }
    }

    public class Keyword
    {
        public ulong ID { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Source
    {
        public string Name { get; set; }
    }
}
