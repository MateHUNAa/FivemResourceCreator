using fvm.Interfaces;

namespace fvm.Snippets
{
    public class DomainEntitySnippet : ILuaSnippet
    {
        public string Name => "DomainEntity";
        public string TargetFile => "server/domain/customEntity.lua";

        private const string CodeTemplate = @"
---@class CustomEntity
---@field id number
---@field name string
---@field data table
local CustomEntity = {}
CustomEntity.__index = CustomEntity

---@param id number
---@param name string
---@param data table
---@return CustomEntity
function CustomEntity:New(id, name, data)
    return setmetatable({
        id = id,
        name = name,
        data = data or {}
    }, self)
end

---@return string
function CustomEntity:GetName()
    return self.name
end

---@param name string
function CustomEntity:SetName(name)
    self.name = name
end

---@return table
function CustomEntity:ToTable()
    return {
        id = self.id,
        name = self.name,
        data = self.data
    }
end

return CustomEntity
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
