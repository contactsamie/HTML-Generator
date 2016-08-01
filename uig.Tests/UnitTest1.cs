using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using uig.lib;

namespace uig.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var outPutFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/!outputweb";
            const string appName = "samuel";

            const string defaultRoute = "/list";

            //the components and routing / states
            var components = new List<AnNgComponent>()
            {
                new SampleNgComponent(appName)
            };

            //the master page
            var webAppFactory = new BootStrapSampleWebAppGeneratorFactory(appName, components);

            var result = AngularJsWebAppFactory.GenerateWebApp(webAppFactory,outPutFolder,appName, defaultRoute);
        }
    }
}
