/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.03.2014
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using ICSharpCode.Reporting.Addin.TypeProvider;


namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of TextItemDesigner.
	/// </summary>
	class TextItemDesigner:AbstractDesigner
	{
		

		protected override void PostFilterProperties(IDictionary properties){
			TypeProviderHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
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
		
	}
}
