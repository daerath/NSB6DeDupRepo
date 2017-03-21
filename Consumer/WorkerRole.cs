using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using Microsoft.WindowsAzure.ServiceRuntime;
using NServiceBus.Logging;

namespace Nsb6DedupeRepro.Consumer
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly NServiceBusRoleEntrypoint _nsb = new NServiceBusRoleEntrypoint();
        private static ILog Logger = LogManager.GetLogger<WorkerRole>();

        public override bool OnStart()
        {
            bool isStarted = false;

            try
            {
                _nsb.Start();
                isStarted = base.OnStart();
                Logger.Info("Role has started.");
            }
            catch (System.Exception ex)
            {
                Logger.Info("Role failed to start. Exception: " + ex.Message + " | InnerException: " + ex.InnerException + ".");
            }

            return isStarted;
        }

        public override void OnStop()
        {
            try
            {
                _nsb.Stop();
                base.OnStop();
                Logger.Info("Role has stopped.");
            }
            catch (System.Exception ex)
            {
                Logger.Info("Role failed to stop. Exception: " + ex.Message + " | InnerException: " + ex.InnerException + ".");
            }
        }
    }
}
