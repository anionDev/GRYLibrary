﻿using GRYLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class ExternalProgramExecutorTest
    {
        [TestMethod]
        public void TestSimpleEcho()
        {
            string testStdOut = "test";
            ExternalProgramExecutor e = ExternalProgramExecutor.Create("echo", testStdOut);
            int result =e.StartConsoleApplicationInCurrentConsoleWindow();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);

        }
    }
}
