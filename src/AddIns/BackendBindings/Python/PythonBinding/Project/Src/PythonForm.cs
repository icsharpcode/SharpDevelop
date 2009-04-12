// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a form in the designer. Used to generate
	/// Python code after the form has been changed in the designer.
	/// </summary>
	public class PythonForm
	{
		StringBuilder codeBuilder;
		string indentString = String.Empty;
		int indent;
		IEventBindingService eventBindingService;
		Attribute[] notDesignOnlyFilter = new Attribute[] { DesignOnlyAttribute.No };
		
		/// <summary>
		/// Used so the EventBindingService.GetEventProperty method can be called to get the property descriptor
		/// for an event.
		/// </summary>
		class PythonFormEventBindingService : EventBindingService
		{
			public PythonFormEventBindingService()
				: base(new ServiceContainer())
			{
			}
			
			protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
			{
				return String.Empty;
			}
			
			protected override ICollection GetCompatibleMethods(EventDescriptor e)
			{
				return new ArrayList();
			}
			
			protected override bool ShowCode()
			{
				return false;
			}
			
			protected override bool ShowCode(int lineNumber)
			{
				return false;
			}
			
			protected override bool ShowCode(IComponent component, EventDescriptor e, string methodName)
			{
				return false;
			}
		}
		
		public PythonForm() 
			: this("\t")
		{
		}
		
		public PythonForm(string indentString) 
			: this(indentString, new PythonFormEventBindingService())
		{
		}
		
		PythonForm(string indentString, IEventBindingService eventBindingService)
		{
			this.indentString = indentString;
			this.eventBindingService = eventBindingService;
		}

		/// <summary>
		/// Generates python code for the InitializeComponent method based on the controls added to the form.
		/// </summary>
		public string GenerateInitializeComponentMethod(Form form)
		{
			codeBuilder = new StringBuilder();
			
			AppendIndentedLine("def InitializeComponent(self):");
			IncreaseIndent();
			
			GenerateInitializeComponentMethodBodyInternal(form);
			
			return codeBuilder.ToString();
		}
		
		/// <summary>
		/// Generates the InitializeComponent method body.
		/// </summary>
		public string GenerateInitializeComponentMethodBody(Form form, int initialIndent)
		{
			codeBuilder = new StringBuilder();
			
			indent = initialIndent;
			GenerateInitializeComponentMethodBodyInternal(form);
			
			return codeBuilder.ToString();
		}
		
		/// <summary>
		/// Gets a list of properties that should be serialized for the specified form.
		/// </summary>
		public PropertyDescriptorCollection GetSerializableProperties(object obj)
		{
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj, notDesignOnlyFilter).Sort()) {
				if (property.SerializationVisibility == DesignerSerializationVisibility.Visible) {
					if (property.ShouldSerializeValue(obj)) {
						properties.Add(property);
					}
				} 
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		
		void GenerateInitializeComponentMethodBodyInternal(Form form)
		{
			AppendChildControlCreation(form);
			AppendChildControlSuspendLayout(form.Controls);
			AppendIndentedLine("self.SuspendLayout()");
			AppendForm(form);			
			AppendChildControlResumeLayout(form.Controls);
			AppendIndentedLine("self.ResumeLayout(False)");
			AppendIndentedLine("self.PerformLayout()");
		}
		
		/// <summary>
		/// Generates python code for the form's InitializeComponent method.
		/// </summary>
		void AppendForm(Form form)
		{
			// Add the controls on the form.
			foreach (Control control in form.Controls) {
				AppendControl(control);
			}
			
			// Add form.
			AppendControl(form, false, false);
		}

		void AppendControl(Control control)
		{
			AppendControl(control, true, true);
		}
		
		/// <summary>
		/// Generates python code for the control.
		/// </summary>
		void AppendControl(Control control, bool addControlNameToProperty, bool addChildControlProperties)
		{
			AppendComment(control.Name);

			string propertyOwnerName = GetPropertyOwnerName(control, addControlNameToProperty);
			AppendProperties(propertyOwnerName, control);
			
			foreach (Control childControl in control.Controls) {
				if (IsSitedComponent(childControl)) {
					AppendIndentedLine(GetPropertyName(propertyOwnerName, "Controls") + ".Add(self._" + childControl.Name + ")");
				}
			}

			AppendEventHandlers(propertyOwnerName, control);
			
			MenuStrip menuStrip = control as MenuStrip;
			ComboBox comboBox = control as ComboBox;
			if (menuStrip != null) {
				AppendMenuStripItems(menuStrip);
			} else if (comboBox != null) {
				AppendSystemArray(comboBox.Name, "Items.AddRange", typeof(Object).FullName, comboBox.Items);
			}

			if (addChildControlProperties) {
				AppendChildControlProperties(control.Controls);
			}
		}
		
		/// <summary>
		/// Appends a property to the InitializeComponents method.
		/// </summary>
		void AppendProperty(string propertyOwnerName, object obj, PropertyDescriptor propertyDescriptor)
		{			
			object propertyValue = propertyDescriptor.GetValue(obj);
			if (propertyValue == null) {
				return;
			}
			
			string propertyName = GetPropertyName(propertyOwnerName, propertyDescriptor.Name);
			Control control = propertyValue as Control;
			if (control != null) {
				AppendIndentedLine(propertyName + " = self._" + control.Name);
			} else {
				AppendIndentedLine(propertyName + " = " + PythonPropertyValueAssignment.ToString(propertyValue));
			}
		}
		
		static string GetPropertyName(string propertyOwnerName, string propertyName)
		{
			if (String.IsNullOrEmpty(propertyOwnerName)) {
				return "self." + propertyName;
			}
			return "self._" + propertyOwnerName + "." + propertyName;
		}
		
		/// <summary>
		/// Appends the comment lines before the control has its properties set.
		/// </summary>
		void AppendComment(string controlName)
		{
			AppendIndentedLine("# ");
			AppendIndentedLine("# " + controlName);
			AppendIndentedLine("# ");
		}
		
		/// <summary>
		/// Increases the indent of any append lines.
		/// </summary>
		void IncreaseIndent()
		{
			++indent;
		}
		
		void DecreaseIndent()
		{
			--indent;
		}

		void Append(string text)
		{
			codeBuilder.Append(text);
		}

		void AppendLine()
		{
			Append("\r\n");
		}
		
		void AppendIndentedLine(string text)
		{
			AppendIndented(text + "\r\n");
		}
		
		void AppendIndented(string text)
		{
			for (int i = 0; i < indent; ++i) {
				codeBuilder.Append(indentString);
			}			
			codeBuilder.Append(text);
		}
		
		void AppendChildControlCreation(Control parentControl)
		{
			MenuStrip menuStrip = parentControl as MenuStrip;
			if (menuStrip != null) {
				AppendChildControlCreation(menuStrip.Items);
			} else {
				AppendChildControlCreation(parentControl.Controls);
			}
		}
			
		void AppendChildControlCreation(Control.ControlCollection controls)
		{
			foreach (Control control in controls) {
				if (IsSitedComponent(control)) {
					AppendComponentCreation(control);
					AppendChildControlCreation(control);
				}
			}
		}
		
		void AppendChildControlCreation(ToolStripItemCollection toolStripItems)
		{
			foreach (ToolStripItem item in toolStripItems) {
				if (IsSitedComponent(item)) {
					AppendComponentCreation(item);
				}
				ToolStripMenuItem menuItem = item as ToolStripMenuItem;
				if (menuItem != null) {
					AppendChildControlCreation(menuItem.DropDownItems);
				}
			}
		}
				
		void AppendComponentCreation(Control control)
		{	
			AppendComponentCreation(control.Name, control);
		}
		
		void AppendComponentCreation(ToolStripItem item)
		{
			AppendComponentCreation(item.Name, item);
		}

		void AppendComponentCreation(string name, object obj)
		{	
			AppendIndentedLine("self._" + name + " = " + obj.GetType().FullName + "()");
		}
		
		void AppendChildControlSuspendLayout(Control.ControlCollection controls)
		{
			AppendChildControlLayoutMethodCalls(controls, new string[] {"SuspendLayout()"});
		}
		
		void AppendChildControlResumeLayout(Control.ControlCollection controls)
		{
			AppendChildControlLayoutMethodCalls(controls, new string[] {"ResumeLayout(False)", "PerformLayout()"});
		}
	
		void AppendChildControlLayoutMethodCalls(Control.ControlCollection controls, string[] methods)
		{
			foreach (Control control in controls) {
				if (HasSitedChildComponents(control)) {
					foreach (string method in methods) {
						AppendIndentedLine("self._" + control.Name + "." + method);
					}
					AppendChildControlLayoutMethodCalls(control.Controls, methods);
				}
			}
		}		
		
		/// <summary>
		/// Generates code that wires an event to an event handler.
		/// </summary>
		/// <remarks>
		/// Note that the EventDescriptorCollection.Sort method does not work if the
		/// enumerator is called first. Sorting will only occur if an item is retrieved after calling
		/// Sort or CopyTo is called. The PropertyDescriptorCollection class does not behave
		/// in the same way.</remarks>
		void AppendEventHandlers(string propertyOwnerName, Control control)
		{
			EventDescriptorCollection events = TypeDescriptor.GetEvents(control, notDesignOnlyFilter).Sort();
			if (events.Count > 0) {
				EventDescriptor dummyEventDescriptor = events[0];
			}
			foreach (EventDescriptor eventDescriptor in events) {
				AppendEventHandler(propertyOwnerName, control, eventDescriptor);
			}
		}
		
		void AppendEventHandler(string propertyOwnerName, Control control, EventDescriptor eventDescriptor)
		{
			PropertyDescriptor propertyDescriptor = eventBindingService.GetEventProperty(eventDescriptor);
			if (propertyDescriptor.ShouldSerializeValue(control)) {
				string methodName = (string)propertyDescriptor.GetValue(control);
				AppendIndentedLine(GetPropertyName(propertyOwnerName, eventDescriptor.Name) + " += self." + methodName);
			}
		}
		
		static bool IsSitedComponent(IComponent component)
		{
			return component.Site != null;
		}
		
		bool HasSitedChildComponents(Control control)
		{
			MenuStrip menuStrip = control as MenuStrip;
			if (menuStrip != null) {
				return HasSitedComponents(menuStrip.Items);
			}
			
			if (control.Controls.Count > 0) {
				foreach (Control childControl in control.Controls) {
					if (!IsSitedComponent(childControl)) {
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		bool HasSitedComponents(ToolStripItemCollection items)
		{	
			foreach (ToolStripItem item in items) {
				if (IsSitedComponent(item)) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Adds ToolStripItems that are stored on the MenuStrip.
		/// </summary>
		void AppendMenuStripItems(MenuStrip menuStrip)
		{
			List<ToolStripItem> items = GetSitedToolStripItems(menuStrip.Items);
			AppendMenuStripItemsAddRange(menuStrip.Name, items);
			AppendToolStripItems(items);
		}
		
		void AppendSystemArray(string componentName, string methodName, string typeName, IList components)
		{
			if (components.Count > 0) {
				AppendIndentedLine("self._" + componentName + "." + methodName + "(System.Array[" + typeName + "](");
				IncreaseIndent();
				for (int i = 0; i < components.Count; ++i) {
					if (i == 0) {
						AppendIndented("[");
					} else {
						Append(",");
						AppendLine();
						AppendIndented(String.Empty);
					}
					object component = components[i];
					if (component is IComponent) {
						Append("self._" + ((IComponent)component).Site.Name);
					} else {
						Append(PythonPropertyValueAssignment.ToString(component));
					}
				}
				Append("]))");
				AppendLine();
				DecreaseIndent();
			}
		}
		
		void AppendMenuStripItemsAddRange(string menuStripName, List<ToolStripItem> items)
		{
			AppendSystemArray(menuStripName, "Items.AddRange", typeof(ToolStripItem).FullName, items);
		}
		
		List<ToolStripItem> GetSitedToolStripItems(ToolStripItemCollection items)
		{
			List<ToolStripItem> sitedItems = new List<ToolStripItem>();
			foreach (ToolStripItem item in items) {
				if (IsSitedComponent(item)) {
					sitedItems.Add(item);
				}
			}
			return sitedItems;
		}
		
		void AppendToolStripItems(IList items)
		{
			foreach (ToolStripItem item in items) {
				AppendToolStripItem(item);
			}
		}
		
		void AppendToolStripItem(ToolStripItem item)
		{
			AppendComment(item.Name);		
			AppendProperties(item.Name, item);
			
			ToolStripMenuItem menuItem = item as ToolStripMenuItem;
			if (menuItem != null) {
				List<ToolStripItem> sitedItems = GetSitedToolStripItems(menuItem.DropDownItems);
				AppendToolStripMenuItemDropDownItems(menuItem.Name, sitedItems);
				AppendToolStripItems(sitedItems);
			}
		}
		
		void AppendToolStripMenuItemDropDownItems(string menuItemName, List<ToolStripItem> items)
		{
			AppendSystemArray(menuItemName, "DropDownItems.AddRange", typeof(ToolStripItem).FullName, items);
		}
		
		void AppendProperties(string propertyOwnerName, object obj)
		{
			foreach (PropertyDescriptor property in GetSerializableProperties(obj)) {
				AppendProperty(propertyOwnerName, obj, property);
			}			
		}
		
		string GetPropertyOwnerName(Control control, bool addControlNameToProperty)
		{
			if (addControlNameToProperty) {
				return control.Name;
			}
			return String.Empty;
		}
		
		void AppendChildControlProperties(Control.ControlCollection controls)
		{
			foreach (Control childControl in controls) {
				if (IsSitedComponent(childControl)) {
					AppendControl(childControl, true, true);
				}
			}			
		}
	}
}
