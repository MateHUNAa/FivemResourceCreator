---@class cl_ExampleFeature
local ExampleFeature = {}
ExampleFeature.__index = ExampleFeature


function ExampleFeature:New()
    local self = setmetatable({}, ExampleFeature)
    --
    return self
end


function ExampleFeature:Start()
    print("Client ExampleFeature started")
    -- Register keybinds, NUI, events
end


function ExampleFeature:Stop()
    print("cl_ExampleFeature stopped")
    -- CleanUP
end

return ExampleFeature
