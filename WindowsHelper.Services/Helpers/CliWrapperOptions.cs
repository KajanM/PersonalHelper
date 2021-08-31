using System;
using System.Collections.Generic;

namespace WindowsHelper.Services.Helpers
{
    public class CliWrapperOptions
    {
        public string ExecutablePath { get; set; }
        
        public IEnumerable<string> Arguments { get; set; }
        
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
    }
}