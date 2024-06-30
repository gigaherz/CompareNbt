using CommunityToolkit.Mvvm.ComponentModel;
using CompareNbt.Parsing;
using System.Collections.Generic;

namespace CompareNbt.ViewModels;

public class CompareFile : ObservableObject
{
    public string FileName { get; private set; } = "Not Loaded";

    public CompareTag Root { get; private set; } = new CompareTag(null);

    public List<CompareTag> ChildTags { get; private set; } = [];

    public void SetFile(string filePath)
    {
        var file = new NbtFile(filePath);
        Root = new CompareTag(file.RootTag);
        FileName = filePath;
        ChildTags = new List<CompareTag>() { Root };
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(Root));
        OnPropertyChanged(nameof(ChildTags));
    }
}
