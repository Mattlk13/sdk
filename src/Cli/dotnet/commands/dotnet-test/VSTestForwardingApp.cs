﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.Cli.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.DotNet.Cli
{
    public class VSTestForwardingApp : ForwardingApp
    {
        private const string VstestAppName = "vstest.console.dll";

        public VSTestForwardingApp(IEnumerable<string> argsToForward)
            : base(GetVSTestExePath(), argsToForward)
        {
            (bool hasRootVariable, string rootVariableName, string rootValue) = GetRootVariable();
            if (!hasRootVariable) {
                WithEnvironmentVariable(rootVariableName, rootValue);
            }
        }

        private static string GetVSTestExePath()
        {
            // Provide custom path to vstest.console.dll or exe to be able to test it against any version of 
            // vstest.console. This is useful especially for our integration tests.
            // This is equivalent to specifying -p:VSTestConsolePath when using dotnet test with csproj.
            string vsTestConsolePath = Environment.GetEnvironmentVariable("VSTEST_CONSOLE_PATH");
            if (!string.IsNullOrWhiteSpace(vsTestConsolePath))
            {
                return vsTestConsolePath;
            }

            return Path.Combine(AppContext.BaseDirectory, VstestAppName);
        }

        internal static (bool hasRootVariable, string rootVariableName, string rootValue) GetRootVariable()
        {
            string rootVariableName = Environment.Is64BitProcess ? "DOTNET_ROOT" : "DOTNET_ROOT(x86)";
            bool hasRootVariable = Environment.GetEnvironmentVariable(rootVariableName) != null;
            string rootValue = hasRootVariable ? null : Path.GetDirectoryName(new Muxer().MuxerPath);

            return (hasRootVariable, rootVariableName, rootValue);
        }
    }
}
