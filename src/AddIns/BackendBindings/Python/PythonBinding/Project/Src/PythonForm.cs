// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		public PythonForm() 
			: this("\t")
		{
		}
		
		public PythonForm(string indentString)
		{
			this.indentString = indentString;			
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
			System.Console.WriteLine("GetSerializableProperties");
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			Attribute[] filter = new Attribute[] { DesignOnlyAttribute.No };
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj, filter).Sort()) {
				if (property.SerializationVisibility == DesignerSerializationVisibility.Visible) {
					if (property.ShouldSerializeValue(obj)) {
						properties.Add(property);
						System.Console.WriteLine("Property.Name: " + property.Name);
					}
				} 
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		
		void GenerateInitializeComponentMethodBodyInternal(Form form)
		{			
			// Add method body.
			AppendIndentedLine("self.SuspendLayout()");
					
			AppendForm(form);
			
			AppendIndentedLine("self.ResumeLayout(False)");
			AppendIndentedLine("self.PerformLayout()");
		}
		
		/// <summary>
		/// Generates python code for the form's InitializeComponent method.
		/// </summary>
		/// <remarks>
		/// Note that when the form is loaded in the designer the Name property appears twice:
		/// 
		/// Property.ComponentType: System.Windows.Forms.Design.DesignerExtenders+NameExtenderProvider
		/// Property.SerializationVisibility: Hidden
		/// Property.IsBrowsable: True
		/// 
		/// Property.ComponentType: System.Windows.Forms.Design.ControlDesigner
		/// Property.SerializationVisibility: Visible
		/// Property.IsBrowsable: False
		/// </remarks>
		void AppendForm(Form form)
		{
			AppendComment(form.Name);

			foreach (PropertyDescriptor property in GetSerializableProperties(form)) {
				AppendProperty(form, property);
			}
		}
		
		/// <summary>
		/// Appends a property to the InitializeComponents method.
		/// </summary>
		void AppendProperty(object obj, PropertyDescriptor propertyDescriptor)
		{			
			object propertyValue = propertyDescriptor.GetValue(obj);
			if (propertyValue == null) {
				return;
			}
			
			AppendIndentedLine("self." + propertyDescriptor.Name + " = " + PythonPropertyValueAssignment.ToString(propertyValue));
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
	}
}
