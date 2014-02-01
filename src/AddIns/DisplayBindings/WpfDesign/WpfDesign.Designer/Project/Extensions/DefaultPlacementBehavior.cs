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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Diagnostics;
using ICSharpCode.WpfDesign.XamlDom;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Panel))]
	[ExtensionFor(typeof(Control))]
	[ExtensionFor(typeof(Border))]
	[ExtensionFor(typeof(Viewbox))]
	public class DefaultPlacementBehavior : BehaviorExtension, IPlacementBehavior
	{
		static List<Type> _contentControlsNotAllowedToAdd;

		static DefaultPlacementBehavior()
		{
			_contentControlsNotAllowedToAdd = new List<Type>();
			_contentControlsNotAllowedToAdd.Add(typeof (Frame));
			_contentControlsNotAllowedToAdd.Add(typeof (GroupItem));
			_contentControlsNotAllowedToAdd.Add(typeof (HeaderedContentControl));
			_contentControlsNotAllowedToAdd.Add(typeof (Label));
			_contentControlsNotAllowedToAdd.Add(typeof (ListBoxItem));
			//_contentControlsNotAllowedToAdd.Add(typeof (ButtonBase));
			_contentControlsNotAllowedToAdd.Add(typeof (StatusBarItem));
			_contentControlsNotAllowedToAdd.Add(typeof (ToolTip));
		}

		public static bool CanContentControlAdd(ContentControl control)
		{
			Debug.Assert(control != null);
			return !_contentControlsNotAllowedToAdd.Any(type => type.IsAssignableFrom(control.GetType()));
		}
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (ExtendedItem.ContentProperty == null ||
			    Metadata.IsPlacementDisabled(ExtendedItem.ComponentType))
				return;
			ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
		}

		public virtual bool CanPlace(ICollection<DesignItem> childItems, PlacementType type, PlacementAlignment position)
		{
			return true;
		}

		public virtual void BeginPlacement(PlacementOperation operation)
		{
		}

		public virtual void EndPlacement(PlacementOperation operation)
		{
			InfoTextEnterArea.Stop(ref infoTextEnterArea);			
		}

		public virtual Rect GetPosition(PlacementOperation operation, DesignItem item)
		{
			if (item.View == null)
				return Rect.Empty;
			var p = item.View.TranslatePoint(new Point(), operation.CurrentContainer.View);
			return new Rect(p, item.View.RenderSize);
		}

		public virtual void BeforeSetPosition(PlacementOperation operation)
		{
		}

		public virtual void SetPosition(PlacementInformation info)
		{
			ModelTools.Resize(info.Item, info.Bounds.Width, info.Bounds.Height);
		}

		public virtual bool CanLeaveContainer(PlacementOperation operation)
		{
			return true;
		}

		public virtual void LeaveContainer(PlacementOperation operation)
		{
			if (ExtendedItem.ContentProperty.IsCollection) {
				foreach (var info in operation.PlacedItems) {
					ExtendedItem.ContentProperty.CollectionElements.Remove(info.Item);
				}
			} else {
				ExtendedItem.ContentProperty.Reset();
			}
		}

		private static InfoTextEnterArea infoTextEnterArea;
		
		public virtual bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter)
		{
			var canEnter = internalCanEnterContainer(operation);
			
			if (canEnter && !shouldAlwaysEnter && !Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
			{
				var b = new Rect(0, 0, ((FrameworkElement)this.ExtendedItem.View).ActualWidth, ((FrameworkElement)this.ExtendedItem.View).ActualHeight);
				InfoTextEnterArea.Start(ref infoTextEnterArea, this.Services, this.ExtendedItem.View, b, Translations.Instance.PressAltText);						
				
				return false;
			}
			
			return canEnter;
		}
		
		private bool internalCanEnterContainer(PlacementOperation operation)
		{
			InfoTextEnterArea.Stop(ref infoTextEnterArea);												
			
			if (ExtendedItem.Component is Expander)
			{
				if (!((Expander) ExtendedItem.Component).IsExpanded)
				{
					((Expander) ExtendedItem.Component).IsExpanded = true;
				}
			}

			if (ExtendedItem.Component is UserControl && ExtendedItem.ComponentType != typeof(UserControl))
				return false;
			
			if (ExtendedItem.Component is Decorator)
				return ((Decorator)ExtendedItem.Component).Child == null;
			
			if (ExtendedItem.ContentProperty.IsCollection)
				return CollectionSupport.CanCollectionAdd(ExtendedItem.ContentProperty.ReturnType,
				                                          operation.PlacedItems.Select(p => p.Item.Component));
			if (ExtendedItem.View is ContentControl) {
				if (!CanContentControlAdd((ContentControl) ExtendedItem.View)) {
					return false;
				}
			}
			
			if (!ExtendedItem.ContentProperty.IsSet)
				return true;
			
			object value = ExtendedItem.ContentProperty.ValueOnInstance;
			// don't overwrite non-primitive values like bindings
			return ExtendedItem.ContentProperty.Value == null && (value is string && string.IsNullOrEmpty(value as string));
		}

		public virtual void EnterContainer(PlacementOperation operation)
		{
			if (ExtendedItem.ContentProperty.IsCollection) {
				foreach (var info in operation.PlacedItems) {
					ExtendedItem.ContentProperty.CollectionElements.Add(info.Item);
				}
			} else {
				ExtendedItem.ContentProperty.SetValue(operation.PlacedItems[0].Item);
			}
			if (operation.Type == PlacementType.AddItem) {
				foreach (var info in operation.PlacedItems) {
					SetPosition(info);
				}
			}
		}
	}
}
