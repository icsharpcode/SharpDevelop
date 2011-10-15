// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of ReportItemDesigner.
	/// </summary>
	public class TextItemDesigner:ControlDesigner
	{
		
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		private BaseTextItem ctrl;
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			GetService();
			this.ctrl = (BaseTextItem) component;
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
				Control.Invalidate();
			}
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
