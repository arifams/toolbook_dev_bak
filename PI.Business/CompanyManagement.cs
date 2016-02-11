using PI.Contract.Business;
using PI.Contract.DTOs.Division;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class CompanyManagement : ICompanyManagement
    {

        //public IList<DivisionDto> GetAllDivisionsForCompany()
        //{
        //    using (var context = PIContext.Get())
        //    {
        //        return Mapper.Map<Division,DivisionDto>(context.Divisions.ToList());
        //    }
        //}

        //public DivisionDto GetDivisionById(long id)
        //{
        //    DivisionDto division = new DivisionDto();

        //    using (var context = PIContext.Get())
        //    {
        //        division = context.Divisions.SingleOrDefault(d => d.Id == id);
        //    }

        //    return
        //}


        public int SaveDivision(DivisionDto division)
        {
            
            using (var context = PIContext.Get())
            {
                if (division.Id == 0)
                {
                    Division newDivision = new Division()
                    {
                        Name = division.Name,
                        Description = division.Description,
                        DefaultCostCenterId = division.DefaultCostCenterId,
                        Status = division.Status,
                        CompanyId = division.CompanyId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1,// TODO : Get created user.                       
                    };
                    context.Divisions.Add(newDivision);
                }
                else
                {
                    var existingDivision = context.Divisions.SingleOrDefault(d => d.Id == division.Id);

                    existingDivision.Name = division.Name;
                    existingDivision.Description = division.Description;
                    existingDivision.DefaultCostCenterId = division.DefaultCostCenterId;
                    existingDivision.Status = division.Status;
                    existingDivision.CompanyId = division.CompanyId;
                    existingDivision.CreatedDate = DateTime.Now;
                    existingDivision.CreatedBy = 1; //sessionHelper.Get<User>().LoginName; 

                }
                context.SaveChanges();
            }

            return 1;
        }
    }
}
