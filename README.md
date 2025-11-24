# QuickAssemblyPublicizer

A command-line tool for making all types, methods, fields, and properties in .NET assemblies public. This is useful for reflection-heavy scenarios, testing internal code, or when you need access to private/internal members of third-party libraries.

## What it does

The tool takes a .NET assembly (.dll file) and creates a modified version where:
- All types (classes, interfaces, structs, etc.) are made public
- All methods are made public
- All fields are made public  
- All properties (getters and setters) are made public
- Nested types are properly handled with public accessibility

The original assembly is left unchanged - a new publicized version is created at the specified output location.

## Requirements

- .NET 10.0 or later

## Installation

### Pre-built binaries

Download the latest release for your platform from the [Releases](../../releases) page. Pre-built binaries are available for:
- Windows (x64, ARM64)
- Linux (x64, ARM64) 
- macOS (x64, ARM64)

### Build from source

Clone this repository and build the project:

```bash
git clone https://github.com/yourusername/QuickAssemblyPublicizer.git
cd QuickAssemblyPublicizer
dotnet build -c Release
```

The compiled executable will be available in `bin/Release/net10.0/`.

## Usage

### Basic usage

```bash
QuickAssemblyPublicizer <input_path> <output_path>
```

### Examples

Publicize a single assembly:
```bash
QuickAssemblyPublicizer "MyLibrary.dll" "MyLibrary_publicized.dll"
```

Publicize all assemblies in a directory:
```bash
QuickAssemblyPublicizer "input_folder/" "output_folder/"
```

When processing a directory, the tool will:
- Find all `.dll` files recursively
- Maintain the same directory structure in the output folder
- Process each assembly individually

## How it works

The tool uses Mono.Cecil to read and modify .NET assemblies at the IL level. It:

1. Loads the input assembly with its symbol information (PDB files) if available
2. Iterates through all types in the assembly
3. Changes visibility modifiers to public for types, methods, fields, and properties
4. Preserves debug symbols when possible
5. Writes the modified assembly to the output location

The tool handles various edge cases like:
- Missing assembly references (continues processing with warnings)
- Nested types (properly sets NestedPublic visibility)
- Symbol files (preserves PDB files when available)
- Different assembly structures

## Error handling

- If an assembly reference cannot be resolved, the tool logs a warning but continues processing
- If symbol files are missing or corrupted, processing continues without them
- File system errors (permissions, disk space, etc.) are reported with clear error messages
- The exit code is set to 1 if any errors occur during processing

## Limitations

- Only processes .NET assemblies (.dll files)
- Does not modify strong-name signatures (publicized assemblies may need re-signing)
- Some runtime checks in the original code may behave differently with public visibility
- Obfuscated assemblies may not process correctly

## Use cases

- **Testing**: Access private/internal members for unit testing
- **Reflection**: Simplify reflection code by avoiding complex binding flags
- **Debugging**: Inspect internal state of third-party libraries
- **Modding**: Modify behavior of existing applications (when legally permitted)
- **Research**: Analyze internal implementations of libraries

## Building from source

```bash
dotnet restore
dotnet build -c Release
```

For a self-contained executable:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Replace `win-x64` with your target runtime identifier (e.g., `linux-x64`, `osx-arm64`).

## Contributing

This is a straightforward tool with a focused purpose. If you encounter bugs or have suggestions for improvements, please open an issue or submit a pull request.