using fvm.Interfaces;

namespace fvm.Snippets
{
    public class InstanceWrapperSnippet : ILuaSnippet
    {
        public string Name => "InstanceWrapper";
        public string TargetFile => "server/instances/customInstance.lua";

        private const string CodeTemplate = @"
local CustomEntity = require ""server.domain.customEntity""

---@class CustomInstance
local CustomInstance = {}
CustomInstance.__index = CustomInstance

local customService = nil

function CustomInstance:SetServices(service)
    customService = service
end

---@param id number
---@return CustomInstance|nil
function CustomInstance:New(id)
    if not id then
        return nil
    end
    
    local instance = setmetatable({
        id = id,
        entity = nil
    }, self)
    
    return instance
end

---@return table
function CustomInstance:Load()
    if not self.entity then
        local result = customService:FindById(self.id)
        if result.ok then
            self.entity = CustomEntity:New(result.data.id, result.data.name, result.data.data)
        end
    end
    return self.entity
end

---@return table
function CustomInstance:GetData()
    if not self.entity then
        self:Load()
    end
    return self.entity and self.entity:ToTable() or nil
end

---@param data table
---@return table
function CustomInstance:Update(data)
    if not self.entity then
        self:Load()
    end
    
    if data.name then
        self.entity:SetName(data.name)
    end
    
    return customService:Update(self.id, self.entity:ToTable())
end

return CustomInstance
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
