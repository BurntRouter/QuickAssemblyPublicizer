using QuickAssemblyPublicizer;

var logger = new ConsoleLogger();

if (args.Length != 2)
{
    logger.Error("Usage: QuickAssemblyPublicizer <input path> <output path>");
    return;
}

var inputPath = args[0];
var outputPath = args[1];

try
{
    var assemblyPublicizer = new AssemblyPublicizer(logger);
    var fileSystemProcessor = new FileSystemProcessor(assemblyPublicizer);
    fileSystemProcessor.Process(inputPath, outputPath);
    logger.Info("Finished processing.");
}
catch (Exception exception)
{
    logger.Error(exception.Message);
    Environment.ExitCode = 1;
}