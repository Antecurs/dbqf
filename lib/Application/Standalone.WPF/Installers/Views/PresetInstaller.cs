﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using dbqf.Display.Preset;
using dbqf.WPF;
using dbqf.WPF.Preset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone.WPF.Installers.Views
{
    public class PresetInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<PresetView>().LifestyleSingleton(),
                Component.For<PresetAdapter<System.Windows.UIElement>>().ImplementedBy<Standalone.Core.Display.PresetAdapter<System.Windows.UIElement>>().LifestyleSingleton()
            );
        }
    }
}
