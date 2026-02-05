---@class Result
local Result = {}

---@param data any
---@return table
function Result.Ok(data)
    return { ok = true, data = data }
end

---@param msg string
---@param msgTyp? string
---@return table
function Result.Err(msg, msgTyp)
    return { ok = false, msg = msg, msgTyp = msgTyp or "error" }
end

---@param msg string
---@return table
function Result.Warn(msg)
    return { ok = false, msg = msg, msgTyp = "warning" }
end

---@param msg string
---@return table
function Result.Info(msg)
    return { ok = true, msg = msg, msgTyp = "info" }
end

---@param condition boolean
---@param data any
---@param errMsg string
---@return table
function Result.FromCondition(condition, data, errMsg)
    if condition then
        return Result.Ok(data)
    end
    return Result.Err(errMsg)
end

---@param value any
---@param errMsg string
---@return table
function Result.FromValue(value, errMsg)
    if value then
        return Result.Ok(value)
    end
    return Result.Err(errMsg)
end

return Result
