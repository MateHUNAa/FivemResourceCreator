---@class FeatureRegistry
local FeatureRegistry = {
    features = {}
}


function FeatureRegistry:Register(featureClass)
    table.insert(self.features, featureClass)
end

function FeatureRegistry:Init()
    for _, feat in ipairs(self.features) do
        if feat.Start then
            feat:Start()
        end
    end
end


function FeatureRegistry:StopAll()
    for _, feat in ipairs(self.features) do
        if feat.Stop then
            feat:Stop()
        end
    end
end

return FeatureRegistry
