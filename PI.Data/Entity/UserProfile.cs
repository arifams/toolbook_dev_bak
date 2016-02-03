using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{

    public class UserProfile
    {
        [Key]
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    public class webpages_Membership
    {

        [Key]
        public long UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public string ConfirmationToken { get; set; }

        public bool IsConfirmed { get; set; }
        public DateTime LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public DateTime PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime PasswordVerificationTokenExpirationDate { get; set; }
    }

}
