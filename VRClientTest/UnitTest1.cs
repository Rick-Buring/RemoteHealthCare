using CommunicationObjects.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vr_Project.RemoteHealthcare;

namespace VRClientTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void testCheckSum()
        {
            //fake resistance massage
            byte checksum = Ergometer.checksum(new byte[] { 0xa4, 0x09, 0x4e, 0x05, 0x30, 0, 0, 0, 0, 0x05, 0 });

            Assert.AreEqual(0xd3, checksum);
        }

    }
}
