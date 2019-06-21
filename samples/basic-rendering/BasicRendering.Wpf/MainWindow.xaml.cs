﻿using ExampleFigma;
using FigmaSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicRendering.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ExampleViewManager manager;

        public MainWindow()
        {
            InitializeComponent();

            var scrollViewWrapper = new ScrollViewWrapper(ContainerPanel);
            var container = new ViewWrapper(new Canvas());
            manager = new ExampleViewManager(scrollViewWrapper, container);
        }
    }
}