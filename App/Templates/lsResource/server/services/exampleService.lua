local Result = require "server.utils.result"

---@class ExampleService
local ExampleService = {}

function ExampleService:New()
    local instance = setmetatable({}, { __index = ExampleService })
    return instance
end

---@param data any
---@return table
function ExampleService:DoSomething(data)
    if not data then
        return Result.Err("Invalid data provided")
    end
    
    return Result.Ok({ success = true })
end

return ExampleService:New()
