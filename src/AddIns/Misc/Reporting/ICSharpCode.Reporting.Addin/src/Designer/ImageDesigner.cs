/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.06.2015
 * Time: 10:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of ShapeDesigner.
	/// </summary>
	public class ImageDesigner:ControlDesigner
	{
		ISelectionService selectionService;
		IComponentChangeService componentChangeService;
		
		public ImageDesigner()
		{
		}
		
		public override void Initialize(IComponent component){
		
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			
			this.componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanging += OnComponentChanging;
				componentChangeService.ComponentChanged += OnComponentChanged;
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
			
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
		}
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties){
			TypeProviderHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		void OnComponentChanging (object sender,ComponentChangingEventArgs e){
		
//			System.Console.WriteLine("changing");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		void OnComponentChanged(object sender,ComponentChangedEventArgs e){
		
//			System.Console.WriteLine("changed");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
		}
		
		
		void OnSelectionChanged(object sender, EventArgs e){
		
			Control.Invalidate(  );
		}
		
		protected override void Dispose(bool disposing){
		
		
			if (this.componentChangeService != null) {
				componentChangeService.ComponentChanging -= OnComponentChanging;
				componentChangeService.ComponentChanged -= OnComponentChanged;
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			if (selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			base.Dispose(disposing);
		}
	}
}
