local Result = require "server.utils.result"

---@class ServiceProvider
---@field private _services table
local ServiceProvider = {}

function ServiceProvider:New()
    local instance = setmetatable({
        _services = {}
    }, { __index = ServiceProvider })
    return instance
end

---@param name string
---@param serviceFactory function
function ServiceProvider:Register(name, serviceFactory)
    if not self._services[name] then
        self._services[name] = {
            factory = serviceFactory,
            instance = nil
        }
    end
end

---@param name string
---@return any
function ServiceProvider:Get(name)
    local service = self._services[name]
    if not service then
        error(("Service '%s' not registered"):format(name))
    end
    
    if not service.instance then
        service.instance = service.factory()
    end
    
    return service.instance
end

---@param name string
function ServiceProvider:Reset(name)
    local service = self._services[name]
    if service then
        service.instance = nil
    end
end

function ServiceProvider:ResetAll()
    for _, service in pairs(self._services) do
        service.instance = nil
    end
end

return ServiceProvider
