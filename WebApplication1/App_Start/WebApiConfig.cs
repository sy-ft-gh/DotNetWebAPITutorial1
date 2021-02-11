using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API の設定およびサービス

            // Web API ルート
            config.MapHttpAttributeRoutes();
            /* AttributeRoutesのみ有効とする
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            */

            // メディアフォーマッターからXMLフォーマットを削除
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // 続いてJSOSNフォーマットを追加。→要求に対して常にJSONで応答する
            config.Formatters.Add(config.Formatters.JsonFormatter);
            // メディアフォーマッターについては以下を参照(リクエストのAcceptに応じてXMLとJSONが切り替わる)
            // https://docs.microsoft.com/ja-jp/aspnet/web-api/overview/formats-and-model-binding/content-negotiation
        }
    }
}
