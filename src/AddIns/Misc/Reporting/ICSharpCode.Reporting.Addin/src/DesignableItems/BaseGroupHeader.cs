/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.05.2014
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of BaseGroupHeader.
	/// </summary>
	[Designer(typeof(GroupedRowDesigner))]
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
