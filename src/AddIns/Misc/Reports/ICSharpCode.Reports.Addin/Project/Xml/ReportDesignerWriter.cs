// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;


namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportDesignerWriter.
	/// </summary>
	public class ReportDesignerWriter:MycroWriter
	{
		public ReportDesignerWriter()
		{
		}
		
		protected override string GetTypeName(Type t)
		{
			if (t.BaseType != null && t.BaseType.Name.StartsWith("Base",StringComparison.InvariantCultureIgnoreCase)) {
//				return t.BaseType.Name;
			}
			return t.Name;
		}
	}
	
}
