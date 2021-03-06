﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace PI.Contract.DTOs.Common
{
    public class PagedList
    {
        public IList Content { get; set; }
        public JObject filterContent { get; set; }
        public dynamic DynamicContent { get; set; }
        public Int32 CurrentPage { get; set; }
        public Int32 PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string UserId { get; set; }
        //public int TotalPages
        //{
        //    get { return (int)Math.Ceiling((decimal)TotalRecords / PageSize); }
        //}
    } 
}