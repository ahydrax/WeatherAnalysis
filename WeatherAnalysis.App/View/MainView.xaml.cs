﻿using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class MainView
    {
        [Inject]
        public MainViewModel MainViewModel
        {
            set { DataContext = value; }
        }

        public MainView()
        {
            InitializeComponent();
        }
    }
}
