using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebApplication1
{
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start()�@{
            // WebApiConfig.Register���Ăяo�����ƂŃ��[�e�B���O��
            // �ݒ肵�܂��B
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
