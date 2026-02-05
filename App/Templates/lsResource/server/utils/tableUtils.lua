---@class TableUtils
local TableUtils = {}

function TableUtils.DeepCopy(orig)
    local orig_type = type(orig)
    local copy
    if orig_type == 'table' then
        copy = {}
        for orig_key, orig_value in next, orig, nil do
            copy[TableUtils.DeepCopy(orig_key)] = TableUtils.DeepCopy(orig_value)
        end
        setmetatable(copy, TableUtils.DeepCopy(getmetatable(orig)))
    else
        copy = orig
    end
    return copy
end

function TableUtils.Merge(t1, t2)
    for k, v in pairs(t2) do
        if type(v) == "table" and type(t1[k]) == "table" then
            TableUtils.Merge(t1[k], v)
        else
            t1[k] = v
        end
    end
    return t1
end

function TableUtils.Filter(tbl, filterFunc)
    local result = {}
    for k, v in pairs(tbl) do
        if filterFunc(v, k) then
            result[k] = v
        end
    end
    return result
end

function TableUtils.Map(tbl, mapFunc)
    local result = {}
    for k, v in pairs(tbl) do
        result[k] = mapFunc(v, k)
    end
    return result
end

function TableUtils.Find(tbl, predicate)
    for k, v in pairs(tbl) do
        if predicate(v, k) then
            return v, k
        end
    end
    return nil
end

function TableUtils.Contains(tbl, value)
    for _, v in pairs(tbl) do
        if v == value then
            return true
        end
    end
    return false
end

function TableUtils.Count(tbl)
    local count = 0
    for _ in pairs(tbl) do
        count = count + 1
    end
    return count
end

return TableUtils
