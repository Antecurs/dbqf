﻿using dbqf.Display.Preset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dbqf.WPF
{
    /// <summary>
    /// Interaction logic for PresetView.xaml
    /// </summary>
    public partial class PresetView : UserControl
    {
        public PresetAdapter<UIElement> Adapter { get; private set; }

        public PresetView(PresetAdapter<UIElement> adapter)
        {
            InitializeComponent();
            Adapter = adapter;
            this.DataContext = Adapter;
        }
    }
}
