using GRYLibrary.Core.Miscellaneous;
using System.Threading;
using Semaphore = GRYLibrary.Core.Miscellaneous.Semaphore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRYLibrary.Core.Miscellaneous.CustomDisposables;
using System.IO;
using GRYLibrary.Core.ExecutePrograms;

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
            using TemporaryDirectory temporaryDirectory = new();
            //arrange
            string file1name = "File 1.txt";
            var file1 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file1name);
            Core.Miscellaneous.Utilities.EnsureFileExists(file1);
            string file2name = "File 2.txt";
            var file2 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file2name);
            Core.Miscellaneous.Utilities.AssertCondition(!File.Exists(file2));
            ExternalProgramExecutor externalProgramExecutor = new("cp", $"\"{file1name}\" \"{file2name}\"", temporaryDirectory.TemporaryDirectoryPath);

            //act
            externalProgramExecutor.Run();

            //assert
            Assert.IsTrue(File.Exists(file2));
        }
        [TestMethod]
        public void TestCopyFileUseUmlautsAndOtherCharacterFromOtherLanguages()
        {
            using TemporaryDirectory temporaryDirectory = new();
            //arrange
            string file1name = "Sourcefile.txt";
            var file1 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file1name);
            Core.Miscellaneous.Utilities.EnsureFileExists(file1);
            string file2name = "[SpecialCharacterTest]äöüßÄÖ'ÜÆÑçéý[_SpecialCharacterTest].txt";
            var file2 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file2name);
            Core.Miscellaneous.Utilities.AssertCondition(!File.Exists(file2));
            ExternalProgramExecutor externalProgramExecutor = new("cp", $"\"{file1name}\" \"{file2name}\"", temporaryDirectory.TemporaryDirectoryPath);

            //act
            externalProgramExecutor.Run();

            //assert
            Assert.IsTrue(File.Exists(file2));
        }
        [TestMethod]
        public void TestAsyncExecution()
        {
            ExternalProgramExecutor externalProgramExecutor = new(Utilities.TestUtilities.GetTimeoutTool(), 2.ToString());
            Semaphore semaphore = new();
            semaphore.Increment();
            externalProgramExecutor.ExecutionFinishedEvent += (ExternalProgramExecutor sender, int exitCode) =>
            {
                if (0 == exitCode)
                {
                    semaphore.Decrement();
                }
                else
                {
                    semaphore.Increment();
                }
            };
            externalProgramExecutor.Run();
            Assert.AreNotEqual(0, externalProgramExecutor.ProcessId);
            while (semaphore.Value == 1)
            {
                Thread.Sleep(200);
            }
            if (semaphore.Value > 1)
            {
                Assert.Fail();
            }
        }
    }
}
