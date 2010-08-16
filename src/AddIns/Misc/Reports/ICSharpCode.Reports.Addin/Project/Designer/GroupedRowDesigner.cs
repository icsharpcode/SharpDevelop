/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.08.2010
 * Time: 19:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of GroupedRowDesigner.
	/// </summary>
	public class GroupedRowDesigner:RowItemDesigner
	{
		private ISelectionService selectionService;
		
		public GroupedRowDesigner()
		{
			
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
		}
	}
}
