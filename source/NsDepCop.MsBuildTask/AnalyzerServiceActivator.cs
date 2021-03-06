﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Codartis.NsDepCop.Core.Interface;

namespace Codartis.NsDepCop.MsBuildTask
{
    /// <summary>
    /// Activates the out-of-process analyzer service.
    /// </summary>
    public static class AnalyzerServiceActivator
    {
        private const string ServiceHostProcessName = "NsDepCop.ServiceHost";

        public static void Activate()
        {
            var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var workingFolder = Path.GetDirectoryName(codeBase.AbsolutePath);
            if (workingFolder == null)
                throw new Exception($"Unable to determine working folder from assembly codebase: {codeBase.AbsolutePath}");

            var serviceExePath = Path.Combine(workingFolder, ServiceHostProcessName + ".exe");

            CreateServer(workingFolder, serviceExePath, GetProcessId());
        }

        private static string GetProcessId() => Process.GetCurrentProcess().Id.ToString();

        private static void CreateServer(string workingFolderPath, string serviceExePath, string arguments)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = serviceExePath,
                    WorkingDirectory = workingFolderPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = arguments,
                };
                Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[{ProductConstants.ToolName}] AnalyzerServiceActivator.CreateServer failed: {e}");
            }
        }
    }
}
