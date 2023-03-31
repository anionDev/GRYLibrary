using GRYLibrary.Core.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class GRYLogTest
    {
        [TestMethod]
        public void TestLogProgress()
        {
            GRYLog logObject = GRYLog.Create();
            logObject.Configuration.Initliaze();
            logObject.Configuration.StoreProcessedLogItemsInternally = true;
            logObject.LogProgress(0, 4);
            logObject.LogProgress(3, 122);
            logObject.LogProgress(73, 73);
            Assert.AreEqual(3, logObject.ProcessedLogItems.Count);
            Assert.AreEqual("Processed 0/4 items (0%)", logObject.ProcessedLogItems[0].PlainMessage);
            Assert.AreEqual("Processed 003/122 items (2,46%)", logObject.ProcessedLogItems[1].PlainMessage);
            Assert.AreEqual("Processed 73/73 items (100%)", logObject.ProcessedLogItems[2].PlainMessage);
        }
    }
}