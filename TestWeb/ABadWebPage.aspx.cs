using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestWeb
{
    public partial class ABadWebPage : System.Web.UI.Page
    {
        public string UserSessionGuid { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.UserSessionGuid = Session["UserSessionGuid"].ToString();
        }
    }
}