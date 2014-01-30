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
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a member field expression in a Control or Form:
	/// 
	/// self._textBox1
	/// self._textBox1.Name
	/// </summary>
	public class PythonControlFieldExpression
	{
		string memberName = String.Empty;
		string fullMemberName = String.Empty;
		string variableName = String.Empty;
		string methodName = String.Empty;
		bool selfReference;
		
		public PythonControlFieldExpression(string memberName, string variableName, string methodName, string fullMemberName)
		{
			this.memberName = memberName;
			this.variableName = variableName;
			this.methodName = methodName;
			this.fullMemberName = fullMemberName;
			selfReference = ContainsSelfReference(fullMemberName);
		}
		
		/// <summary>
		/// From a member expression of the form: self._textBox1.Name this property will return "Name".
		/// </summary>
		public string MemberName {
			get { return memberName; }
		}
				
		/// <summary>
		/// From a member expression of the form: self._textBox1.Name this property will return "self._textBox1.Name".
		/// </summary>
		public string FullMemberName {
			get { return fullMemberName; }
		}
		
		/// <summary>
		/// From a member expression of the form: self._textBox1.Name this property will return "textBox1".
		/// </summary>		
		public string VariableName {
			get { return variableName; }
		}
		
		/// <summary>
		/// Returns the method being called by the field reference.
		/// </summary>
		public string MethodName {
			get { return methodName; }
		}
		
		/// <summary>
		/// Returns whether the variable is for a field or not.
		/// </summary>
		public bool IsSelfReference {
			get { return selfReference; }
		}
		
		public override string ToString()
		{
			return "[VariableName: " + variableName + " FullMemberName: " + fullMemberName + "]";
		}
		
		public override bool Equals(object obj)
		{
			PythonControlFieldExpression rhs = obj as PythonControlFieldExpression;
			if (rhs != null) {
				return rhs.fullMemberName == fullMemberName && rhs.variableName == variableName;
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return fullMemberName.GetHashCode();			
		}
		
		/// <summary>
		/// Creates a PythonControlField from a member expression:
		/// 
		/// self._textBox1
		/// self._textBox1.Name
		/// </summary>
		public static PythonControlFieldExpression Create(MemberExpression expression)
		{
			return Create(GetMemberNames(expression));
		}
				
		/// <summary>
		/// Creates a PythonControlField from a call expression:
		/// 
		/// self._menuItem1.Items.AddRange(...)
		/// </summary>
		public static PythonControlFieldExpression Create(CallExpression expression)
		{
			string[] allNames = GetMemberNames(expression.Target as MemberExpression);
			
			// Remove last member since it is the method name.
			int lastItemIndex = allNames.Length - 1;
			string[] memberNames = new string[lastItemIndex];
			Array.Copy(allNames, memberNames, lastItemIndex);
			
			PythonControlFieldExpression field = Create(memberNames);
			field.methodName = allNames[lastItemIndex];
			return field;
		}
		
		/// <summary>
		/// From a name such as "System.Windows.Forms.Cursors.AppStarting" this method returns:
		/// "System.Windows.Forms.Cursors"
		/// </summary>
		public static string GetPrefix(string name)
		{
			int index = name.LastIndexOf('.');
			if (index > 0) {
				return name.Substring(0, index);
			}
			return name;
		}

		/// <summary>
		/// Removes the underscore from the variable name.
		/// </summary>
		public static string GetVariableName(string name)
		{
			if (!String.IsNullOrEmpty(name)) {
				if (name.Length > 0) {
					if (name[0] == '_') {
						return name.Substring(1);
					}
				}
			}
			return name;
		}
		
		/// <summary>
		/// Gets the fully qualified name being referenced in the MemberExpression.
		/// </summary>
		public static string GetMemberName(MemberExpression expression)
		{
			return GetMemberName(GetMemberNames(expression));
		}
				
		/// <summary>
		/// Gets the member names that make up the MemberExpression in order.
		/// </summary>
		public static string[] GetMemberNames(MemberExpression expression)
		{
			List<string> names = new List<string>();
			while (expression != null) {
				names.Insert(0, expression.Name);
				
				NameExpression nameExpression = expression.Target as NameExpression;
				expression = expression.Target as MemberExpression;
				if (expression == null) {
					if (nameExpression != null) {
						names.Insert(0, nameExpression.Name);
					}
				}
			}
			return names.ToArray();
		}
		
		/// <summary>
		/// Returns true if the variable has a property with the specified name.
		/// </summary>
		public bool HasPropertyValue(IComponentCreator componentCreator, string name)
		{
			object component = GetObject(componentCreator);
			if (component != null) {
				return TypeDescriptor.GetProperties(component).Find(name, true) != null;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the name of the instance. If the name matches a property of the current component being created
		/// then this method returns null.
		/// </summary>
		public string GetInstanceName(IComponentCreator componentCreator)
		{
			if (IsSelfReference) {
				if (!HasPropertyValue(componentCreator, memberName)) {
					return variableName;
				}
			}
			return null;
		}

		/// <summary>
		/// Looks for a field in the component with the given name.
		/// </summary>
		public static object GetInheritedObject(string name, object component)
		{
			if (component != null) {
				FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (FieldInfo field in fields) {
					if (String.Equals(name, field.Name, StringComparison.InvariantCultureIgnoreCase)) {
						return field.GetValue(component);
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the object that the field expression variable refers to.
		/// </summary>
		/// <remarks>
		/// This method will also check form's base class for any inherited objects that match
		/// the object being referenced.
		/// </remarks>
		public object GetObject(IComponentCreator componentCreator)
		{
			if (variableName.Length > 0) {
				object component = componentCreator.GetComponent(variableName);
				if (component != null) {
					return component;
				}	
				return GetInheritedObject(variableName, componentCreator.RootComponent);
			}
			return componentCreator.RootComponent;		
		}
		
		/// <summary>
		/// Returns the object that the property is defined on. This method may just return the object
		/// passed to it if the property is defined on that object.
		/// </summary>
		/// <remarks>The object parameter must be equivalent to the object referred to
		/// by the variable name in this PythonControlFieldExpression 
		/// (e.g. button1 in self._button1.FlatAppearance.BorderSize).</remarks>
		public object GetObjectForMemberName(object component)
		{
			string[] members = fullMemberName.Split('.');
			int startIndex = GetMembersStartIndex(members);
			
			object currentComponent = component;
			for (int i = startIndex; i < members.Length - 1; ++i) {
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(currentComponent).Find(members[i], true);
				if (propertyDescriptor == null) {
					return null;
				}
				currentComponent = propertyDescriptor.GetValue(currentComponent);
			}
			return currentComponent;
		}
		
		/// <summary>
		/// Sets the property value that is referenced by this field expression.
		/// </summary>
		/// <remarks>
		/// This method checks that the name expression matches a created instance before
		/// converting the name expression as a string and setting the property value.
		/// </remarks>
		public bool SetPropertyValue(IComponentCreator componentCreator, NameExpression nameExpression)
		{
			object component = GetComponent(componentCreator);
			PropertyDescriptor property = GetProperty(component, memberName);
			if (property != null) {
				string name = nameExpression.Name;
				if (property.PropertyType != typeof(bool)) {
					if ("self" == name) {
						return SetPropertyValue(component, memberName, componentCreator.RootComponent);
					} else {
						object instance = componentCreator.GetInstance(name);
						if (instance != null) {
							return SetPropertyValue(component, memberName, instance);
						}
					}
				}
				return SetPropertyValue(component, memberName, name);
			}
			return false;
		}
		
		/// <summary>
		/// Sets the property value that is referenced by this field expression.
		/// </summary>
		/// <remarks>
		/// Checks the field expression to see if it references an class instance variable (e.g. self._treeView1) 
		/// or a variable that is local to the InitializeComponent method (e.g. treeNode1.BackColor)
		/// </remarks>
		public bool SetPropertyValue(IComponentCreator componentCreator, object propertyValue)
		{
			object component = GetComponent(componentCreator);
			return SetPropertyValue(component, memberName, propertyValue);
		}
		
		/// <summary>
		/// Converts the value to the property's type if required.
		/// </summary>
		public static object ConvertPropertyValue(PropertyDescriptor propertyDescriptor, object propertyValue)
		{
			if (propertyValue != null) {
				Type propertyValueType = propertyValue.GetType();
				if (!propertyDescriptor.PropertyType.IsAssignableFrom(propertyValueType)) {
					if (propertyDescriptor.Converter.CanConvertFrom(propertyValueType)) {
						return propertyDescriptor.Converter.ConvertFrom(propertyValue);
					}
					TypeConverter converter = TypeDescriptor.GetConverter(propertyValue);
					return converter.ConvertTo(propertyValue, propertyDescriptor.PropertyType);
				}
			}
			return propertyValue;
		}		
						
		/// <summary>
		/// Gets the member object that matches the field member.
		/// 
		/// For a field: 
		/// 
		/// self._menuStrip.Items.AddRange() 
		/// 
		/// This method returns:
		/// 
		/// Items
		/// </summary>
		public object GetMember(IComponentCreator componentCreator)
		{
			object obj = componentCreator.GetComponent(variableName);
			if (obj == null) {
				obj = componentCreator.GetInstance(variableName);
				if (obj == null) {
					obj = GetInheritedObject(memberName, componentCreator.RootComponent);
					if ((obj == null) && !IsSelfReference) {
						obj = componentCreator.GetInstance(fullMemberName);
					}
				}
			}
			
			if (obj != null) {
				string[] memberNames = fullMemberName.Split('.');
				int startIndex = GetMembersStartIndex(memberNames);
				return GetMember(obj, memberNames, startIndex, memberNames.Length - 1);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the member object that matches the field member.
		/// </summary>
		/// <remarks>
		/// The member names array should contain all items including self, for example:
		///  
		/// self
		/// Controls
		/// </remarks>
		public static object GetMember(object obj, CallExpression expression)
		{
			string[] memberNames = GetMemberNames(expression.Target as MemberExpression);
			if (ContainsSelfReference(memberNames)) {
				return GetMember(obj, memberNames, 1, memberNames.Length - 2);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the member that matches the last item in the memberNames array.
		/// </summary>
		/// <param name="startIndex">The point at which to start looking in the memberNames.</param>
		/// <param name="endIndex">The last memberNames item to look at.</param>
		static object GetMember(object obj, string[] memberNames, int startIndex, int endIndex)
		{			
			for (int i = startIndex; i <= endIndex; ++i) {
				Type type = obj.GetType();
				string name = memberNames[i];
				
				// Try class members excluding inherited members first.
				BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
				PropertyInfo property = type.GetProperty(name, propertyBindingFlags);
				if (property == null) {
					// Try inherited members.
					propertyBindingFlags = propertyBindingFlags & ~BindingFlags.DeclaredOnly;
					property = type.GetProperty(name, propertyBindingFlags);
				}
				
				if (property != null) {
					obj = property.GetValue(obj, null);
				} else {
					return null;
				}
			}
			return obj;
		}
		
		static string GetMemberName(string[] names)
		{
			return String.Join(".", names);
		}
		
		/// <summary>
		/// Gets the variable name from an expression of the form:
		/// 
		/// self._textBox1.Name
		/// 
		/// Returns "textBox1"
		/// </summary>
		/// <remarks>
		/// If there is no self part then the variable name is the first part of the name.
		/// </remarks>
		static string GetVariableNameFromSelfReference(string name)
		{
			if (ContainsSelfReference(name)) {
				name = name.Substring(5);
			}

			int endIndex = name.IndexOf('.');
			if (endIndex > 0) {
				return GetVariableName(name.Substring(0, endIndex));
			} else if (name.StartsWith("_")) {
				return GetVariableName(name);
			}
			return String.Empty;
		}
		
		static PythonControlFieldExpression Create(string[] memberNames)
		{
			string memberName = String.Empty;
			if (memberNames.Length > 1) {
				memberName = memberNames[memberNames.Length - 1];
			}
			string fullMemberName = PythonControlFieldExpression.GetMemberName(memberNames);
			return new PythonControlFieldExpression(memberName, GetVariableNameFromSelfReference(fullMemberName), String.Empty, fullMemberName);
		}
		
		static bool ContainsSelfReference(string name)
		{
			return name.StartsWith("self.", StringComparison.InvariantCultureIgnoreCase);
		}
		
		static bool ContainsSelfReference(string[] members)
		{
			if (members.Length > 0) {
				return "self".Equals(members[0], StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}
		
		/// <summary>
		/// Returns the index into the members array where the members actually start.
		/// The "self" and variable name are skipped.
		/// </summary>
		int GetMembersStartIndex(string[] members)
		{
			if (ContainsSelfReference(members)) {
				// Skip self over when searching for member.
				return 2;
			}
			return 1;
		}
		
		/// <summary>
		/// Sets the value of a property on the component.
		/// </summary>
		bool SetPropertyValue(object component, string name, object propertyValue)
		{
			PropertyDescriptor property = GetProperty(component, name);
			if (property != null) {
				if (OverrideNameProperty(component, name)) {
					propertyValue = variableName;
				} else {
					propertyValue = ConvertPropertyValue(property, propertyValue);
				}
				property.SetValue(component, propertyValue);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Override the name property with the instance variable name when the component is a
		/// ToolStripSeparator to support BindingNavigator separators using the same value for the 
		/// name property.
		/// </summary>
		bool OverrideNameProperty(object component, string property)
		{
			if (property == "Name") {
				return component is ToolStripSeparator;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the component that this field refers to.
		/// </summary>
		object GetComponent(IComponentCreator componentCreator)
		{
			object component = null;
			if (IsSelfReference) {
				component = GetObject(componentCreator);
				component = GetObjectForMemberName(component);
			} else {
				component = componentCreator.GetInstance(variableName);
			}
			return component;
		}
		
		static PropertyDescriptor GetProperty(object component, string name)
		{
			return TypeDescriptor.GetProperties(component).Find(name, true);			
		}
	}
}
