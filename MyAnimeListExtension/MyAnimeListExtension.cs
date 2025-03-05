// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace MyAnimeListExtension;

[ComVisible(true)]
[Guid("96f4e58e-05fc-49b7-8b36-a00b3ed40f90")]
[ComDefaultInterface(typeof(IExtension))]
public sealed partial class MyAnimeListExtension : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;

    private readonly CommandProvider _provider;

    public MyAnimeListExtension(ManualResetEvent extensionDisposedEvent, CommandProvider provider)
    {
        _extensionDisposedEvent = extensionDisposedEvent;
        _provider = provider;
    }

    public object? GetProvider(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Commands => _provider,
            _ => null,
        };
    }

    public void Dispose() => this._extensionDisposedEvent.Set();
}
