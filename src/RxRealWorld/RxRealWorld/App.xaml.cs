using Microsoft.Extensions.DependencyInjection;
using RealWorld.Services;
using RxRealWorld.Components;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinReactorUI;
using XamarinReactorUI.HotReload;

namespace RxRealWorld
{
    public partial class App : Application
    {
        private readonly RxApplication _app;

        public App()
        {
            InitializeComponent();


            _app = RxApplication.Create<ShellComponent>(this)
                .WithServices()
                .WithHotReload();
        }

        protected override void OnStart() => _app.Run();

        protected override void OnSleep() => _app.Stop();

        protected override void OnResume() => _app.Run();
    }
}
