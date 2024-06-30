using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace CompareNbt.ViewModels;

public class CompareModel : ObservableObject
{
    private string changesSummary = "";

    public CompareFile LeftFile { get; } = new();
    public CompareFile RightFile { get; } = new();

    public string ChangesSummary
    {
        get => changesSummary;
        set
        {
            if (value != ChangesSummary)
            {
                changesSummary = value;
                OnPropertyChanged();
            }
        }
    }

    public CompareModel()
    {
    }

    public CompareModel(string[] args)
    {
        if (args.Length != 0)
        {
            if (args.Length != 2)
                throw new ArgumentException("Expected 2 arguments: <left file> and <right file>");

            LeftFile.SetFile(args[0]);
            RightFile.SetFile(args[1]);

            HashSet<string> seenChanges = [];
            RightFile.Root.ProcessDifferences(LeftFile.Root, seenChanges);

            bool modifications = seenChanges.Contains("*");
            bool additions = seenChanges.Contains("+");
            bool removals = seenChanges.Contains("-");
            if (seenChanges.Count == 0)
            {
                ChangesSummary = "No changes have been found.";
            }
            else if (modifications && !additions && !removals)
            {
                ChangesSummary = "Some values have changed.";
            }
            else if (additions && !removals && !modifications)
            {
                ChangesSummary = "Right side has additional values.";
            }
            else if (removals && !additions && !modifications)
            {
                ChangesSummary = "Left side has additional values.";
            }
            else
            {
                ChangesSummary = "Multiple things have changed.";
            }
        }
    }

}
