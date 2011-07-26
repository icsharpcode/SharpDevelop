// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of GroupedRowDesigner.
	/// </summary>
	public class GroupedRowDesigner:ContainerItemDesigner
	{
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		
		public GroupedRowDesigner()
		{
			
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			GetService();
		}
		
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		
		private void GetService ()
		{
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;

			}
			
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
			
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
		
		private void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Text = e.NewName;
				Control.Invalidate();
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			if (componentChangeService != null) {
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			base.Dispose(disposing);
		}
	}
}
