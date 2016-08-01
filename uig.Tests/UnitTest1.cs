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

            var result =     HtmlElementFactory.GenerateSampleResult(outPutFolder);
        }
    }
}
