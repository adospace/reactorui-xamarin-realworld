using RealWorld.Models;
using RealWorld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamarinReactorUI;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XamarinReactorUI.Utils;
using RxRealWorld.Components.Shared;

namespace RxRealWorld.Components
{
    public class HomeComponentState : IState
    { 
        public bool IsLoading { get; set; }

        public bool IsRefreshing { get; set; }

        public ObservableCollection<Article> Articles { get; set; } = new ObservableCollection<Article>();
    }

    public class HomeComponent : RxComponent<HomeComponentState>
    {
        protected override void OnMounted()
        {
            LoadArticles();

            base.OnMounted();
        }

        private async void LoadArticles()
        {
            if (State.IsLoading)
                return;

            SetState(s => s.IsLoading = true);

            System.Diagnostics.Debug.WriteLine($"Loading articles from offset {State.Articles.Count}...");

            var articleService = Context.ServiceProvider().GetService<IArticleService>();

            var articlesDownloaded = await articleService.ListAsync(offset: State.Articles.Count);
            System.Diagnostics.Debug.WriteLine($"Load completed articles {articlesDownloaded.articles.Length}/{articlesDownloaded.articlesCount}");

            SetState(s => 
            {
                //s.Articles.AddRange(articlesDownloaded.articles);
                foreach (var article in articlesDownloaded.articles)
                    s.Articles.Add(article);
                s.IsLoading = false;
                s.IsRefreshing = false;
            });
        }

        private void RefreshList()
        {
            if (State.IsRefreshing)
                return;

            SetState(_ =>
            {
                _.IsRefreshing = true;
                _.Articles.Clear();
            });

            LoadArticles();
        }

        public override VisualNode Render()
        {
            return new RxContentPage()
            {
                new RxGrid()
                {
                    RenderArticles(),
                    RenderIsBusy()
                }                
            }
            .Title("Conduit");
        }

        private VisualNode RenderIsBusy()
        {
            return new RxActivityIndicator()
                .IsRunning(true)
                .IsVisible(State.IsLoading && !State.IsRefreshing)
                .VCenter()
                .HCenter();
        }

        private VisualNode RenderArticles()
        {
            return new RxRefreshView()
            {
                new RxCollectionView<Article>()
                    .RenderCollection(State.Articles, RenderArticle)
                    .RemainingItemsThreshold(10)
                    .OnRemainingItemsThresholdReached(LoadArticles)
                    .ItemSizingStrategy(ItemSizingStrategy.MeasureFirstItem)
            }
            .OnRefresh(RefreshList)
            .IsRefreshing(State.IsRefreshing);
        }

        //Version using stack layout (less performant)
        //private VisualNode RenderArticle(Article article)
        //{
        //    return new RxFrame()
        //    {
        //        new RxStackLayout()
        //        {
        //            new RxLabel(article.title)
        //                .FontSize(NamedSize.Large),
        //            new RxLabel(article.description)
        //                .Margin(0,5)
        //                .VFillAndExpand(),
        //            new RxStackLayout()
        //            {
        //                RenderAuthorIcon(article.author),
        //                new RxStackLayout()
        //                {
        //                    new RxLabel(article.author.username),
        //                    new RxLabel(article.createdAt.ToShortDateString())
        //                }
        //            }
        //            .Margin(0,10,0,0)
        //            .WithHorizontalOrientation()
        //            .VEnd()
        //        }
        //    }
        //    .BorderColor(Color.Gray)
        //    .CornerRadius(2)
        //    .Padding(10, 10)
        //    .Margin(0, 5);
        //}

        private VisualNode RenderArticle(Article article)
        {
            return new RxFrame()
            {
                new RxGrid("32 72 48", "48 *")
                {
                    new RxLabel(article.title)
                        .FontSize(NamedSize.Large)
                        .GridColumnSpan(2),
                    new RxLabel(article.description)
                        .Margin(0,5)
                        .VFillAndExpand()
                        .GridRow(1)
                        .GridColumnSpan(2),
                    RenderAuthorIcon(article.author),
                    //new RxStackLayout()
                    //{
                    //    new RxLabel(article.author.username),
                    //    new RxLabel(article.createdAt.ToShortDateString())
                    //}
                    //.GridRow(2)
                    //.GridColumn(1)
                }
            }
            .BorderColor(Color.Gray)
            .CornerRadius(2)
            .Padding(10, 10)
            .Margin(0, 5);
        }

        private VisualNode RenderAuthorIcon(Profile profile)
        {
            if (Uri.TryCreate(profile.image, UriKind.Absolute, out var userImageUri) &&
                userImageUri.IsAbsoluteUri)
            {
                return new RxImage()
                    .Source(userImageUri, cachingEnabled: true, cacheValidity: TimeSpan.FromDays(10))
                    .GridRow(2)
                    .WidthRequest(48)
                    .HeightRequest(48)
                    .Aspect(Aspect.AspectFit);

                //Uncomment the following to use the ImageComponent instead
                //return new ImageComponent()
                //    .Source(userImageUri);
            }

            return null;
        }
    }
}
