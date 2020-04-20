using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealWorld.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IApiService, Impl.ApiService>();
            serviceCollection.AddTransient<IArticleService, Impl.ArticleService>();
            serviceCollection.AddTransient<ICommentService, Impl.CommentService>();
            serviceCollection.AddTransient<IProfileService, Impl.ProfileService>();
            serviceCollection.AddTransient<ITagService, Impl.TagService>();
            serviceCollection.AddTransient<IUserService, Impl.UserService>();

            serviceCollection.AddSingleton<IMemoryCache, MemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));
        }
    }
}
