using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Models
{

public sealed class RecipeDefinition
{
    public RecipeDefinition(string recipeId, string version, string programFile, string conditionFile, short sheetCount)
    {
        if (string.IsNullOrWhiteSpace(recipeId)) throw new ArgumentException("RecipeId is required.", nameof(recipeId));
        if (string.IsNullOrWhiteSpace(programFile) || programFile.Length > 128) throw new ArgumentException("ProgramFile is required and limited to 128 characters.", nameof(programFile));
        if (string.IsNullOrWhiteSpace(conditionFile) || conditionFile.Length > 128) throw new ArgumentException("ConditionFile is required and limited to 128 characters.", nameof(conditionFile));
        if (sheetCount < -1 || sheetCount > 9999) throw new ArgumentOutOfRangeException(nameof(sheetCount));

        RecipeId = recipeId;
        Version = version;
        ProgramFile = programFile;
        ConditionFile = conditionFile;
        SheetCount = sheetCount;
    }

    public string RecipeId { get; }
    public string Version { get; }
    public string ProgramFile { get; }
    public string ConditionFile { get; }
    public short SheetCount { get; }
}
}
