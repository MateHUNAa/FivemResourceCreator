using fvm.Interfaces;

namespace fvm.Snippets
{
    public class EventHandlerSnippet : ILuaSnippet
    {
        public string Name => "EventHandler";
        public string TargetFile => "server/handlers/eventHandlers.lua";

        private const string CodeTemplate = @"
---@class EventHandlers
local EventHandlers = {}

function EventHandlers:New()
    local instance = setmetatable({}, { __index = EventHandlers })
    return instance
end

function EventHandlers:Register()
    RegisterNetEvent('{resourceName}:server:exampleEvent', function(data)
        local source = source
        LS.Logger:Info(('Event triggered by player %s'):format(source))
        
        if not data then
            return
        end
        
        -- Handle event logic here
    end)
end

return EventHandlers:New()
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
