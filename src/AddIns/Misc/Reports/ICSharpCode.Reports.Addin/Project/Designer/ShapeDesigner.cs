// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of ShapeDesigner.
	/// </summary>
	public class ShapeDesigner:ControlDesigner
	{
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		
		public ShapeDesigner()
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
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
			
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
		}
		
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
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
		
		
		private void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
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
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			base.Dispose(disposing);
		}
	}
}
