/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 01.02.2008
 * Zeit: 13:07
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin
	
{
	/// <summary>
	/// Description of ShapeDesigner.
	/// </summary>
	public class ImageDesigner:ControlDesigner
	{
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		
		public ImageDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			
			this.componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanging += OnComponentChanging;
				componentChangeService.ComponentChanged += OnComponentChanged;
			}
			
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
		}
		
		private void OnComponentChanging (object sender,ComponentChangingEventArgs e)
		{
//			System.Console.WriteLine("changing");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnComponentChanged(object sender,ComponentChangedEventArgs e)
		{
//			System.Console.WriteLine("changed");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate(  );
		}
		
		protected override void Dispose(bool disposing)
		{
		
			if (this.componentChangeService != null) {
				componentChangeService.ComponentChanging -= OnComponentChanging;
				componentChangeService.ComponentChanged -= OnComponentChanged;
			}
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			base.Dispose(disposing);
		}
	}
}

