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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Canvas))]
	public class CanvasDrawLineBehavior : BehaviorExtension, IDrawItemBehavior
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (ExtendedItem.ContentProperty == null || Metadata.IsPlacementDisabled(ExtendedItem.ComponentType))
				return;
			ExtendedItem.AddBehavior(typeof(IDrawItemBehavior), this);
		}

		#region IDrawItemBehavior implementation

		public bool CanItemBeDrawn(Type createItemType)
		{
			return createItemType == typeof(Line);
		}
		
		public void StartDrawItem(DesignItem clickedOn, DesignItem createdItem, ChangeGroup changeGroup, IDesignPanel panel, System.Windows.Input.MouseEventArgs e)
		{
			var startPoint = e.GetPosition(clickedOn.View);
			var operation = PlacementOperation.TryStartInsertNewComponents(clickedOn,
			                                                               new DesignItem[] { createdItem },
			                                                               new Rect[] { new Rect(startPoint.X, startPoint.Y, 1, 1) },
			                                                               PlacementType.AddItem);
			if (operation != null) {
				createdItem.Services.Selection.SetSelectedComponents(new DesignItem[] { createdItem });
				operation.Commit();
			}
			
			
			//changeGroup.Commit();
			
			var lineHandler = createdItem.Extensions.OfType<LineHandlerExtension>().First();
			lineHandler.DragListener.ExternalStart();
			
			new DrawLineMouseGesture(lineHandler, clickedOn.View).Start(panel, (MouseButtonEventArgs) e);
		}

		#endregion
	}
	
	sealed class DrawLineMouseGesture : ClickOrDragMouseGesture
	{
		LineHandlerExtension l;
		
		public DrawLineMouseGesture(LineHandlerExtension l, IInputElement relativeTo)
		{
			this.l = l;
			this.positionRelativeTo = relativeTo;
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e);
			l.DragListener.ExternalMouseMove(e);
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			l.DragListener.ExternalStop();
			base.OnMouseUp(sender, e);
		}

		protected override void OnStopped()
		{
			//if (operation != null)
			//{
			//    operation.Abort();
			//    operation = null;
			//}
			//if (changeGroup != null)
			//{
			//    changeGroup.Abort();
			//    changeGroup = null;
			//}
			if (services.Tool.CurrentTool is CreateComponentTool)
			{
				services.Tool.CurrentTool = services.Tool.PointerTool;
			}
			base.OnStopped();
		}
		
	}
}
