/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.04.2014
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of DataItemDesigner.
	/// </summary>
	class DataItemDesigner:TextItemDesigner
	{
		public DataItemDesigner () {
			
		}
		/*
		#region SmartTags
		
		public override DesignerActionListCollection ActionLists {
			get {
				DesignerActionListCollection actions = new DesignerActionListCollection ();
				actions.Add (new TextBasedDesignerActionList(this.Component));
				
				return actions;
			}
		}
		#endregion
		
		
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
		*/
	}
}
