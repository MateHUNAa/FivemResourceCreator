local ExampleEntity = require "server.domain.exampleEntity"

---@class ExampleInstance
local ExampleInstance = {}
ExampleInstance.__index = ExampleInstance

local exampleService = nil

function ExampleInstance:SetServices(service)
    exampleService = service
end

---@param id number
---@return ExampleInstance|nil
function ExampleInstance:New(id)
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
function ExampleInstance:Load()
    if not self.entity then
        local result = exampleService:FindById(self.id)
        if result.ok then
            self.entity = ExampleEntity:New(result.data.id, result.data.name, result.data.data)
        end
    end
    return self.entity
end

---@return table
function ExampleInstance:GetData()
    if not self.entity then
        self:Load()
    end
    return self.entity and self.entity:ToTable() or nil
end

return ExampleInstance
