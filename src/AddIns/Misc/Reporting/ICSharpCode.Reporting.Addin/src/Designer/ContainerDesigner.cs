/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.04.2014
 * Time: 18:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.Designer{

	/// <summary>
	/// Description of RectangleDesigner.
	/// </summary>
	public class ContainerDesigner:ParentControlDesigner{
	
		ISelectionService selectionService;
		IComponentChangeService componentChangeService;
		
		
		public override void Initialize(IComponent component){
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			GetService ();	
		}
		
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties){
			TypeProviderHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}

		
		protected override void OnDragDrop(DragEventArgs de){
			base.OnDragDrop(de);
			var toolboxService = (IToolboxService)this.GetService(typeof(IToolboxService));
			toolboxService.SetSelectedToolboxItem(null);
		}
		
		
		void OnSelectionChanged(object sender, EventArgs e){
			Control.Invalidate( );
		}
		
	
		void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
		}
		
		
		void GetService (){
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentRename += OnComponentRename;
				componentChangeService.ComponentAdding += (sender, e) => {
				};
			}
		}
		
		
		#region Dispose
		protected override void Dispose(bool disposing){
			if (selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			
			if (componentChangeService != null) {
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}
