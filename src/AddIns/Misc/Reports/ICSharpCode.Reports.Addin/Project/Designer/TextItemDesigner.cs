/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 11.11.2007
 * Zeit: 22:49
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportItemDesigner.
	/// </summary>
	public class TextItemDesigner:ControlDesigner
	{
		
		ISelectionService selectionService;
		BaseTextItem ctrl;
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			GetService();
			this.ctrl = (BaseTextItem) component;
		}
		
		
		private void GetService ()
		{
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
		#region SmartTag
		
		public override DesignerActionListCollection ActionLists {
			get {
				DesignerActionListCollection actions = new DesignerActionListCollection ();
				actions.Add (new TextBasedDesignerActionList(this.Component));
				return actions;
			}
		}
		
		#endregion
		
		#region ContextMenu
		
		public override DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbs = new DesignerVerbCollection();
				DesignerVerb v1 = new DesignerVerb ("TextEditor",OnRunTextEditor);
				verbs.Add (v1);
				return verbs;
			}
		}
		
		
		private void OnRunTextEditor (object sender,EventArgs e)
		{
			ITextEditorDialog ed = new TextEditorDialog (ctrl.Text,ctrl.Name);
			if (ed.ShowDialog() == DialogResult.OK) {
				ctrl.Text = ed.TextValue;
				this.SetProperty ("Name",ed.TextValue);
			}
		}
		
		
		private void SetProperty (string prop, object value)
		{
			PropertyDescriptor p = TypeDescriptor.GetProperties(Control)[prop];
			if (p == null) {
				throw new ArgumentException (this.ctrl.Text);
			} else {
				p.SetValue (Control,value);
			}
		}
		
		
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			base.Dispose(disposing);
		}
	}
}
