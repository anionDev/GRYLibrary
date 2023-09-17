using GRYLibrary.Core.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class GRYDateTimeTests
    {
        [TestMethod]
        public void TestGRYDateTimeFromString()
        {
            // arrange
            string input = "2017-12-14 11:55:54";
            GRYDateTime expected = new GRYDateTime(2017, 12, 14, 11, 55, 54);

            // act
            var actual = GRYDateTime.FromString(input);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
