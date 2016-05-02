using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.ImportAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IAddressBookManagement
    {
        PagedList GetAllAddresses(string type, string userId, string searchtext, int page = 1, int pageSize = 25);
        PagedList GetFilteredAddresses(string userId, string searchtext, int page = 1, int pageSize = 25);
        int DeleteAddress(long id);
        int SaveAddressDetail(AddressBookDto addressDetail);
        int ImportAddressBook(IList<ImportAddressDto> addressDetails, string userId);
        AddressBookDto GetAddressBookDtoById(long Id);
        byte[] GetAddressBookDetailsByUserId(string userId);
    }
}
