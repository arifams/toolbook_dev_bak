using PI.Contract.DTOs.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Salutation { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IList<Role.RolesDto> Roles { get; set; }

        public IList<DivisionDto> Divisions { get; set; }

        public bool IsActive { get; set; }
    }
}
