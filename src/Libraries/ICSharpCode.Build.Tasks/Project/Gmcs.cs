// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using System;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// MSBuild task for Mono's GMCS.
	/// </summary>
	public class Gmcs : MonoCompilerTask
	{
		protected override string ToolName {
			get {
				return "Gmcs.exe";
			}
		}
		
		protected override string GenerateFullPathToTool()
		{
			return String.Concat(Environment.GetEnvironmentVariable("ComSpec"), " /c gmcs");
		}
	}
}
