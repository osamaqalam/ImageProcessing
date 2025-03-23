using ImageProcessing.App.Services.Imaging;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using ImageProcessing.App.ViewModels;
using ImageProcessing.App.ViewModels.Flowchart;
using ImageProcessing.App.Services;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
        }

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register the service
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<InsertNodeDialogViewModel>();
            services.AddTransient<IFlowchartNode, FlowchartNodeViewModel>();
            services.AddTransient<LoadImageNodeViewModel>();
            
            return services.BuildServiceProvider();
        }
    }
}
