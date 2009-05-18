// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Profiling
{
	public class DefaultProfiler : IProfiler
	{
		public bool CanProfile(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			return false;
		}
		
		public void Start(System.Diagnostics.ProcessStartInfo info, string outputPath, Action afterFinishedAction)
		{
			throw new NotSupportedException();
		}
		
		public void Dispose()
		{
		}
		
		public bool IsRunning {
			get {
				return false;
			}
		}
		
		public void Stop()
		{
			throw new NotSupportedException();
		}
	}
}
