using App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Snippets
{
    public class NuiSnippet : ILuaSnippet
    {
        public string Name => "NUI Integration";
        public string TargetFile => "client/main.lua";

        private string CodeTemplate = @"
function sendNUI(action, data)
     if type(data) ~= 'table' then
          data = {}
     end
     SendNUIMessage({
          action = action,
          data   = data
     })
end

function nuiCallback(name, callback)
     RegisterNUICallback(name, function(data, cb)
          callback(data, cb)
     end)
end

function nuiServerCallback(name, otherParams)
     nuiCallback(name, (function(params, cb)
          lib.callback((""{resourceName}:%s""):format(name), false, (function(result)
               local ok, err = pcall(function(...)
                    if result.msg and result.msgTyp ~= nil then
                         mCore.Notify(lang.Title, result.msg, result.msgTyp, 5000)
                    elseif result.msg then
                         mCore.Notify(lang.Title, result.msg, result.err and ""error"" or ""info"", 5000)
                    end
               end)

               if not ok then
                    Logger:Error((""[%s]""):format(name), err)
               end

               cb(result)
          end), params, otherParams and otherParams())
     end))
end

nuiCallback(""exit"", (function(_, cb)
     visible = false
     SetNuiFocus(false, false)
     cb(""ok"")
end))

exports(""Close"", function(...)
     visible = false
     SetNuiFocus(false, false)
     sendNUI(""close"")
end)

RegisterCommand('{resourceName}', function (source, args, raw)
    exports['{resourceName}']:Open()
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
