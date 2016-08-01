using System;
using System.Collections.Generic;

namespace uig.lib
{
    public abstract class AnNgComponent
    {
        protected AnNgComponent(string appName)
        {

            Module = @"angular.module('" + appName + @"')";
        }

        public string Module { private set; get; }
        public abstract List<string> GetFactories();
        public abstract List<string> GetServices();
        public abstract List<string> GetDirectives();
        public abstract List<string> GetControllers();
        public abstract List<Tuple<string, string>> GetViews();
        public abstract List<NgStateObject> GetStates();
    }
}