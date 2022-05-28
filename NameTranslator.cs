/*
    Copyright 2020 Katy Coe - http://www.djkaty.com - https://github.com/djkaty

    All rights reserved.
*/

/*
 * This plugin demonstrates where the various load pipeline hooks are called and how to use them
 * 
 * TIP: This plugin should work with any Unity application. Step through it line by line with the debugger!
 * 
 * TIP: The API surface is large and we give only small examples here. Use IntelliSense to discover useful methods and properties
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppInspector;
using Il2CppInspector.Cpp;
using Il2CppInspector.Model;
using Il2CppInspector.PluginAPI.V100;
using Il2CppInspector.Reflection;
using NoisyCowStudios.Bin2Object;

namespace Loader
{
    // Define your plugin class, implementing IPlugin plus interfaces for any hooks you wish to use
    public class Plugin : IPlugin, ILoadPipeline
    {
        // Set the details of the plugin here
        public string Id => "name-translation";
        public string Name => "Name translator";
        public string Author => "Callow";
        public string Version => "1.0";
        public string Description => "Apply nametranslation.txt file to BeeByte obfuscated.";

        // File path option (GUI: file selection dialog)
        // The internal validator will check the pathname is valid
        private PluginOptionFilePath translationPath = new PluginOptionFilePath
        {
            Name = "path-to-name-translation",
            Description = "Path to name translation file",
            Required = true,

            // Set this to ensure the user selects a file that exists
            MustExist = true,

            // Set this to ensure the user selects a file that doesn't exist (usually for saving)
            MustNotExist = false,

            // Set this to ensure the user selects a folder rather than a file
            IsFolder = false
        };

        private PluginOptionText obfuscationPattern = new PluginOptionText
        {
            Name = "obfuscation-pattern",
            Required = true,
            Description = "Obfuscation pattern",
            Value = "[A-Z]{11}"
        };

        public List<IPluginOption> Options => new List<IPluginOption> { translationPath, obfuscationPattern };

        Dictionary<string, string> nameMapping = null;

        string Translate(string name)
        {
            Regex rx = new Regex(obfuscationPattern.Value);

            if (nameMapping.ContainsKey(name))
            {
                return nameMapping[name];
            }

            MatchCollection matches = rx.Matches(name);
            foreach (Match match in matches)
            {
                string matchValue = match.Groups[0].Value;
                if (nameMapping.ContainsKey(matchValue))
                    name = name.Replace(matchValue, nameMapping[matchValue]);
            }

            return name;
        }

        // See: IL2CPP/Il2CppInspector.cs
        public void PostProcessPackage(Il2CppInspector.Il2CppInspector package, PluginPostProcessPackageEventInfo data)
        {
            
            if (File.Exists(translationPath.Value))
            {
                string[] nameTranslations = File.ReadAllLines(translationPath.Value);
                nameMapping = new Dictionary<string, string>();
                foreach (var line in nameTranslations)
                {
                    var split = line.Split("⇨", StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length < 2)
                        continue;

                    nameMapping[split[0]] = split[1];
                }
            }

            for (int i = 0; i < package.StringLiterals.Count(); i++)
            {
                package.StringLiterals[i] = Translate(package.StringLiterals[i]);
            }

            List<int> keys = new List<int>(package.Strings.Keys);
            foreach (var key in keys)
            {
                package.Strings[key] = Translate(package.Strings[key]);
            }

            data.IsDataModified = true;
        }
    }
}
