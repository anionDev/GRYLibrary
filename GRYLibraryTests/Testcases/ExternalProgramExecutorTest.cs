using GRYLibrary.Core.Miscellaneous;
using System.Threading;
using Semaphore = GRYLibrary.Core.Miscellaneous.Semaphore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRYLibrary.Core.Miscellaneous.CustomDisposables;
using System.IO;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class ExternalProgramExecutorTest
    {
        [TestMethod]
        public void TestSimpleEcho()
        {
            string testStdOut = "test";
            ExternalProgramExecutor e = new("echo", testStdOut);
            int result = e.StartSynchronously();
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, e.AllStdOutLines.Length);
            Assert.AreEqual(testStdOut, e.AllStdOutLines[0]);
            Assert.AreEqual(0, e.AllStdErrLines.Length);
        }
        [TestMethod]
        public void TestCopyFile1()
        {
            using (TemporaryDirectory temporaryDirectory = new())
            {
                //arrange
                string file1name = "File 1.txt";
                var file1 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file1name);
                Core.Miscellaneous.Utilities.EnsureFileExists(file1);
                string file2name = "File 2.txt";
                var file2 = Path.Combine(temporaryDirectory.TemporaryDirectoryPath, file2name);
                Core.Miscellaneous.Utilities.AssertCondition(!File.Exists(file2));
                ExternalProgramExecutor externalProgramExecutor = new("cp", $"\"{file1name}\" \"{file2name}\"", temporaryDirectory.TemporaryDirectoryPath);
                externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;

                //act
                externalProgramExecutor.StartSynchronously();

                //assert
                Assert.IsTrue(File.Exists(file2));
            }
        }
        [TestMethod]
        public void TestAsyncExecution()
        {
            ExternalProgramExecutor externalProgramExecutor = new(Utilities.TestUtilities.GetTimeoutTool(), 2.ToString());
            Semaphore semaphore = new();
            semaphore.Increment();
            externalProgramExecutor.ExecutionFinishedEvent += (ExternalProgramExecutor sender, int exitCode) =>
            {
                Assert.AreEqual(0, exitCode);
                semaphore.Decrement();
            };
            externalProgramExecutor.StartAsynchronously();
            Assert.AreNotEqual(0, externalProgramExecutor.ProcessId);
            while (semaphore.Value != 0)
            {
                Thread.Sleep(200);
            }
        }
    }
}
