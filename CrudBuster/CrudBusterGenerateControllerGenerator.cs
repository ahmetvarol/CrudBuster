using System.Text;

namespace CrudBuster;

public static class CrudBusterGenerateControllerGenerator
{
    public static void GenerateController(string Entity ,string CreateViewModel, string UpdateViewModel, string DeleteViewModel, string GetViewModel, string ListViewModel, CrudOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace WebAPI.BaseControllers;");
        
        sb.AppendLine("[Route(\"[controller]\")]");
        sb.AppendLine("[ApiController]");
        
        if(options.IsAuthenticateRequired)
            sb.AppendLine("[Authorize]");

        
        sb.AppendLine($"public class {Entity}Controller : BaseController");
        sb.AppendLine("{");
        
        sb.AppendLine($"    public {Entity}Controller()");
        
        
        sb.AppendLine("}");
        
        
        
        
        
    }
}