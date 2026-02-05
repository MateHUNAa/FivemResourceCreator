---@class ExampleEntity
---@field id number
---@field name string
---@field data table
local ExampleEntity = {}
ExampleEntity.__index = ExampleEntity

---@param id number
---@param name string
---@param data table
---@return ExampleEntity
function ExampleEntity:New(id, name, data)
    return setmetatable({
        id = id,
        name = name,
        data = data or {}
    }, self)
end

---@return table
function ExampleEntity:ToTable()
    return {
        id = self.id,
        name = self.name,
        data = self.data
    }
end

return ExampleEntity
