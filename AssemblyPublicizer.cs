using Mono.Cecil;

namespace QuickAssemblyPublicizer;

public sealed class AssemblyPublicizer
{
    private readonly ILogger _logger;

    public AssemblyPublicizer(ILogger logger)
    {
        this._logger = logger;
    }

    public void Publicize(string inputFilePath, string outputFilePath)
    {
        try
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(inputFilePath) ?? ".");

            var readSymbols = ShouldReadSymbols(inputFilePath);
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = readSymbols,
                ThrowIfSymbolsAreNotMatching = false,
                AssemblyResolver = assemblyResolver
            };

            using var assemblyDefinition = AssemblyDefinition.ReadAssembly(inputFilePath, readerParameters);
            MakeScopePublic(assemblyDefinition.MainModule);

            var writerParameters = new WriterParameters { WriteSymbols = assemblyDefinition.MainModule.HasSymbols };
            assemblyDefinition.Write(outputFilePath, writerParameters);

            _logger.Info($"Publicized '{inputFilePath}' -> '{outputFilePath}'");
        }
        catch (AssemblyResolutionException exception)
        {
            _logger.Warn($"Could not resolve reference '{exception.AssemblyReference.FullName}' - continuing anyway");
            
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(inputFilePath) ?? ".");
            
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = false,
                ThrowIfSymbolsAreNotMatching = false,
                AssemblyResolver = assemblyResolver,
                ReadingMode = ReadingMode.Deferred
            };

            using var assemblyDefinition = AssemblyDefinition.ReadAssembly(inputFilePath, readerParameters);
            MakeScopePublic(assemblyDefinition.MainModule);

            var writerParameters = new WriterParameters { WriteSymbols = false };
            assemblyDefinition.Write(outputFilePath, writerParameters);

            _logger.Info($"Publicized '{inputFilePath}' -> '{outputFilePath}'");
        }
    }

    private static bool ShouldReadSymbols(string inputFilePath)
    {
        var pdbFilePath = Path.ChangeExtension(inputFilePath, ".pdb");
        return File.Exists(pdbFilePath);
    }

    private static void MakeScopePublic(ModuleDefinition module)
    {
        foreach (var typeDefinition in module.Types)
        {
            if (typeDefinition.Name == "<Module>")
            {
                continue;
            }

            MakeTypePublic(typeDefinition);
        }
    }

    private static void MakeTypePublic(TypeDefinition typeDefinition)
    {
        typeDefinition.Attributes = typeDefinition.IsNested
            ? (typeDefinition.Attributes & ~TypeAttributes.VisibilityMask) | TypeAttributes.NestedPublic
            : (typeDefinition.Attributes & ~TypeAttributes.VisibilityMask) | TypeAttributes.Public;

        foreach (var nestedType in typeDefinition.NestedTypes)
        {
            MakeTypePublic(nestedType);
        }

        foreach (var methodDefinition in typeDefinition.Methods)
        {
            MakeMethodPublic(methodDefinition);
        }

        foreach (var fieldDefinition in typeDefinition.Fields)
        {
            MakeFieldPublic(fieldDefinition);
        }

        foreach (var propertyDefinition in typeDefinition.Properties)
        {
            MakePropertyPublic(propertyDefinition);
        }
    }

    private static void MakeMethodPublic(MethodDefinition methodDefinition)
    {
        methodDefinition.Attributes = (methodDefinition.Attributes & ~MethodAttributes.MemberAccessMask) | MethodAttributes.Public;
    }

    private static void MakeFieldPublic(FieldDefinition fieldDefinition)
    {
        fieldDefinition.Attributes = (fieldDefinition.Attributes & ~FieldAttributes.FieldAccessMask) | FieldAttributes.Public;
    }

    private static void MakePropertyPublic(PropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.GetMethod is not null)
        {
            MakeMethodPublic(propertyDefinition.GetMethod);
        }

        if (propertyDefinition.SetMethod is not null)
        {
            MakeMethodPublic(propertyDefinition.SetMethod);
        }
    }
}
