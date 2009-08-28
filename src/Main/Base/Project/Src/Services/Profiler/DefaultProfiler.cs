// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Profiling
{
	public class DefaultProfiler : IProfiler
	{
		public bool CanProfile(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo info, string outputPath, Action afterFinishedAction)
		{
			MessageService.ShowError("Profiling not supported! " +
			                         "No appropriate Profiler AddIn was found.");
			afterFinishedAction();
		}
		
		public void Dispose()
		{
			Stop();
		}
		
		public bool IsRunning {
			get {
				return false;
			}
		}
		
		public void Stop()
		{
		}
	}
}
