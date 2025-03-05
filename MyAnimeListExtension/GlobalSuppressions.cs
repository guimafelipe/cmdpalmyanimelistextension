// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Public method. We don't want to acess static methods and create unecessary coupling.", Scope = "member", Target = "~M:MyAnimeListExtension.Authentication.OAuthClient")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~M:MyAnimeListExtension.Authentication.OAuthClient.CreateOAuthRequestUri~System.Uri")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~M:MyAnimeListExtension.Authentication.OAuthClient.CreateRequestTokenUri~System.Uri")]
