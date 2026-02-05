local Result = require "server.utils.result"

---@class ExampleRepository
local ExampleRepository = {}

function ExampleRepository:New()
    local instance = setmetatable({
        cache = {}
    }, { __index = ExampleRepository })
    return instance
end

---@param id number
---@return table
function ExampleRepository:FindById(id)
    local result = MySQL.query.await([[
        SELECT * FROM example_table WHERE id = ?
    ]], { id })
    
    if not result or #result == 0 then
        return Result.Err("Record not found")
    end
    
    return Result.Ok(result[1])
end

---@param data table
---@return table
function ExampleRepository:Create(data)
    local insertId = MySQL.insert.await([[
        INSERT INTO example_table (name, data) VALUES (?, ?)
    ]], { data.name, json.encode(data.data) })
    
    if not insertId then
        return Result.Err("Failed to create record")
    end
    
    return Result.Ok({ id = insertId })
end

---@param id number
---@param data table
---@return table
function ExampleRepository:Update(id, data)
    local affected = MySQL.update.await([[
        UPDATE example_table SET name = ?, data = ? WHERE id = ?
    ]], { data.name, json.encode(data.data), id })
    
    if affected == 0 then
        return Result.Err("Failed to update record")
    end
    
    return Result.Ok({ affected = affected })
end

---@param id number
---@return table
function ExampleRepository:Delete(id)
    local affected = MySQL.update.await([[
        DELETE FROM example_table WHERE id = ?
    ]], { id })
    
    if affected == 0 then
        return Result.Err("Failed to delete record")
    end
    
    return Result.Ok({ affected = affected })
end

return ExampleRepository:New()
