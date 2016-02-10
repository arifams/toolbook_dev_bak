using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PI.Service.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {



    }
}
