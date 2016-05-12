using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NETHelper.Tests
{
      [TestClass()]
      public class UnzipTests
      {
            [TestMethod()]
            public void Run()
            {
                  NETHelper.Unzip.Run(@"C:\Users\YYp\Desktop\zips", @"C:\Users\YYp\Desktop\zips\unzip1");
            }
      }
}
