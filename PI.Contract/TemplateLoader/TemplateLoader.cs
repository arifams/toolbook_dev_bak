using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PI.Contract.TemplateLoader
{
  public class TemplateLoader
    {

        public HtmlDocument getHtmlTemplatebyName(string name)
        {
            HtmlDocument template = new HtmlDocument();
            if (name=="invoiceUS")
            {

              //var stream= System.Web.HttpContext.Current.Server.MapPath("~\\Templates\\USInvoiceTemplate.html");
               template.Load(System.Web.HttpContext.Current.Server.MapPath("~\\Templates\\USInvoiceTemplate.html"));              
            }
           else if (name == "exceptionEmail")
            {

                var uploadFolder = "~/App_Data/Templates/ExceptionEmailTemplate.html";
                string wanted_path = System.Web.HttpContext.Current.Server.MapPath(uploadFolder);
                //var stream= System.Web.HttpContext.Current.Server.MapPath("~\\Templates\\USInvoiceTemplate.html");
                template.Load(wanted_path);
            }

            return template;
        }

       
    }
}
