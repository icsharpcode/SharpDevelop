// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using System;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Profiling
{
	public interface IProfiler : IDisposable
	{
		bool CanProfile(IProject project);
		void Start(ProcessStartInfo info, string outputPath, Action afterFinishedAction);
		void Stop();
		bool IsRunning { get; }
	}
}
