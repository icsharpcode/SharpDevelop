// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of ReportViewModel.
	/// </summary>
	internal class ReportViewModel
	{
		public ReportViewModel()
		{
		}
		
		public AssemblyNode Node {get;set;}
		
		
		public string Name
		{
			get {return Node.Name;}
		}
		
		
	}
}
