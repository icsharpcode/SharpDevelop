// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of BaseGroupeRow.
	/// </summary>
	public class GroupHeader:BaseRowItem
	{
		public GroupHeader():base()
		{
		}
		
		public bool PageBreakOnGroupChange {get;set;}
	}
}
