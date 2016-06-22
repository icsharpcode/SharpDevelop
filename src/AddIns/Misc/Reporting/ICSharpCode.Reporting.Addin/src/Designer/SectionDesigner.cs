/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.03.2014
 * Time: 19:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Reporting.Addin.DesignableItems;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of SectionDesigner.
	/// </summary>
	class SectionDesigner:ParentControlDesigner
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
		
		
		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);
			var it = (IToolboxService)this.GetService(typeof(IToolboxService));
			it.SetSelectedToolboxItem(null);
		}
		
		
		public override bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner)
		{
			return false;
		}
		
		
		void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
		
		void GetService ()
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
