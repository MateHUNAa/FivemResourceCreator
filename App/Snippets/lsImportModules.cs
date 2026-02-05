using fvm.Interfaces;

namespace fvm.Snippets
{
    public class LsImportModulesSnippet : ILuaSnippet
    {
        public string Name => "lsLogger";
        public string TargetFile => "server/main.lua";

        private string CodeTemplate = @"
local function importModules()
     Logger = LS:ImportModule('Logger')
end

AddEventHandler('{resourceName}:Shared:DependencyUpdated', importModules)
AddEventHandler('Core:Shared:Ready', function()
     LS:ImportDependencies('{resourceName}', {
          'Logger',
     }, function(errors)
          if #errors > 0 then
               return
          end

          importModules()
     end)
end)

AddEventHandler(""Core:Shared:RegisterReady"", function() 
    -- RegisterModule

end) 
";

        public string GetCode(Dictionary<string, string> placeholders)
        {
            string code = CodeTemplate;
            foreach (var kvp in placeholders)
            {
                code = code.Replace("{" + kvp.Key + "}", kvp.Value);
            }
            return code;
        }
    }
}
