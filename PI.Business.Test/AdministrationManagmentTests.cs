using NUnit.Framework;
using PI.Business;
using PI.Business.Test;
using PI.Contract.DTOs;
using PI.Contract.DTOs.AuditTrail;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class AdministrationManagmentTests
    {
        AdministrationManagment adminManagement = null;

        [TestFixtureSetUp]
        public void Init()
        {
            List<Company> companies = new List<Company>()
            {
                new Company
                {
                    Id=1,
                    Name="comp1",
                    COCNumber="123coc",
                    TenantId=1,
                    LogoUrl="http://parcelinternational.cpm",
                    CompanyCode="comp1",
                    IsDelete=false,
                    IsActive=true,
                    IsInvoiceEnabled=false,
                    Tenant= new Tenant
                    {
                        Id=1,
                        TenancyName="tenant1",
                        IsCorporateAccount=true,
                        IsActive=true,
                        IsDelete=false
                    }

                }
            };

            List<AuditTrail> auditTrails = new List<AuditTrail>()
            {
                new AuditTrail
                {
                    Id=1,
                    CreatedBy="1",
                    IsActive=true,
                    IsDelete=false
                }
            };

            var mockSet = MoqHelper.CreateMockForDbSet<Company>()
                                               .SetupForQueryOn(companies)
                                               .WithAdd(companies);
            var mockSetAuditTrail = MoqHelper.CreateMockForDbSet<AuditTrail>()
                                             .SetupForQueryOn(auditTrails)
                                             .WithAdd(auditTrails);


            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Company>(mockSet);

            // Prevent context null
            mockContext.Setup(c => c.Companies).Returns(mockSet.Object);
            mockContext.Setup(c => c.AuditTrail).Returns(mockSetAuditTrail.Object);

            adminManagement = new AdministrationManagment(mockContext.Object);
        }

        [Test]
        public void ImportRateSheetExcelTest()
        {
            string URI = "";
            OperationResult response = adminManagement.ImportRateSheetExcel(URI);
            Assert.AreEqual(response.Status, Status.Success);
        }

        [TestCase(1)]        
        public void ManageInvoicePaymentSettingTest(long _companyId)
        {
            long comapnyId = _companyId;
            bool response = adminManagement.ManageInvoicePaymentSetting(comapnyId);
                      
           Assert.AreEqual(response, true);           
        }

        [TestCase("1")]
        [TestCase("10")]
        public void GetAuditTrailsForCustomerTest(string _userId)
        {
            string userId = _userId;
            List<AuditTrailDto> response = adminManagement.GetAuditTrailsForCustomer(userId);
            if (_userId=="1")
            {
                Assert.AreEqual(response.Count, 1);
            }
            else if (_userId=="10")
            {
                Assert.AreEqual(response.Count, 0);
            }
            
        }
    }
}