/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 18:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using ICSharpCode.Reporting.Addin.DesignableItems;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of AbstractDesigner.
	/// </summary>
	public class AbstractDesigner:ControlDesigner
	{
		
		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component);
			SelectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			SelectionService.SelectionChanged += OnComponentSelected;
			ComponentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			ComponentChangeService.ComponentRename += OnComponentRename;
		}

		
		void OnComponentSelected(object sender, EventArgs e)
		{
			Control.Invalidate();
		}
		
		
		void OnComponentRename(object sender, ComponentRenameEventArgs e)
		{
			if (e.Component == Component) {
				if ((!String.IsNullOrEmpty(e.NewName)) && (e.NewName != Control.Name)) {
					var c = Component as AbstractItem;;
					c.Name = e.NewName;
					Control.Invalidate();
				}
			}
		}
		
		
		protected ISelectionService SelectionService {get; private set;}
		
		protected IComponentChangeService ComponentChangeService {get;private set;}
		
		protected override void Dispose(bool disposing)
		{
			if (ComponentChangeService != null) {
				ComponentChangeService.ComponentRename -= OnComponentRename;
			}
			if (SelectionService != null) {
				SelectionService.SelectionChanged -= OnComponentSelected;
			}
			base.Dispose(disposing);
		}
	}
}
