// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Security;
using Windows.Security.Credentials;

namespace MyAnimeListExtension.Authentication;

public interface ICredentialVault
{
    PasswordCredential? GetCredentials(string loginId);

    void RemoveCredentials(string loginId);

    void SaveCredentials(string loginId, SecureString? accessToken);

    IEnumerable<string> GetAllCredentials();

    void RemoveAllCredentials();
}
