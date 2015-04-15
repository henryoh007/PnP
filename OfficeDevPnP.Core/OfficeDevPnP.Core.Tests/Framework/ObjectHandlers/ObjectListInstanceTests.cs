﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectListInstanceTests
    {
        private string listName;
        private static TokenParser parser;
        [TestInitialize]
        public void Initialize()
        {
            listName = string.Format("Test_{0}", DateTime.Now.Ticks);
            
        }
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var list = ctx.Web.GetListByUrl(string.Format("lists/{0}",listName));
                if (list != null)
                {
                    list.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            var listInstance = new Core.Framework.Provisioning.Model.ListInstance();

            listInstance.Url = string.Format("lists/{0}", listName);
            listInstance.Title = listName;
            listInstance.TemplateType = (int) ListTemplateType.GenericList;

            template.Lists.Add(listInstance);

            using (var ctx = TestCommon.CreateClientContext())
            {
                parser = new TokenParser(ctx.Web);

                new ObjectListInstance().ProvisionObjects(ctx.Web, template, parser);

                var list = ctx.Web.GetListByUrl(listInstance.Url);
                Assert.IsNotNull(list);
            }
        }

        [TestMethod]
        public void CanCreateEntities()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                // Load the base template which will be used for the comparison work
                var creationInfo = new ProvisioningTemplateCreationInformation(ctx.Web) {BaseTemplate = ctx.Web.GetBaseTemplate()};

                var template = new ProvisioningTemplate();
                template = new ObjectListInstance().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.Lists.Any());
            }
        }
    }
}
