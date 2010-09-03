// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	[ExtensionFor(typeof(ContentControl))]
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
			_contentControlsNotAllowedToAdd.Add(typeof (ButtonBase));
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

		public virtual bool CanEnterContainer(PlacementOperation operation)
		{
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
