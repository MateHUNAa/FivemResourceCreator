-- DOC GENERATED WITH ChatGPT <3

---@meta
-- c_functions.lua — Type definitions and implementations for Functions table

-------------------------------------------------
-- Utility Types
-------------------------------------------------
---@diagnostic disable: duplicate-doc-field

---@class vector3
---@field x number
---@field y number
---@field z number

---@class vector4: vector3
---@field w number

---@class makePedData
---@field coords vector4
---@field freeze boolean
---@field collision boolean
---@field scenario string
---@field anim table

Functions = {}

---@param pos vector3|vector4
---@param handleCam? boolean
function Functions.TeleportPlayer(pos, handleCam) end

---@param r number
---@param centerCoord vector3
---@return vector3
function Functions.FindPlayerPointInRadius(r, centerCoord) end

---@param r number
---@param centerCoord vector3
---@return vector3
function Functions.FindPointInRadius(r, centerCoord) end

---@param model string
function Functions.loadModel(model) end

---@param model string
function Functions.unloadModel(model) end

---@param dict string
function Functions.loadAnimDict(dict) end

---@param dict string
function Functions.unloadAnimDict(dict) end

---@param dict string
function Functions.loadPtfxDict(dict) end

---@param dict string
function Functions.unloadPtfxDict(dict) end

---@param data { prop: string, coords: vector4 }
---@param freeze boolean
---@param synced boolean
---@return number
function Functions.makeProp(data, freeze, synced) end

---@param data { coords: vector3, name: string, sprite: number, col: number, scale?: number, disp?: number, category?: number }
---@return number
function Functions.makeBlip(data) end

---@param model string
---@param data makePedData
---@param options? table
function Functions.makePed(model, data, options) end

---@param entity number
function Functions.destoryProp(entity) end

function Functions.loadDrillSound() end
function Functions.unloadDrillSound() end

---@param entity number
function Functions.lookAtMe(entity) end

---@param entity number|vector3
function Functions.lookEnt(entity) end

---@param toggle boolean
function Functions.lockInv(toggle) end

---@param data { time: number, label: string, dead?: boolean, cancel?: boolean, dict?: string, anim?: string, flag?: number, task?: string }
---@return boolean
function Functions.progressBar(data) end

---@param give number|boolean
---@param item string
---@param amount number
function Functions.toggleItem(give, item, amount) end

---@param items string
---@param amount number
---@return boolean
function Functions.HasItem(items, amount) end

---@param t table
---@return fun(): string, any
function Functions.pairsByKeys(t) end

---@param word string
---@return string
function Functions.CapitalizeFirstLetter(word) end

---@param destination vector3|number
---@return boolean
function Functions.MoveTo(destination) end

---@param rotation vector3
---@return vector3
function Functions.RotToDir(rotation) end

---@param model string
---@return table
function Functions.GetVehicleProps(model) end
