---@class sv_ExampleFeature
local ExampleFeature = {}
ExampleFeature.__index = ExampleFeature

function ExampleFeature:New()
    local self = setmetatable({}, ExampleFeature)
    return self
end

function ExampleFeature:Start()
    print("Server ExampleFeature started")
    -- Register net events, commands etc
end

function ExampleFeature:Stop()
    print("Server ExampleFeature stopped")
end

return ExampleFeature
