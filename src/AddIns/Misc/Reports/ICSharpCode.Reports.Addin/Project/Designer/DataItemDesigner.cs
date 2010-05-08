/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 30.01.2008
 * Zeit: 08:24
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of DataItemDesigner.
	/// </summary>
	public class DataItemDesigner:ControlDesigner
	{
		private ISelectionService selectionService;
		
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			GetService();
		}
		
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
		
		
		private void GetService ()
		{
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
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
