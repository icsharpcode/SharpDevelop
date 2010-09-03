// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
