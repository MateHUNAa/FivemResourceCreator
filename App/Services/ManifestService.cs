using Spectre.Console;

namespace fvm.Services
{
    public static class ManifestService
    {
        public static void Generate(string resourcePath, string author, string description, List<string> snippets, string frontend, string resourceBase)
        {
            var lines = new List<string>
            {
                "--- @diagnostic disable: undefined-global",
                "",
                "fx_version 'cerulean'",
                "game 'gta5'",
                "use_experimental_fxv2_oal 'yes'",
                "",
                "lua54 'yes'",
                "",
                $"author '{author}'",
                $"description '{description}'",
                ""
            };

            var shared = new List<string>();
            var client = new List<string>();
            var server = new List<string>();
            var files = new List<string>();

            foreach (var snip in snippets)
            {
                switch (snip)
                {
                    case "Logger-system":
                        shared.Add("'@mate-logger/init.lua'");
                        break;
                    case "Grid-system":
                        client.Add("'@mate-grid/init.lua'");
                        break;
                    case "OxMySQL":
                        server.Add("'@oxmysql/lib/MySQL.lua'");
                        break;
                    case "ClientLoader":
                        lines.Add("shared_scripts {'@clientloader/shared.lua'}");
                        break;
                    case "NUI":
                        files.Add("'web/dist/index.html'");
                        files.Add("'web/dist/assets/*.js'");
                        files.Add("'web/dist/assets/*.css'");
                        files.Add("'web/dist/assets/images/{*.png, *.jpg, *.svg, *.webp, *.ico}'");
                        lines.Add("--ui_page 'web/dist/index.html'");
                        lines.Add("ui_page 'http://localhost:5173'");
                        break;
                    case "oxLib":
                        shared.Add("'@ox_lib/init.lua'");
                        break;
                    case "lsCore":
                        shared.Add("'@ls_core/init.lua'");
                        break;
                }
            }

            files.Add("'client/features/**/*.lua'");

            bool isCloader = snippets.Contains("ClientLoader");

            switch (resourceBase)
            {
                case "mCore":
                    if (isCloader)
                    {
                        lines.Add("clientloader {'client/init.lua','client/main.lua'}");
                    }
                    else
                    {
                        client.Add("'client/init.lua'");
                        client.Add("'client/main.lua'");
                    }
                    server.Add("'server/init.lua'");
                    server.Add("'server/main.lua'");
                    shared.Add("'shared/**.*'");
                    break;
                case "ESX":
                case "Standalone":
                    if (isCloader)
                    {
                        lines.Add("clientloader {'client/main.lua'}");
                    }
                    else
                    {
                        client.Add("'utils/cl_*.lua'");
                        client.Add("'client/main.lua'");
                    }
                    shared.Add("'shared/constants/*.lua'");
                    shared.Add("'shared/types/*.lua'");
                    shared.Add("'shared/clases/*.lua'");
                    shared.Add("'utils/sh_*.lua'");
                    break;
            }

            AddSection(lines, "shared_scripts", shared);
            AddSection(lines, "client_scripts", client);
            AddSection(lines, "server_scripts", server);
            AddSection(lines, "files", files);

            File.WriteAllLines(Path.Combine(resourcePath, "fxmanifest.lua"), lines);
        }

        private static void AddSection(List<string> lines, string name, List<string> items)
        {
            if (items.Count == 0) return;

            lines.Add($"{name} {{");
            foreach (var item in items)
            {
                lines.Add($"    {item},");
            }
            if (lines.Count > 0) lines[^1] = lines[^1].TrimEnd(',');
            lines.Add("}");
            lines.Add("");
        }
    }
}
