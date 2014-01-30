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
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class PointerTool : ITool
	{
		internal static readonly PointerTool Instance = new PointerTool();
		
		public Cursor Cursor {
			get { return null; }
		}
		
		public void Activate(IDesignPanel designPanel)
		{
			designPanel.MouseDown += OnMouseDown;
		}
		
		public void Deactivate(IDesignPanel designPanel)
		{
			designPanel.MouseDown -= OnMouseDown;
		}
		
		void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			IDesignPanel designPanel = (IDesignPanel)sender;
			DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true);
			if (result.ModelHit != null) {
				IHandlePointerToolMouseDown b = result.ModelHit.GetBehavior<IHandlePointerToolMouseDown>();
				if (b != null) {
					b.HandleSelectionMouseDown(designPanel, e, result);
				}
				if (!e.Handled) {
					if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
						e.Handled = true;
						ISelectionService selectionService = designPanel.Context.Services.Selection;
						bool setSelectionIfNotMoving = false;
						if (selectionService.IsComponentSelected(result.ModelHit)) {
							setSelectionIfNotMoving = true;
							// There might be multiple components selected. We might have
							// to set the selection to only the item clicked on
							// (or deselect the item clicked on if Ctrl is pressed),
							// but we should do so only if the user isn't performing a drag operation.
						} else {
							selectionService.SetSelectedComponents(new DesignItem[] { result.ModelHit }, SelectionTypes.Auto);
						}
						if (selectionService.IsComponentSelected(result.ModelHit)) {
							new DragMoveMouseGesture(result.ModelHit, e.ClickCount == 2, setSelectionIfNotMoving).Start(designPanel, e);
						}
					}
				}
			}
		}
	}
}
