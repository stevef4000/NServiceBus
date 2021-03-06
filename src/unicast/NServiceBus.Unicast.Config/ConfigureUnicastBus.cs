﻿using NServiceBus.Config;
using NServiceBus.Unicast.Config;

namespace NServiceBus
{
    using System;

    /// <summary>
    /// Contains extension methods to NServiceBus.Configure.
    /// </summary>
    public static class ConfigureUnicastBus
    {
        /// <summary>
        /// Use unicast messaging (your best option on nServiceBus right now).
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigUnicastBus UnicastBus(this Configure config)
        {
            Instance = new ConfigUnicastBus();
            Instance.Configure(config);

            return Instance;
        }
        /// <summary>
        /// Return Timeout Manager Address. Uses "TimeoutManagerAddress" parameter form config file if defined, if not, uses "EndpointName.Timeouts".
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Address GetTimeoutManagerAddress(this Configure config)
        {
            var unicastConfig = Configure.GetConfigSection<UnicastBusConfig>();

            if ((unicastConfig != null) && (!String.IsNullOrWhiteSpace(unicastConfig.TimeoutManagerAddress)))
                return Address.Parse(unicastConfig.TimeoutManagerAddress);
            
            return Address.Parse(Configure.EndpointName).SubScope("Timeouts");
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters
        /// </summary>
        /// <returns></returns>
        public static Configure EnablePerformanceCounters(this Configure config)
        {
            performanceCountersEnabled = true;
            return config;
        }

        /// <summary>
        /// True id performance counters are enabled
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool PerformanceCountersEnabled(this Configure config)
        {
            return performanceCountersEnabled;
        }

        static bool performanceCountersEnabled;

        internal static ConfigUnicastBus Instance { get; private set; }
    }

    class EnsureLoadMessageHandlersWasCalled : INeedInitialization
    {
        void INeedInitialization.Init()
        {
            if (ConfigureUnicastBus.Instance != null)
                if (!ConfigureUnicastBus.Instance.LoadMessageHandlersCalled)
                    ConfigureUnicastBus.Instance.LoadMessageHandlers();
        }
    }
}
