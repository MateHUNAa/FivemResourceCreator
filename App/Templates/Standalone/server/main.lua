local FeatureRegistry = require "shared.classes.FeatureRegistry"

-- Import Features
local ExampleFeature = require "server.features.ExampleFeature.feature"

-- Create Features
local example = ExampleFeature:New()

-- Register Features
FeatureRegistry:Register(example)


FeatureRegistry:Init()
