/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.08.2010
 * Time: 18:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of GroupeHeaderDesigner.
	/// </summary>
	public class GroupHeaderDesigner:DataItemDesigner
	{
		
		private ISelectionService selectionService;
		
		public GroupHeaderDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
		}
		
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
		
		private void GetService ()
		{
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
		}
	
	}
}
