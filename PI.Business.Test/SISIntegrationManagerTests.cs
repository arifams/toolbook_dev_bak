using NUnit.Framework;
using PI.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class SISIntegrationManagerTests
    {
        SISIntegrationManager sisManager = null;

        public SISIntegrationManagerTests()
        {
            sisManager = new SISIntegrationManager();
        }

        [Test]
        public void GetRateSheetForShipmentTest()
        {
            
        }

        [Test]
        public void SendShipmentDetailsTest()
        {
            
        }

        [Test]
        public void DeleteShipmentTest()
        {
            
        }

        [Test]
        public void GetShipmentStatusTest()
        {
            
        }

        [Test]
        public void GetUpdatedShipmentStatusehistoryTest()
        {
            
        }

        [Test]
        public void TrackAndTraceShipmentTest()
        {
            new NotImplementedException();
        }

        [Test]
        public void GetRateRequestURLTest()
        {
            
        }

        [Test]
        public void GetLabelTest()
        {
            new NotImplementedException();
        }
    }
}