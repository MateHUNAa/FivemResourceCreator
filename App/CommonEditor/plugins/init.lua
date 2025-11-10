--- @diagnostic disable

local pluginFolder = "./plugins/"

local pfile = io.popen('ls "'..pluginFolder..'"')
for filename in pfile:lines() do
     if filename:match("%.lua$") and filename ~= "init.lua" then
             dofile(pluginFolder .. filename)
     end
end

pfile:close()
