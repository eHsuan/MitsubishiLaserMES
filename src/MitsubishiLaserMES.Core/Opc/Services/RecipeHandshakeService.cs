using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Models;
using MitsubishiLaserOpc.Nodes;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaserOpc.Services
{

/// <summary>Writes a recipe in the required order. Device state waiting is added by the OPC subscription layer.</summary>
public sealed class RecipeHandshakeService
{
    private readonly NodeDiagnosticService _nodes;
    public RecipeHandshakeService(NodeDiagnosticService nodes) => _nodes = nodes;

    public async Task<OperationResult> DeliverRecipeAsync(RecipeDefinition recipe, CancellationToken cancellationToken = default(CancellationToken))
    {
        foreach (var write in new (LaserOpcNode Node, object Value)[]
        {
            (LaserOpcNode.RecipeProgramFile, recipe.ProgramFile),
            (LaserOpcNode.RecipeConditionFile, recipe.ConditionFile),
            (LaserOpcNode.RecipeSheetNum, recipe.SheetCount),
            (LaserOpcNode.GetRecipeAck, (short)1),
        })
        {
            var result = await _nodes.WriteAsync(write.Node, write.Value, cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded) return result;
        }
        return OperationResult.Success();
    }

    public Task<OperationResult> ClearRecipeAckAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
        _nodes.WriteAsync(LaserOpcNode.GetRecipeAck, (short)0, cancellationToken);

    public Task<OperationResult> StartAutoScheduleAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
        _nodes.WriteAsync(LaserOpcNode.StartScheduleRequest, true, cancellationToken);
}
}
