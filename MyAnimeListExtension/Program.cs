// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace MyAnimeListExtension;

public class Program
{
    [MTAThread]
    public static async Task Main(string[] args)
    {
        Debug.WriteLine($"Launched with args: {string.Join(' ', args.ToArray())}");

        // Force the app to be single instanced.
        // Get or register the main instance.
        var mainInstance = AppInstance.FindOrRegisterForKey("mainInstance");
        var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (!mainInstance.IsCurrent)
        {
            await mainInstance.RedirectActivationToAsync(activationArgs);
            return;
        }

        // Register for activation redirection.
        AppInstance.GetCurrent().Activated += AppActivationRedirected;

        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            try
            {
                HandleCOMServerActivation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

        }
        else
        {
            Console.WriteLine("Not being launched as a Extension... exiting.");
        }
    }

    private static void AppActivationRedirected(object? sender, Microsoft.Windows.AppLifecycle.AppActivationArguments activationArgs)
    {
        // Handle COM server.
        if (activationArgs.Kind == ExtendedActivationKind.Launch)
        {
            var d = activationArgs.Data as ILaunchActivatedEventArgs;
            var args = d?.Arguments.Split();

            if (args?.Length > 1 && args[1] == "-RegisterProcessAsComServer")
            {
                HandleCOMServerActivation();
            }
        }

        // Handle Protocol.
        if (activationArgs.Kind == ExtendedActivationKind.Protocol)
        {
            var d = activationArgs.Data as IProtocolActivatedEventArgs;
            if (d is not null)
            {
                HandleProtocolActivation(d.Uri);
            }
        }
    }

    private static void HandleCOMServerActivation()
    {
        using ExtensionServer server = new();
        var extensionDisposedEvent = new ManualResetEvent(false);
        var extensionInstance = new MyAnimeListExtension(extensionDisposedEvent);


        var dataProvider = new DataProvider();

        var res = DataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        Console.WriteLine("Eu existo?");
        Console.WriteLine(res);
        Debug.WriteLine("Eu existo?");
        Debug.WriteLine(res);

        OAuthClient.BeginOAuthRequest();

        // We are instantiating an extension instance once above, and returning it every time the callback in RegisterExtension below is called.
        // This makes sure that only one instance of SampleExtension is alive, which is returned every time the host asks for the IExtension object.
        // If you want to instantiate a new instance each time the host asks, create the new instance inside the delegate.
        server.RegisterExtension(() => extensionInstance);

        // This will make the main thread wait until the event is signalled by the extension class.
        // Since we have single instance of the extension object, we exit as soon as it is disposed.
        extensionDisposedEvent.WaitOne();
    }

    private static void HandleProtocolActivation(Uri oauthRedirectUri) => _ = OAuthClient.HandleOAuthRedirection(oauthRedirectUri);
}
