using System;
using System.Collections.Generic;

namespace uig.lib
{
    public class AppDefinitionObject
    {
        public string OutPutFolder { set; get; }
        public List<NgStateObject> States { set; get; }
        public string AppName { set; get; }
        public string DefaultRoute { set; get; }
        public List<string> Components { set; get; }
        public List<Tuple<string, string>> Views { set; get; }

      
    }
}