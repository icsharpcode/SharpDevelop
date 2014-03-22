/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.03.2014
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Addin.TypeProvider;


namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of TextItemDesigner.
	/// </summary>
	public class TextItemDesigner:ControlDesigner
	{
		
		ISelectionService selectionService;
		IComponentChangeService componentChangeService;
		ICSharpCode.Reporting.Addin.DesignableItems.BaseTextItem ctrl;
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			GetService();
			ctrl = (ICSharpCode.Reporting.Addin.DesignableItems.BaseTextItem) component;
		}
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			TypeProviderHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		
		void GetService ()
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
		
		
		void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
		
		void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
		}

		
		
		#region SmartTag
		
//		public override DesignerActionListCollection ActionLists {
//			get {
//				var actions = new DesignerActionListCollection ();
//				actions.Add (new TextBasedDesignerActionList(this.Component));
//				return actions;
//			}
//		}
		
		#endregion
		
		#region ContextMenu
		/*
		public override DesignerVerbCollection Verbs {
			get {
				var verbs = new DesignerVerbCollection();
				var v1 = new DesignerVerb ("TextEditor",OnRunTextEditor);
				verbs.Add (v1);
				return verbs;
			}
		}
		
		
		private void OnRunTextEditor (object sender,EventArgs e)
		{
			IStringBasedEditorDialog ed = new TextEditorDialog (ctrl.Text,ctrl.Name);
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
		
		*/
		#endregion
		
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
