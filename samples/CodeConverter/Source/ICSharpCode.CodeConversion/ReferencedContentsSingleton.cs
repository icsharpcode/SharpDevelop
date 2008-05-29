using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using System.IO;
using System.Runtime.InteropServices;

namespace ICSharpCode.CodeConversion
{
    public static class ReferencedContentsSingleton
    {
        public readonly static List<IProjectContent> ReferencedContents;
        static readonly ProjectContentRegistry pcr = new ProjectContentRegistry();

        static ReferencedContentsSingleton()
        {
            ReferencedContents = new List<IProjectContent> {
              pcr.Mscorlib,
              InternalGetRtDirPC("System"),
              // InternalGet35DirPC("System.Core"),
              InternalGetRtDirPC("System.Data"),
              InternalGetRtDirPC("System.Drawing"),
              InternalGetRtDirPC("System.Windows.Forms"),
              InternalGetRtDirPC("System.Xml"),
              // InternalGet35DirPC("System.Xml.Linq"),
              // InternalGet35DirPC("System.Data.Linq"),
              InternalGetRtDirPC("System.Web"),
              // InternalGet35DirPC("System.Net"),
              // InternalGet35DirPC("System.Data.DataSetExtensions"),
              InternalGetRtDirPC("Microsoft.VisualBasic")
              // add more references here
          };
        }

        static IProjectContent InternalGetRtDirPC(string name)
        {
            IProjectContent pc = pcr.GetProjectContentForReference(name,
                Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), name + ".dll"));

            if (null == pc)
                throw new Exception("Assembly " + name + " was not found in Runtime directory.");

            return pc;
        }

        static IProjectContent InternalGet35DirPC(string name)
        {
            string threeFiveDir =
                Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Reference Assemblies\Microsoft\Framework\v3.5");

            IProjectContent pc = pcr.GetProjectContentForReference(name,
                Path.Combine(threeFiveDir, name + ".dll"));

            if (null == pc)
                throw new Exception("Assembly " + name + " was not found 3.5 reference assemblies directory.");

            return pc;
        }
    }
}