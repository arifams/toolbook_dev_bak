using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.User
{
    public class UserResultDto
    {
        public string UserId { get; set; }
        public bool IsSucess { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsAddUser { get; set; }
    }
}
