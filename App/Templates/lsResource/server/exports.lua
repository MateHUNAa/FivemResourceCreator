local ServiceProvider = require "server.services.serviceProvider"

exports('GetService', function(serviceName)
    return ServiceProvider:Get(serviceName)
end)
