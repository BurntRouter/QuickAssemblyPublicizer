namespace QuickAssemblyPublicizer;

public sealed class FileSystemProcessor
{
    private readonly AssemblyPublicizer _assemblyPublicizer;

    public FileSystemProcessor(AssemblyPublicizer assemblyPublicizer)
    {
        this._assemblyPublicizer = assemblyPublicizer;
    }

    public void Process(string inputPath, string outputPath)
    {
        if (File.Exists(inputPath))
        {
            ProcessSingleFile(inputPath, outputPath);
            return;
        }

        if (Directory.Exists(inputPath))
        {
            ProcessDirectory(inputPath, outputPath);
            return;
        }

        throw new FileNotFoundException($"Input path '{inputPath}' not found.");
    }

    private void ProcessSingleFile(string inputFilePath, string outputFilePath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath) ?? outputFilePath);
            _assemblyPublicizer.Publicize(inputFilePath, outputFilePath);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to process '{inputFilePath}': {exception.Message}", exception);
        }
    }

    private void ProcessDirectory(string inputDirectoryPath, string outputDirectoryPath)
    {
        foreach (var filePath in Directory.EnumerateFiles(inputDirectoryPath, "*.dll", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(inputDirectoryPath, filePath);
            var outputFilePath = Path.Combine(outputDirectoryPath, relativePath);
            ProcessSingleFile(filePath, outputFilePath);
        }
    }
}

