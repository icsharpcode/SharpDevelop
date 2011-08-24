// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin.Designer
	
{
	/// <summary>
	/// Description of ShapeDesigner.
	/// </summary>
	public class ImageDesigner:ControlDesigner
	{
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		
		public ImageDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			
			this.componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanging += OnComponentChanging;
				componentChangeService.ComponentChanged += OnComponentChanged;
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
			
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
		}
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		private void OnComponentChanging (object sender,ComponentChangingEventArgs e)
		{
//			System.Console.WriteLine("changing");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnComponentChanged(object sender,ComponentChangedEventArgs e)
		{
//			System.Console.WriteLine("changed");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate(  );
		}
		
		protected override void Dispose(bool disposing)
		{
		
			if (this.componentChangeService != null) {
				componentChangeService.ComponentChanging -= OnComponentChanging;
				componentChangeService.ComponentChanged -= OnComponentChanged;
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			base.Dispose(disposing);
		}
	}
}