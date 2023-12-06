using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Tests.Testcases.APIServer.Mid.Aut
{
    [TestClass]
    public class AuthorizeAttributeTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void AuthorizeAttributeConstructorTests()
        {
            Assert.IsTrue(new AuthorizeAttribute(null).Groups.SetEquals(new HashSet<string>()));
            Assert.IsTrue(new AuthorizeAttribute(string.Empty).Groups.SetEquals(new HashSet<string>()));
            Assert.IsTrue(new AuthorizeAttribute("a").Groups.SetEquals(new HashSet<string>() { "a" }));
            Assert.IsTrue(new AuthorizeAttribute("a,b").Groups.SetEquals(new HashSet<string>() { "a", "b" }));
            Assert.IsTrue(new AuthorizeAttribute("a,b,c").Groups.SetEquals(new HashSet<string>() { "a", "b", "c" }));
        }
    }
}