using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultibeamFileProcessor
{
    public class Configurations
    {
        public int MaxFileSizeMB { get; set; }

        public string[] AllowedExtensions { get; set; }
    }
}
