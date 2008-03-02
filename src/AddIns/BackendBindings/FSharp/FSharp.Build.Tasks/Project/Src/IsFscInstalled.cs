// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;

namespace FSharp.Build.Tasks
{
	/// <summary>
	/// Tasks that determines if the F# compiler is installed.
	/// </summary>
	public sealed class IsFscInstalled : Task
	{
		bool installed;
			
		[Output]
		public bool IsInstalled {
			get { return installed; }
			set { installed = value; }
		}
		
		public override bool Execute()
		{
			if (FscToolLocationHelper.GetPathToTool() != null) {
				installed = true;
			}
			return true;
		}
	}
}
