using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
        class Program
        {
                static void Main(string[] args)
                {
                      NETHelper.Unzip.Run(@"C:\Users\YYp\Desktop\zips", @"C:\Users\YYp\Desktop\zips\unzip");
                }
        }
}
