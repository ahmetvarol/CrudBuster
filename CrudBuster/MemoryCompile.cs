using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CrudBuster;

public static class MemoryCompile
{
    public static Assembly CompileToAssembly(string ViewModelOutputPath)
    {
       var sourceCodes = GetFiles(ViewModelOutputPath);
        
        var syntaxTrees = sourceCodes.Select(code =>
            CSharpSyntaxTree.ParseText(code)).ToList();
       
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();
        
        var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString())
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTrees);
    
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
    
        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
            foreach (var diagnostic in failures)
                Console.WriteLine(diagnostic.ToString());
    
            return null;
        }
    
        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    public static List<string> GetFiles(string ViewModelOutputPath)
    {
        var csFiles = Directory.GetFiles(ViewModelOutputPath, "*.cs", SearchOption.AllDirectories);
        var sourceCodes = csFiles.Select(File.ReadAllText).ToList();

        return sourceCodes;
    }
}