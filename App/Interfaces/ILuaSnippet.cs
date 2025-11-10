using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Interfaces
{
    public interface ILuaSnippet
    {
        string Name { get; }                 // Friendly name of the snippet
        string TargetFile { get; }           // File to inject into (e.g., "client/main.lua")
        string GetCode(Dictionary<string, string> placeholders); // Return Lua code with placeholders replaced
    }
}
