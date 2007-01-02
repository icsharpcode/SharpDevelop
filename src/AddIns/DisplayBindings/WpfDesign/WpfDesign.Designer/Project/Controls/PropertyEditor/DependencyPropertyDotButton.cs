// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// The "dot" button appearing after dependency properties to specified advanced values like data bindings.
	/// </summary>
	public class DependencyPropertyDotButton : ButtonBase
	{
		/// <summary>
		/// Dependency property for <see cref="Checked"/>.
		/// </summary>
		public static readonly DependencyProperty CheckedProperty
			= DependencyProperty.Register("Checked", typeof(bool), typeof(DependencyPropertyDotButton),
			                              new FrameworkPropertyMetadata(false));
		
		static DependencyPropertyDotButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DependencyPropertyDotButton), new FrameworkPropertyMetadata(typeof(DependencyPropertyDotButton)));
		}
		
		/// <summary>
		/// Creates a new DependencyPropertyDotButton instance.
		/// </summary>
		public DependencyPropertyDotButton()
		{
		}
		
		IPropertyEditorDataProperty property;
		
		bool isIsSetChangedEventHandlerAttached;
		
		/// <summary>
		/// Creates a new DependencyPropertyDotButton instance that binds its Checked property to the
		/// data properties IsSet property.
		/// </summary>
		public DependencyPropertyDotButton(IPropertyEditorDataProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			this.property = property;
			
			this.Loaded += delegate {
				if (!isIsSetChangedEventHandlerAttached) {
					isIsSetChangedEventHandlerAttached = true;
					this.property.IsSetChanged += OnIsSetChanged;
					OnIsSetChanged(null, null);
				}
			};
			this.Unloaded += delegate {
				if (isIsSetChangedEventHandlerAttached) {
					isIsSetChangedEventHandlerAttached = false;
					this.property.IsSetChanged -= OnIsSetChanged;
				}
			};
			OnIsSetChanged(null, null);
		}
		
		/// <summary>
		/// Creates the context menu on-demand.
		/// </summary>
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			ContextMenu = CreateContextMenu();
			base.OnContextMenuOpening(e);
		}
		
		/// <summary>
		/// Gets/Sets if the button looks checked.
		/// </summary>
		public bool Checked {
			get { return (bool)GetValue(CheckedProperty); }
			set { SetValue(CheckedProperty, value); }
		}
		
		void OnIsSetChanged(object sender, EventArgs e)
		{
			this.Checked = property.IsSet;
		}
		
		/// <summary>
		/// Fires the Click event and opens the context menu.
		/// </summary>
		protected override void OnClick()
		{
			base.OnClick();
			ContextMenu = CreateContextMenu();
			ContextMenu.IsOpen = true;
		}
		
		internal ContextMenu CreateContextMenu()
		{
			ContextMenu contextMenu = new ContextMenu();
			if (property.IsSet) {
				contextMenu.Items.Add(CreateMenuItem("_Reset", OnResetClick));
			} else {
				contextMenu.Items.Add(CreateMenuItem("_Copy to local", OnCopyToLocalClick));
			}
			return contextMenu;
		}
		
		MenuItem CreateMenuItem(string title, RoutedEventHandler handler)
		{
			MenuItem item = new MenuItem();
			item.Header = title;
			item.Click += handler;
			return item;
		}
		
		void OnResetClick(object sender, RoutedEventArgs e)
		{
			property.IsSet = false;
		}
		
		void OnCopyToLocalClick(object sender, RoutedEventArgs e)
		{
			property.IsSet = true;
		}
	}
}
