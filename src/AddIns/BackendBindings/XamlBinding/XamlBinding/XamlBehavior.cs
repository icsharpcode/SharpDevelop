// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding
{
	public class XamlBehavior : ProjectBehavior
	{
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (".xaml".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
				return ItemType.Page;
			
			return base.GetDefaultItemType(fileName);
		}
	}
	
	public class SilverlightBehavior : ProjectBehavior
	{
		public override bool IsStartable {
			get { return TestPageFileName.Length > 0; }
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			string pagePath = "file:///" + Path.Combine(((CompilableProject)Project).OutputFullPath, TestPageFileName);
			return new  ProcessStartInfo(pagePath);
		}
		
		public string TestPageFileName {
			get { return ((MSBuildBasedProject)Project).GetEvaluatedProperty("TestPageFileName") ?? ""; }
			set { ((MSBuildBasedProject)Project).SetProperty("TestPageFileName", string.IsNullOrEmpty(value) ? null : value); }
		}
	}
}
