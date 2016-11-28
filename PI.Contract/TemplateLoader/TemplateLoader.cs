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

        string uploadFolder = null;
        string correctPath = null;

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
                uploadFolder = "~/App_Data/Templates/ExceptionEmailTemplate.html";
            }
            else if (name == "OrderConfirmEmail")
            {
                uploadFolder = "~/App_Data/Templates/OrderConfirmEmail.html";
            }
            else if (name == "RegistrationEmailTemplate")
            {
                uploadFolder = "~/App_Data/Templates/RegistrationEmailTemplate.html";             
            }


            if (uploadFolder != null)
            {
                correctPath = System.Web.HttpContext.Current.Server.MapPath(uploadFolder);
                template.Load(correctPath);
            }

            return template;
        }

       
    }
}
