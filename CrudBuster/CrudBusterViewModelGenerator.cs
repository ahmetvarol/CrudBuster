using System.Reflection;
using System.Text;

namespace CrudBuster;

public static class CrudBusterViewModelGenerator
{
    public static void GenerateDtoFromEntity(Type entityType, string newClassName, string viewModelOutputPath, string entityName, string viewModelPattern, string viewModelType)
    {
        var sb = new StringBuilder();

        sb.AppendLine("public class " + newClassName);
        sb.AppendLine("{");

        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
       
        var usedNamespaces = entityType.GetProperties().Select(p => p.PropertyType.Namespace).Where(ns => ns != null && ns != "System").Distinct();
        foreach (var ns in usedNamespaces)
        {
            sb.AppendLine($"using {ns};");
        }
        sb.AppendLine();

        sb.AppendLine($"public class {newClassName}");
        sb.AppendLine("{");
        
        foreach (var prop in properties)
        {
            sb.AppendLine($"    public {prop.PropertyType.Name} {prop.Name} {{ get; set; }}");
        }

        sb.AppendLine("}");
        
        MakeViewModel(viewModelOutputPath, entityName, viewModelPattern, viewModelType,sb.ToString());
    }

    public static void MakeViewModel(string viewModelOutputPath, string entityName, string viewModelPattern, string viewModelType, string outputCode)
    {
        if (!Directory.Exists($"{viewModelOutputPath}/{entityName}ViewModels"))
            Directory.CreateDirectory($"{viewModelOutputPath}/{entityName}ViewModels");
        
        string file  = System.IO.Path.Combine($"{viewModelOutputPath}/{entityName}ViewModels", $"{entityName}{viewModelType}{viewModelPattern}.cs");
        File.WriteAllText(file, outputCode);
    }
}