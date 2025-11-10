local data = LoadResourceFile(GetCurrentResourceName(), "config.lua")
Config = assert(load(data))()

for k,v in pairs(Config.ServerOnly) do
     if not Config[k] then
         Config[k] = v
     end
end
