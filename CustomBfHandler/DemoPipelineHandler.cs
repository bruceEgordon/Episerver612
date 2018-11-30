using Mediachase.BusinessFoundation.Data.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBfHandler
{
    public class DemoPipelineHandler : BaseRequestHandler, IPlugin
    {
        public new void Execute(BusinessContext context)
        {
            switch (context.PluginStage)
            {
                case EventPipeLineStage.All:
                    break;

                case EventPipeLineStage.MainOperation:
                    break;
                case EventPipeLineStage.None:
                    break;
                case EventPipeLineStage.PostMainOperation:
                    PostExecWork(context);
                    break;
                case EventPipeLineStage.PostMainOperationInsideTranasaction:
                    PostTransactionalWork(context);
                    break;
                case EventPipeLineStage.PreMainOperation:
                    PreExecWork(context);
                    break;
                case EventPipeLineStage.PreMainOperationInsideTranasaction:
                    break;
                default:
                    break;
            }
            base.Execute(context);
        }

        private void PreExecWork(BusinessContext context)
        {
            throw new NotImplementedException();
        }

        private void PostTransactionalWork(BusinessContext context)
        {
            throw new NotImplementedException();
        }

        private void PostExecWork(BusinessContext context)
        {
            throw new NotImplementedException();
        }
    }
}
