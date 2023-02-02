using System;
namespace AuroraControls;

public static class AuroraControlBuilder
{

    public static MauiAppBuilder ConfigureAuroraControls(this MauiAppBuilder mauiAppBuilder)
    {
        return mauiAppBuilder
            .ConfigureMauiHandlers(
                mauiHandlersCollection =>
                {
                    mauiHandlersCollection.AddHandler(typeof(FloatLabelEntry), typeof(FloatLabelEntryHandler));
                });
    }
}

