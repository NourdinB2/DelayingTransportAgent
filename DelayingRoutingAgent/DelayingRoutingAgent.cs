using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using DelayAgentConfig;
using System.Threading;

namespace DelayingRoutingAgent
{
    public class DelayingRoutingAgentFactory : RoutingAgentFactory
    {
        private AgentConfig agentConfig = new AgentConfig();
        public override RoutingAgent CreateAgent(SmtpServer server)
        {
            return new DelayingRoutingAgent(agentConfig, server);
        }

        public static void WriteLog(string message, EventLogEntryType entryType,
                                int eventID, string proccessName)
        {
            try
            {
                EventLog evtLog = new EventLog();
                evtLog.Log = "Application";
                evtLog.Source = proccessName;
                if (!EventLog.SourceExists(evtLog.Source))
                {
                    EventLog.CreateEventSource(evtLog.Source, evtLog.Log);
                }
                evtLog.WriteEntry(message, entryType, eventID);
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }
    }

    public class DelayingRoutingAgent : RoutingAgent
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private AgentConfig agentConfig;
        
        private SmtpServer server;

        public DelayingRoutingAgent(AgentConfig agentConfig, SmtpServer server)
        {
            // save the config
            this.agentConfig= agentConfig;
            this.server = server;

            // register on all possible Categorizer events
            this.OnSubmittedMessage += DelayingRoutingAgent_OnSubmittedMessage;
            this.OnResolvedMessage  += DelayingRoutingAgent_OnResolvedMessage;
            this.OnRoutedMessage    += DelayingRoutingAgent_OnRoutedMessage;
            this.OnCategorizedMessage += DelayingRoutingAgent_OnCategorizedMessage;

        }

        private void DelayingRoutingAgent_OnSubmittedMessage(SubmittedMessageEventSource source, QueuedMessageEventArgs e)
        {
            // delay time is configured in agentConfig
            System.Threading.Thread.Sleep(agentConfig.CATSMdelay);

            //throw new NotImplementedException();
        }

        private void DelayingRoutingAgent_OnResolvedMessage(ResolvedMessageEventSource source, QueuedMessageEventArgs e)
        {
            // delay time is configured in agentConfig
            System.Threading.Thread.Sleep(agentConfig.CATRSdelay);

            //throw new NotImplementedException();

            Thread thread = Thread.CurrentThread;
            thread.Abort();
        }

        private void DelayingRoutingAgent_OnRoutedMessage(RoutedMessageEventSource source, QueuedMessageEventArgs e)
        {
            // delay time is configured in agentConfig
            System.Threading.Thread.Sleep(agentConfig.CATRTdelay);

            //throw new NotImplementedException();
        }

        private void DelayingRoutingAgent_OnCategorizedMessage(CategorizedMessageEventSource source, QueuedMessageEventArgs e)
        {
            // delay time is configured in agentConfig
            System.Threading.Thread.Sleep(agentConfig.CatCMdelay);

            //throw new NotImplementedException();
        }

    }
}
