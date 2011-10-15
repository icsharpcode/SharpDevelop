// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
