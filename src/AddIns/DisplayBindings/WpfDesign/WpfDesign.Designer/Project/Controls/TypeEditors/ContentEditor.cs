// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors
{
	/// <summary>
	/// Description of ContentEditor.
	/// </summary>
	[PropertyEditor(typeof(ContentControl), "Content")]
	[PropertyEditor(typeof(HeaderedContentControl), "Header")]
	[PropertyEditor(typeof(HeaderedItemsControl), "Header")]
	public class ContentEditor : DockPanel
	{
		readonly IPropertyEditorDataProperty property;
		Button createObjectButton = new DropDownButton();
		readonly TextBoxEditor textBoxEditor;
		readonly FallbackEditor fallbackEditor;
		readonly DataPropertyWithCustomOnValueChangedEvent textBoxEditorDataProperty, fallbackEditorDataProperty;
		UIElement activeEditor;
		DataPropertyWithCustomOnValueChangedEvent activeEditorDataProperty;
		
		public ContentEditor(IPropertyEditorDataProperty property)
		{
			this.property = property;
			PropertyEditorBindingHelper.AddValueChangedEventHandler(this, property, OnValueChanged);
			
			createObjectButton.ContextMenuOpening += delegate {
				createObjectButton.ContextMenu = CreateContextMenu();
			};
			createObjectButton.Click += delegate {
				createObjectButton.ContextMenu = CreateContextMenu();
				createObjectButton.ContextMenu.IsOpen = true;
			};
			SetDock(createObjectButton, Dock.Right);
			this.Children.Add(createObjectButton);
			
			textBoxEditor = new TextBoxEditor(textBoxEditorDataProperty = new DataPropertyWithCustomOnValueChangedEvent(property));
			fallbackEditor = new FallbackEditor(fallbackEditorDataProperty = new DataPropertyWithCustomOnValueChangedEvent(property));
			
			OnValueChanged(null, null);
		}
		
		#region CreateObjectButton Context menu
		ContextMenu CreateContextMenu()
		{
			ContextMenu contextMenu = new ContextMenu();
			contextMenu.Items.Add(CreateMenuItem("Set to _null", delegate { property.Value = null; }));
			contextMenu.Items.Add(CreateMenuItem("Create _string", delegate { property.Value = ""; }));
			contextMenu.Items.Add(CreateMenuItem("Create _Canvas", delegate { property.Value = new Canvas(); }));
			contextMenu.Items.Add(CreateMenuItem("Create _Grid", delegate { property.Value = new Grid(); }));
			return contextMenu;
		}
		
		static MenuItem CreateMenuItem(string title, RoutedEventHandler handler)
		{
			MenuItem item = new MenuItem();
			item.Header = title;
			item.Click += handler;
			return item;
		}
		#endregion
		
		void SetActiveEditor(UIElement newEditor, DataPropertyWithCustomOnValueChangedEvent newDataProperty)
		{
			if (activeEditor != newEditor) {
				if (activeEditorDataProperty != null) {
					activeEditorDataProperty.preventSetValue = true;
				}
				this.Children.Remove(activeEditor);
				this.Children.Add(newEditor);
				activeEditor = newEditor;
				newDataProperty.preventSetValue = false;
				activeEditorDataProperty = newDataProperty;
			}
		}
		
		void OnValueChanged(object sender, EventArgs e)
		{
			object val = property.Value;
			if (val is string) {
				SetActiveEditor(textBoxEditor, textBoxEditorDataProperty);
			} else {
				SetActiveEditor(fallbackEditor, fallbackEditorDataProperty);
			}
			// raise ValueChanged after the new editor's Loaded event has fired
			Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
			                       (Action)activeEditorDataProperty.RaiseValueChanged);
		}
		
		sealed class DataPropertyWithCustomOnValueChangedEvent : ProxyPropertyEditorDataProperty
		{
			internal DataPropertyWithCustomOnValueChangedEvent(IPropertyEditorDataProperty property)
				: base(property)
			{
			}
			
			internal bool preventSetValue;
			
			public override object Value {
				get { return base.Value; }
				set {
					if (!preventSetValue) {
						base.Value = value;
					}
				}
			}
			
			// don't forward add/remove calls to the underlying property, but register them here
			public override event EventHandler ValueChanged;
			
			public override System.ComponentModel.TypeConverter TypeConverter {
				get { return System.ComponentModel.TypeDescriptor.GetConverter(typeof(string)); }
			}
			
			internal void RaiseValueChanged()
			{
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}
	}
}
