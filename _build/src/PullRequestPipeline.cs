using Sharpliner;
using Sharpliner.AzureDevOps;

namespace Pipelines;

class PullRequestPipeline : SingleStagePipelineDefinition 
{
    // Name and where to serialize the YAML of this pipeline into
    public override string TargetFile => "_build/azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    private static readonly Variable DotnetVersion = new Variable("DotnetVersion", string.Empty);

    public override SingleStagePipeline Pipeline => new()
    {
        Trigger = new Trigger("main"),
        Variables =
    [
        If.IsBranch("net-6.0")
            .Variable(DotnetVersion with { Value = "6.0.100" })
            .Group("net6-kv")
        .Else
            .Variable(DotnetVersion with { Value = "5.0.202" }),
    ],

        Jobs =
    [
        new Job("Build", "Build and test")
        {
            Pool = new HostedPool("Azure Pipelines", "windows-latest"),
            Steps =
            [
                If.IsPullRequest
                    .Step(Powershell.Inline("Write-Host 'Hello-World'").DisplayAs("Hello world")),

                DotNet.Install
                    .Sdk(DotnetVersion)
                    .DisplayAs("Install .NET SDK"),

                DotNet
                    .Build("src/MyProject.sln", includeNuGetOrg: true)
                    .DisplayAs("Build"),

                DotNet
                    .Test("src/MyProject.sln")
                    .DisplayAs("Test"),

                Script
                    .Inline("echo 'hi'")
                    .WithName("Script")
            ]
        },
        new Job("AfterBuild")
        {
            Pool = new HostedPool(vmImage: "ubuntu-latest"),
            Variables = [
                new Variable("MyVariable", "fred"),
                Group("Group Name`"),
],
            Steps = [
                Checkout.None
                ]
        }
    ],
    };
}