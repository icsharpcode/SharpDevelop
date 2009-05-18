// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Python.Build.Tasks
{
	public class Resources
	{
		Resources()
		{
		}
		
		/// <summary>
		/// No main file specified when trying to compile an application
		/// </summary>
		public static string NoMainFileSpecified {
			get { return "No main file specified."; }
		}
	}
}
