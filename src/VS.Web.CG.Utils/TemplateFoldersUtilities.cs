// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace Microsoft.VisualStudio.Web.CodeGeneration
{
    public static class TemplateFoldersUtilities
    {
        private const string NupkgExtension = ".nupkg";
        public static List<string> GetTemplateFolders(
            string containingProject,
            string applicationBasePath,
            string[] baseFolders,
            IProjectContext projectContext)
        {
            if (containingProject == null)
            {
                throw new ArgumentNullException(nameof(containingProject));
            }

            if (applicationBasePath == null)
            {
                throw new ArgumentNullException(nameof(applicationBasePath));
            }

            if (baseFolders == null)
            {
                throw new ArgumentNullException(nameof(baseFolders));
            }

            if (projectContext == null)
            {
                throw new ArgumentNullException(nameof(projectContext));
            }

            var rootFolders = new List<string>();
            var templateFolders = new List<string>();

            rootFolders.Add(applicationBasePath);

            var dependency = projectContext.GetPackage(containingProject);

            // if (dependency != null)
            // {
            //     string containingProjectPath = "";
            //     if (dependency.Type == DependencyType.Project)
            //     {
            //         containingProjectPath = Path.GetDirectoryName(dependency.Path);
            //     }
            //     else if (dependency.Type == DependencyType.Package)
            //     {
            //         containingProjectPath = dependency.Path;
            //     }
            //     else
            //     {
            //         Debug.Assert(false, Resource.UnexpectedTypeLibraryForTemplates);
            //     }

            //     if (Directory.Exists(containingProjectPath))
            //     {
            //         rootFolders.Add(containingProjectPath);
            //     }
            // }
            // else 
            // {
                var assembly = projectContext.GetAssembly(containingProject);
                if (assembly != null)
                {
                    rootFolders.Add(GetPackagePathFromAssembly(containingProject, assembly.ResolvedPath));
                }
            //}

            foreach (var rootFolder in rootFolders)
            {
                foreach (var baseFolderName in baseFolders)
                {
                    string templatesFolderName = "Templates";
                    var candidateTemplateFolders = Path.Combine(rootFolder, templatesFolderName, baseFolderName);
                    if (Directory.Exists(candidateTemplateFolders))
                    {
                        templateFolders.Add(candidateTemplateFolders);
                    }
                }
            }
            return templateFolders;
        }

        public static string GetPackagePathFromAssembly(string assemblyName, string resolvedPath)
        {
            if (!string.IsNullOrEmpty(resolvedPath) && !string.IsNullOrEmpty(assemblyName))
            {
                string path = Path.GetFullPath(Path.Combine(resolvedPath, @"..\..\..\"));
                if (Directory.Exists(path))
                {
                    //should leave us at the directory name. 
                    string version = new DirectoryInfo(path).Name;
                    if (!string.IsNullOrEmpty(version))
                    {
                        //Check if nupkg exists at this path.
                        string nupkg = string.Concat(assemblyName, ".", version, NupkgExtension);
                        string nupkgPath = Path.Combine(path, nupkg);
                        if (File.Exists(nupkgPath))
                        {
                            return path;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
