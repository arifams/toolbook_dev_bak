using PI.Contract.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IProfileManagement
    {
        ProfileDto getProfileByUserName(string username);
        ProfileDto GetAccountSettings(long customerId);
        ProfileDto GetCustomerAddressDetails(long cusomerAddressId, long companyId);
        ProfileDto getProfileByUserNameForShipment(string username);
        int UpdateProfileAccountSettings(ProfileDto updatedProfile);
        int UpdateProfileAddress(ProfileDto updatedProfile);
        int UpdateProfileBillingAddress(ProfileDto updatedProfile);
        int UpdateProfileLoginDetails(ProfileDto updatedProfile);
        int UpdateProfileGeneral(ProfileDto updatedProfile);
        int updateProfileData(ProfileDto updatedProfile);
    }
}
