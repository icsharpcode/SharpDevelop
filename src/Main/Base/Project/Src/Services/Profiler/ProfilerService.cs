// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Profiling
{
	public static class ProfilerService
	{
		static IProfiler currentProfiler;
		static IProfiler[] profilers;
		
		static ProfilerService()
		{
			profilers = AddInTree.BuildItems<IProfiler>("/SharpDevelop/Services/ProfilerService/Profiler", null, false).ToArray();
		}
		
		public static bool IsProfilerLoaded
		{
			get {
				return currentProfiler != null;
			}
		}
		
		static IProfiler GetCompatibleProfiler()
		{
			IProject project = null;
			if (ProjectService.OpenSolution != null)
				project = ProjectService.OpenSolution.StartupProject;
			foreach (var p in profilers) {
				if (p != null && p.CanProfile(project))
					return p;
			}
			return new DefaultProfiler();
		}
		
		public static IProfiler CurrentProfiler {
			get {
				if (currentProfiler == null) {
					currentProfiler = GetCompatibleProfiler();
				}
				
				return currentProfiler;
			}
		}
		
		public static string GetSessionFileName(IProject project)
		{
			AbstractProject currentProj = ProjectService.CurrentProject as AbstractProject;
			
			if (currentProj == null)
				throw new InvalidOperationException("Can not profile project");
			
			string filePart = @"ProfilingSessions\Session" +
				DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) +
				".sdps";
			
			string path = Path.Combine(currentProj.Directory, filePart);
			
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			return path;
		}
	}
}
