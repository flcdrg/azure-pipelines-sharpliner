using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps;
using Sharpliner;

namespace Pipelines;

public class EachWithParameterPipeline : PipelineDefinition
{
    private static readonly ObjectParameter Stages = ObjectParameter("Stages", "Environment names (do not edit)", new ConditionedDictionary()
    {
        { "Dev", string.Empty}
    });


    public override string TargetFile => "each-pipeline.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override Pipeline Pipeline => new()
    {
        Parameters =
        [
            Stages
        ],


        Stages = [
            Each("stage", "parameters.Stages")
                .Stage(new Stage("${{ stage }}")
                {

                    Jobs = [
                        new Job("Example")
                                {
                                    DisplayName = If.IsBranch("main")
                                            .Value("job_1")
                                            .Else
                                            .Value("job_2")
                                            .EndIf,

                            Steps = [
                                Powershell.Inline("Write-Host 'Hello-World'").DisplayAs("Hello world")
                            ]
                            }
                        ]
                })
        ]
    };
}