/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.07.2010
 * Time: 20:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of BaseGroupHeaderDesigner.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupHeaderDesigner))]
	public class GroupHeaderDesigner:ParentControlDesigner
	{
		
		private ISelectionService selectionService;
		
		public GroupHeaderDesigner()
		{
			
		}
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			GetService ();	
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
