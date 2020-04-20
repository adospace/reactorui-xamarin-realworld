using Microsoft.Extensions.Caching.Memory;
using RealWorld.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinReactorUI;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace RxRealWorld.Components.Shared
{
    public class ImageComponentState : IState
    {
        public ImageSource ImageSource { get; set; }
    }

    public class ImageComponent : RxComponent<ImageComponentState>
    {
        private Uri _source;
        private static readonly HttpClient _httpClient = new HttpClient();

        public ImageComponent Source(Uri source)
        {
            _source = source;
            return this;
        }

        protected override void OnMounted()
        {
            Task.Run(async () =>
            {
                try
                {
                    var imageBytes = await _httpClient.GetByteArrayAsync(_source);

                    SetState(s =>
                    {
                        s.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes.ToArray()));
                    });
                }
                catch (Exception)
                { }
            });

            base.OnMounted();
        }

        public override VisualNode Render()
        {
            if (State.ImageSource == null)
            {
                return new RxBoxView()
                    .WidthRequest(48)
                    .HeightRequest(48);
            }

            return new RxImage()
                .Source(State.ImageSource)
                .WidthRequest(48)
                .HeightRequest(48)
                .Aspect(Aspect.AspectFit);
        }
    }
}
