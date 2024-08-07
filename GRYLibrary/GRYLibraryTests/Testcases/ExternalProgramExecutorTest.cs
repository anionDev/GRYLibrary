﻿using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.Misc.CustomDisposables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class ExternalProgramExecutorTest
    {
        [TestMethod]
        public void TestEchoWithSomeSpecialCharacter()
        {
            string testStdOut = "test \\ \" < > ' testend";
            ExternalProgramExecutor externalProgramExecutor = new("echo", '"' + testStdOut.Replace("\"", "\\\"") + '"');
            externalProgramExecutor.Run();
            Assert.AreEqual(0, externalProgramExecutor.ExitCode);
            Assert.AreEqual(1, externalProgramExecutor.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, externalProgramExecutor.AllStdOutLines[0]);
            Assert.AreEqual(0, externalProgramExecutor.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestCopyFileWithSpaceInFilename()
        {
            //arrange
            using TemporaryDirectory temporaryDirectory = new();
            string file1name = "File 1.txt";
            string file1 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file1name);
            Core.Misc.Utilities.EnsureFileExists(file1);
            string file2name = "File 2.txt";
            string file2 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file2name);
            Core.Misc.Utilities.AssertCondition(!File.Exists(file2));
            ExternalProgramExecutor externalProgramExecutor = new("cp", $"\"{file1name}\" \"{file2name}\"", temporaryDirectory.TemporaryDirectoryPath);

            //act
            externalProgramExecutor.Run();

            //assert
            Assert.IsTrue(File.Exists(file2));
        }
        [TestMethod]
        public void TestCopyFileUseUmlautsAndOtherCharacterFromOtherLanguages()
        {
            //arrange
            using TemporaryDirectory temporaryDirectory = new();
            string file1name = "Sourcefile.txt";
            string file1 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file1name);
            Core.Misc.Utilities.EnsureFileExists(file1);
            string file2name = "[SpecialCharacterTest]äöüßÄÖ'ÜÆÑçéý[_SpecialCharacterTest].txt";
            string file2 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file2name);
            Core.Misc.Utilities.AssertCondition(!File.Exists(file2));
            ExternalProgramExecutor externalProgramExecutor = new("cp", $"\"{file1name}\" \"{file2name}\"", temporaryDirectory.TemporaryDirectoryPath);

            //act
            externalProgramExecutor.Run();

            //assert
            Assert.IsTrue(File.Exists(file2));
        }
    }
}