// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	public class BaseRowItem:ReportContainer
	{
		public BaseRowItem()
		{
		}
	}
	
	public class GroupHeader :BaseRowItem
	{
		public GroupHeader() {
			Console.WriteLine("init groupHeader");
		}
	}
}
