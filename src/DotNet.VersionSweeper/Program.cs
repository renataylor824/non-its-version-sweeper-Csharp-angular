﻿using CommandLine;
using DotNet.Extensions;
using DotNet.GitHub;
using DotNet.Releases;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CommandLine.Parser;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDotNetGitHubServices()
                .AddDotNetVersionServices())
    .Build();

var parser = Default.ParseArguments<Options>(args);

static void HandleParseError(IEnumerable<Error> errors)
{
    foreach (var error in errors)
    {
        Console.WriteLine(error);
    }
}

async Task StartSweeperAsync(Options options, IServiceProvider services)
{
    var projectReader = services.GetRequiredService<IProjectFileReader>();
    DirectoryInfo directory = new(options.Directory);
    ConcurrentDictionary<string, string[]> projects = new(StringComparer.OrdinalIgnoreCase);

    await directory.EnumerateFiles(options.SearchPattern, SearchOption.AllDirectories)
        .ForEachAsync(
            Environment.ProcessorCount,
            async fileInfo =>
            {
                var path = fileInfo.FullName;
                var tfms = await projectReader.ReadProjectTfmsAsync(path);
                if (tfms is { Length: > 0 })
                {
                    projects.TryAdd(path, tfms);
                }
            });

    if (projects is not { IsEmpty: true })
    {
        var unsupportedProjectReporter =
            services.GetRequiredService<IUnsupportedProjectReporter>();
        var gitHubIssueService =
            services.GetRequiredService<IGitHubIssueService>();

        foreach (var (projectPath, tfms) in projects)
        {
            await foreach (var projectSupportReport
                in unsupportedProjectReporter.ReportAsync(projectPath, tfms))
            {
                var (proj, reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    Console.WriteLine(projectSupportReport.ToMarkdownBody());
                }
            }
        }
    }
}

parser.WithNotParsed(HandleParseError);

await parser.WithParsedAsync(
    options => StartSweeperAsync(options, host.Services));

await host.RunAsync();
