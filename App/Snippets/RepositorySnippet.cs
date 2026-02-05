using fvm.Interfaces;

namespace fvm.Snippets
{
    public class RepositorySnippet : ILuaSnippet
    {
        public string Name => "Repository";
        public string TargetFile => "server/data/repository/customRepository.lua";

        private const string CodeTemplate = @"
local Result = require ""server.utils.result""

---@class CustomRepository
local CustomRepository = {}

function CustomRepository:New()
    local instance = setmetatable({
        cache = {}
    }, { __index = CustomRepository })
    return instance
end

---@param id number
---@return table
function CustomRepository:FindById(id)
    local result = MySQL.query.await([[
        SELECT * FROM custom_table WHERE id = ?
    ]], { id })
    
    if not result or #result == 0 then
        return Result.Err(""Record not found"")
    end
    
    return Result.Ok(result[1])
end

---@param data table
---@return table
function CustomRepository:Create(data)
    local insertId = MySQL.insert.await([[
        INSERT INTO custom_table (name) VALUES (?)
    ]], { data.name })
    
    if not insertId then
        return Result.Err(""Failed to create record"")
    end
    
    return Result.Ok({ id = insertId })
end

return CustomRepository:New()
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
