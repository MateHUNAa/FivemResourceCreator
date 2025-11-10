mCore      = exports["mCore"]:getSharedObj()
inv        = exports["ox_inventory"]
Logger     = require("shared.Logger")
Loc		   = {}
lang       = Loc[Config.lan]
local data = LoadResourceFile(GetCurrentResourceName(), "config.lua")
Config = assert(load(data))()?.Config or {}
