local ServiceProvider = require "server.services.serviceProvider"

ServiceProvider:Register("ExampleService", function()
    return require "server.services.exampleService"
end)

local exampleService = ServiceProvider:Get("ExampleService")

CreateThread(function()
    LS.Logger:Info("Resource initialized")
end)
