/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 23.09.2008
 * Zeit: 20:03
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of TableDesigner.
	/// </summary>
	public class TableDesigner:ParentControlDesigner
	{
		private ISelectionService selectionService;
		
		public TableDesigner():base()
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
		
		/*
		public override bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner)
		{
			return false;
		}
		*/
		
		
		public override bool CanParent(Control control)
		{
			return base.CanParent(control);
		}
		
		protected override Control GetParentForComponent(IComponent component)
		{
			return base.GetParentForComponent(component);
		}
		
		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);
			IToolboxService it = (IToolboxService)this.GetService(typeof(IToolboxService));
			it.SetSelectedToolboxItem(null);
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
		
		
		
		private void SetValues (string propName,object value)
		{
			PropertyDescriptor p = TypeDescriptor.GetProperties(Control)[propName];
			if ( p == null) {
				throw new ArgumentException (propName);
			} else {
				p.SetValue(Control,value);
			}
		}
		
		
		#region Dispose
		
		protected override void Dispose(bool disposing)
		{
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}
