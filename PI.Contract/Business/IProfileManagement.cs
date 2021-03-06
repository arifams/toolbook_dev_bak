﻿using PI.Contract.DTOs.Profile;
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
        string GetLanguageCodeByUserId(string userId);
        int UpdateProfileAccountSettings(ProfileDto updatedProfile);
        int UpdateProfileAddress(ProfileDto updatedProfile);
        int UpdateProfileBillingAddress(ProfileDto updatedProfile);
        int UpdateSetupWizardBillingAddress(ProfileDto updatedProfile);
        int UpdateProfileGeneral(ProfileDto updatedProfile);

        /// <summary>
        /// Update Theme Colour
        /// </summary>
        /// <param name="updatedProfile"></param>
        /// <returns></returns>
        int UpdateThemeColour(ProfileDto updatedProfile);

    }
}
