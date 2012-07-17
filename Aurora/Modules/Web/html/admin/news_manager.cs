﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aurora.Framework.Servers.HttpServer;
using Aurora.Framework;
using Nini.Config;
using OpenMetaverse;
using OpenSim.Services.Interfaces;

namespace Aurora.Modules.Web
{
    public class NewsManagerPage : IWebInterfacePage
    {
        public string[] FilePath
        {
            get
            {
                return new[]
                       {
                           "html/admin/news_manager.html"
                       };
            }
        }

        public bool RequiresAuthentication { get { return false; } }
        public bool RequiresAdminAuthentication { get { return false; } }

        public Dictionary<string, object> Fill(WebInterface webInterface, string filename, OSHttpRequest httpRequest,
            OSHttpResponse httpResponse, Dictionary<string, object> requestParameters, ITranslator translator)
        {
            var vars = new Dictionary<string, object>();
            IGenericsConnector connector = Aurora.DataManager.DataManager.RequestPlugin<IGenericsConnector>();
            if (httpRequest.Query.Contains("delete"))
            {
                string newsID = httpRequest.Query["delete"].ToString();
                connector.RemoveGeneric(UUID.Zero, "WebGridNews", newsID);
                vars["Success"] = "Successfully deleted the news item";
            }
            else if (httpRequest.Query.Contains("edit"))
            {
                string newsID = httpRequest.Query["edit"].ToString();

                webInterface.Redirect(httpResponse, "edit_news.html?newsid=" + newsID);
                return vars;
            }
            else
                vars["Success"] = "";
            var newsItems = connector.GetGenerics<GridNewsItem>(UUID.Zero, "WebGridNews");
            if (newsItems.Count == 0)
                newsItems.Add(GridNewsItem.NoNewsItem);
            vars.Add("News", newsItems.ConvertAll<Dictionary<string, object>>(item => item.ToDictionary()));
            vars.Add("NewsManager", translator.GetTranslatedString("NewsManager"));
            vars.Add("EditNewsItem", translator.GetTranslatedString("EditNewsItem"));
            vars.Add("AddNewsItem", translator.GetTranslatedString("AddNewsItem"));
            vars.Add("DeleteNewsItem", translator.GetTranslatedString("DeleteNewsItem"));
            vars.Add("NewsTitleText", translator.GetTranslatedString("NewsTitleText"));
            vars.Add("NewsDateText", translator.GetTranslatedString("NewsDateText"));
            vars.Add("EditNewsText", translator.GetTranslatedString("EditNewsText"));
            vars.Add("DeleteNewsText", translator.GetTranslatedString("DeleteNewsText"));
            return vars;
        }
    }
}
