﻿// Copyright 2016 Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Autofac;
using Nancy;
using Seq.Forwarder.Config;
using Seq.Forwarder.ServiceProcess;
using Seq.Forwarder.Shipper;
using Seq.Forwarder.Storage;
using Seq.Forwarder.Web.Formats;
using Seq.Forwarder.Web.Host;

namespace Seq.Forwarder
{
    class SeqForwarderModule : Module
    {
        readonly string _bufferPath;
        readonly SeqForwarderConfig _config;

        public SeqForwarderModule(string bufferPath, SeqForwarderConfig config)
        {
            if (bufferPath == null) throw new ArgumentNullException(nameof(bufferPath));
            if (config == null) throw new ArgumentNullException(nameof(config));
            _bufferPath = bufferPath;
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonNetSerializer>().As<ISerializer>();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<INancyModule>()
                .As<INancyModule>()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterType<ServerService>().SingleInstance();
            builder.RegisterType<NancyBootstrapper>();
            builder.Register(c => new LogBuffer(_bufferPath, _config.Storage.BufferSizeBytes)).SingleInstance();
            builder.RegisterType<HttpLogShipper>().SingleInstance();
            builder.RegisterInstance(_config.Output);
        }
    }
}
