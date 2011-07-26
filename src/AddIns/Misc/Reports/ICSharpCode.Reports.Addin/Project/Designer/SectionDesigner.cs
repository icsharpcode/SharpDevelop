// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of SectionDesigner.
	/// </summary>
	public class SectionDesigner:ParentControlDesigner
	{
		BaseSection section;
		ISelectionService selectionService;
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			this.section = (BaseSection)component;
			if (String.IsNullOrEmpty(component.Site.Name)) {
				component.Site.Name = section.Name;
			} else {
				section.Name = component.Site.Name;
			}
			GetService ();
		}
		
		
		public override SelectionRules SelectionRules {
			get { 
			return SelectionRules.BottomSizeable|SelectionRules.TopSizeable;
			}
		}
		
		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			base.OnPaintAdornments(pe);
		}
		
		
		
		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);
			IToolboxService it = (IToolboxService)this.GetService(typeof(IToolboxService));
			it.SetSelectedToolboxItem(null);
		}
		
		
		public override bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner)
		{
			return false;
		}
		
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
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
		
		
		protected override void Dispose(bool disposing)
		{
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			base.Dispose(disposing);
		}
	}
}
