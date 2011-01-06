// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.TypeProviders;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of GroupedRow.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupedRowDesigner))]
	public class GroupHeader:BaseRowItem
	{
		public GroupHeader()
		{
			TypeDescriptor.AddProvider(new GroupedRowTypeProvider(), typeof(GroupHeader));
		}
		
		
		[Category("Behavior")]
		public bool PageBreakOnGroupChange {get;set;}
	}
	

}
