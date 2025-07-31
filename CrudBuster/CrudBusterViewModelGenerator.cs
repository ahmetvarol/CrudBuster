using System.Reflection;
using System.Text;

namespace CrudBuster;

public static class CrudBusterViewModelGenerator
{
    public static bool GenerateDtoFromEntity(Type entityType, string newClassName, string viewModelOutputPath, string entityName, string viewModelPattern, string viewModelType, string domainAssamblyName)
    {
        var sb = new StringBuilder();

        //namespace WebApplication1.ViewModels.CategoryViewModels;
        sb.AppendLine($"namespace {domainAssamblyName}.ViewModels.{entityName}ViewModels;");
        

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
            string propName = prop.PropertyType.IsEnum
                ? prop.PropertyType.Name
                : prop.PropertyType.Name.ToLower();
            
            sb.AppendLine($"    public {propName} {prop.Name} {{ get; set; }}");
        }

        sb.AppendLine("}");
        
        bool status = MakeViewModel(viewModelOutputPath, entityName, viewModelPattern, viewModelType,sb.ToString());
        return status;
    }

    public static bool MakeViewModel(string viewModelOutputPath, string entityName, string viewModelPattern, string viewModelType, string outputCode)
    {
        if (!Directory.Exists($"{viewModelOutputPath}/{entityName}ViewModels"))
            Directory.CreateDirectory($"{viewModelOutputPath}/{entityName}ViewModels");

        if (!File.Exists($"{viewModelOutputPath}/{entityName}ViewModels/{entityName}{viewModelType}{viewModelPattern}/{entityName}{viewModelType}{viewModelPattern}.cs"))
        {
            string file  = System.IO.Path.Combine($"{viewModelOutputPath}/{entityName}ViewModels", $"{entityName}{viewModelType}{viewModelPattern}.cs");
            File.WriteAllText(file, outputCode);
            return true;
        }

        return false;
    }
}