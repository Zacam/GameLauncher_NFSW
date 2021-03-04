﻿using Nancy;
using Nancy.Configuration;
using System;
using System.Collections.Generic;

namespace GameLauncherSimplified.App.Classes.LauncherCore.Proxy
{
    class ProxyBootstrap : DefaultNancyBootstrapper
    {
        protected override IEnumerable<Type> ViewEngines
        {
            get { return new List<Type>(); }
        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            environment.Tracing(true, true);
        }
    }
}
