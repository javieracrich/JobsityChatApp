namespace JobsityChatApp.Extensions;
public static class ApplicationBuilderExtentions
{
    //the simplest way to store a single long-living object, just for example.
    private static IRabbitListener? Listener { get; set; }

    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        Listener = app.ApplicationServices.GetService<IRabbitListener>()!;

        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        lifetime!.ApplicationStarted.Register(OnStarted);

        //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
        lifetime.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        Listener!.Register();
    }

    private static void OnStopping()
    {
        Listener!.Deregister();
    }
}