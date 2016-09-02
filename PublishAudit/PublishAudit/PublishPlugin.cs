using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishAudit
{
    public class PublishPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.MessageName != "Publish" && context.MessageName != "PublishAll")
                return;

            string parameterXml = string.Empty;
            if (context.MessageName == "Publish")
            {
                if (context.InputParameters.Contains("ParameterXml"))
                {
                    parameterXml = (string)context.InputParameters["ParameterXml"];
                }
            }

            CreatePublishAuditRecord(service, context.MessageName, context.InitiatingUserId, parameterXml);
        }

        private void CreatePublishAuditRecord(IOrganizationService service, string messageName, Guid userId, string parameterXml)
        {
            Entity auditRecord = new Entity("fjo_publishaudit");
            auditRecord["fjo_message"] = messageName;
            auditRecord["fjo_publishbyid"] = new EntityReference("systemuser", userId);
            auditRecord["fjo_publishon"] = DateTime.Now;
            auditRecord["fjo_parameterxml"] = parameterXml;

            service.Create(auditRecord);
        }
    }
}
