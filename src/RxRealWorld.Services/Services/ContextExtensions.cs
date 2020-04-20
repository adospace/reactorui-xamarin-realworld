using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XamarinReactorUI;

namespace RealWorld.Services
{
    public static class ContextExtensions
    {
        public static IServiceProvider ServiceProvider(this RxContext context) => context.Get<IServiceProvider>(nameof(ServiceProvider));

        public static RxApplication WithServices(this RxApplication application)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddServices();

            return application
                .WithContext(nameof(ServiceProvider), serviceCollection.BuildServiceProvider());
        }
    }
}
