﻿using System.Collections.Generic;

namespace DotNet.Versions
{
    public record ReleaseDetails(
        string ChannelVersion,
        string LatestRelease,
        string LatestReleaseDate,
        string LatestRuntime,
        string LatestSdk,
        string SupportPhase,
        string LifeCyclePolicy,
        IEnumerable<Release> Releases,
        IntelliSense IntelliSense);

    public record IntelliSense(
        string Version,
        string VersionDisplay,
        IEnumerable<File> Files);

    public record File(
        string Lang,
        string Name,
        string Rid,
        string Url,
        string Hash,
        string AkaMs);

    public record Release(
        string ReleaseDate,
        string ReleaseVersion,
        bool Security,
        IEnumerable<CveList> CveList,
        string ReleaseNotes,
        Runtime Runtime,
        Sdk Sdk,
        IEnumerable<Sdk> Sdks,
        AspnetcoreRuntime AspnetcoreRuntime,
        WindowsDesktop WindowsDesktop,
        Symbols Symbols);

    public record Runtime(
        string Version,
        string VersionDisplay,
        string VsVersion,
        string VsMacVersion,
        IEnumerable<File> Files);

    public record Sdk(
        string Version,
        string VersionDisplay,
        string RuntimeVersion,
        string VsVersion,
        string VsMacVersion,
        string VsSupport,
        string VsMacSupport,
        string CSharpVersion,
        string FSharpVersion,
        string VBVersion,
        IEnumerable<File> Files);

    public record AspnetcoreRuntime(
        string Version,
        string VersionDisplay,
        IEnumerable<string> VersionAspNetCoreModule,
        string VsVersion,
        IEnumerable<File> Files);

    public record WindowsDesktop(
        string Version,
        string VersionDisplay,
        IEnumerable<File> Files);

    public record Symbols(
        string Version,
        IEnumerable<File> Files);

    public record CveList(
        string CveId,
        string CveUrl);
}