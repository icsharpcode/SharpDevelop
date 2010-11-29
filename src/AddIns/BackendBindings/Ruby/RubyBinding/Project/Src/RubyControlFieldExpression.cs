// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using IronRuby.Builtins;
using IronRuby.Compiler.Ast;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Represents a member field expression in a Control or Form:
	/// 
	/// @textBox1
	/// @textBox1.Name
	/// self.TextBox.Name
	/// </summary>
	public class RubyControlFieldExpression
	{
		string memberName = String.Empty;
		string fullMemberName = String.Empty;
		string variableName = String.Empty;
		string methodName = String.Empty;
		bool selfReference;
		
		public RubyControlFieldExpression(string memberName, string variableName, string methodName, string fullMemberName)
		{
			this.memberName = memberName;
			this.variableName = variableName;
			this.methodName = methodName;
			this.fullMemberName = fullMemberName;
			selfReference = ContainsSelfReferenceOrIsClassField(fullMemberName);
		}
		
		/// <summary>
		/// From a member expression of the form: self.textBox1.Name this property will return "Name".
		/// </summary>
		public string MemberName {
			get { return memberName; }
		}
				
		/// <summary>
		/// From a member expression of the form: self.textBox1.Name this property will return "self.textBox1.Name".
		/// </summary>
		public string FullMemberName {
			get { return fullMemberName; }
		}
		
		/// <summary>
		/// From a member expression of the form: @textBox1.Name this property will return "textBox1".
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
			RubyControlFieldExpression rhs = obj as RubyControlFieldExpression;
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
		/// Creates a RubyControlField from an attribute access expression:
		/// 
		/// @textBox1.Name =
		/// self.textBox1 = 
		/// self.textBox1.Name = 
		/// </summary>
		public static RubyControlFieldExpression Create(AttributeAccess expression)
		{
			return Create(GetMemberNames(expression));
		}
		
		/// <summary>
		/// Creates a RubyControlField from an attribute access expression:
		/// 
		/// listViewItem1 =
		/// </summary>
		public static RubyControlFieldExpression Create(LocalVariable localVariable)
		{
			return new RubyControlFieldExpression(String.Empty, localVariable.Name, String.Empty, localVariable.Name);
		}
		
		/// <summary>
		/// Creates a RubyControlField from a call expression:
		/// 
		/// @menuItem1.Items.AddRange(...)
		/// 
		/// Note that in Ruby a method call is also of the form:
		/// 
		/// System::Drawing::Color.Red
		/// </summary>
		public static RubyControlFieldExpression Create(MethodCall methodCall)
		{
			string[] allNames = GetMemberNames(methodCall);
			
			// Remove last member since it is the method name.
			int lastItemIndex = allNames.Length - 1;
			string[] memberNames = new string[lastItemIndex];
			Array.Copy(allNames, memberNames, lastItemIndex);
			
			RubyControlFieldExpression field = Create(memberNames);
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
		/// Removes the @ from the variable name.
		/// </summary>
		public static string GetVariableName(string name)
		{
			if (!String.IsNullOrEmpty(name)) {
				if (name[0] == '@') {
					return name.Substring(1);
				}
			}
			return name;
		}
		
		/// <summary>
		/// Gets the fully qualified name (e.g. System.Windows.Forms.Form)
		/// </summary>
		public static string GetQualifiedName(ConstantVariable variable)
		{
			StringBuilder name = new StringBuilder();
			while (variable != null) {
				name.Insert(0, variable.Name);
				variable = variable.Qualifier as ConstantVariable;
				if (variable != null) {
					name.Insert(0, '.');
				}
			}
			return name.ToString();
		}
		
		/// <summary>
		/// Gets the fully qualified name being referenced in the MemberExpression.
		/// </summary>
		public static string GetMemberName(MethodCall methodCall)
		{
			return GetMemberName(GetMemberNames(methodCall));
		}
		
		public static string[] GetMemberNames(AttributeAccess attributeAccess)
		{
			string[] methodNames = GetMemberNames(attributeAccess.Qualifier);
			
			List<string> names = new List<string>();
			names.AddRange(methodNames);
			
			string lastMemberName = attributeAccess.Name.Substring(0, attributeAccess.Name.Length - 1);
			names.Add(lastMemberName);

			return names.ToArray();
		}
		
		public static string[] GetMemberNames(Expression expression)
		{
			if (expression is MethodCall) {
				return GetMemberNames((MethodCall)expression);
			} else if (expression is InstanceVariable) {
				return GetMemberNames((InstanceVariable)expression);
			} else if (expression is SelfReference) {
				return GetMemberNames((SelfReference)expression);
			} else if (expression is LocalVariable) {
				return GetMemberNames((LocalVariable)expression);
			}
			return new string[0];
		}
				
		/// <summary>
		/// Gets the member names that make up the MemberExpression in order.
		/// </summary>
		public static string[] GetMemberNames(MethodCall methodCall)
		{
			if (methodCall != null) { 
				if (methodCall.Target is ConstantVariable) {
					return GetMemberNamesFromConstant(methodCall);
				} else if (IsClrMemberMethodCall(methodCall)) {
					return GetMemberNamesFromClrMemberMethodCall(methodCall);
				}
			}
			return GetMemberNamesFromMethodCall(methodCall);
		}
		
		static string[] GetMemberNamesFromConstant(MethodCall methodCall)
		{
			List<string> names = new List<string>();
			ConstantVariable constantVariable = methodCall.Target as ConstantVariable;
			while (constantVariable != null) {
				names.Insert(0, constantVariable.Name);
				constantVariable = constantVariable.Qualifier as ConstantVariable;
			}
			names.Add(methodCall.MethodName);
			return names.ToArray();
		}
		
		static string[] GetMemberNamesFromMethodCall(MethodCall methodCall)
		{
			List<string> names = new List<string>();
			while (methodCall != null) {
				names.Insert(0, methodCall.MethodName);
		
				SelfReference selfRef = methodCall.Target as SelfReference;
				InstanceVariable instance = methodCall.Target as InstanceVariable;
				LocalVariable localVariable = methodCall.Target as LocalVariable;
				methodCall = methodCall.Target as MethodCall;
				if (methodCall == null) {
					if (selfRef != null) {
						names.Insert(0, "self");
					} else if (instance != null) {
						names.Insert(0, instance.Name);
					} else if (localVariable != null) {
						names.Insert(0, localVariable.Name);
					}
				}
			}
			return names.ToArray();
		}
		
		static bool IsClrMemberMethodCall(MethodCall methodCall)
		{
			MethodCall clrMemberMethod = methodCall.Target as MethodCall;
			if (clrMemberMethod != null) {
				return IsClrMemberMethod(clrMemberMethod);
			}
			return false;
		}
		
		static bool IsClrMemberMethod(MethodCall methodCall)
		{
			return String.Equals(methodCall.MethodName, "clr_member", StringComparison.InvariantCultureIgnoreCase);
		}
		
		static string[] GetMemberNamesFromClrMemberMethodCall(MethodCall methodCall)
		{
			string[] allNames = GetMemberNamesFromMethodCall(methodCall);
			
			List<string> names = new List<string>();

			MethodCall clrMemberMethodCall = methodCall.Target as MethodCall;
			SymbolLiteral literal = clrMemberMethodCall.Arguments.Expressions[1] as SymbolLiteral;
			MutableString mutableString = literal.GetMutableString();
			string methodBeingCalledByClrMemberMethod = mutableString.ConvertToString();

			// Remove two members since these are 'clr_member' and 'call'.
			int lastItemIndex = allNames.Length - 2;
			string[] memberNames = new string[lastItemIndex];
			Array.Copy(allNames, memberNames, lastItemIndex);
			
			names.AddRange(memberNames);
			names.Add(methodBeingCalledByClrMemberMethod);
			
			return names.ToArray();
		}
		
		public static string[] GetMemberNames(InstanceVariable instance)
		{
			return new string[] { instance.Name };
		}
		
		public static string[] GetMemberNames(SelfReference selfRef)
		{
			return new string[] { "self" };
		}
		
		public static string[] GetMemberNames(LocalVariable localVariable)
		{
			return new string[] { localVariable.Name };
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
		/// by the variable name in this RubyControlFieldExpression 
		/// (e.g. button1 in @button1.FlatAppearance.BorderSize).</remarks>
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
		public static object GetMember(object obj, MethodCall methodCall)
		{
			string[] memberNames = GetMemberNames(methodCall);
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
		/// self.@textBox1.Name
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
			} else if (name.StartsWith("@")) {
				return GetVariableName(name);
			}
			return String.Empty;
		}
		
		static RubyControlFieldExpression Create(string[] memberNames)
		{
			string memberName = String.Empty;
			if (memberNames.Length > 1) {
				memberName = memberNames[memberNames.Length - 1];
			}
			string fullMemberName = RubyControlFieldExpression.GetMemberName(memberNames);
			return new RubyControlFieldExpression(memberName, GetVariableNameFromSelfReference(fullMemberName), String.Empty, fullMemberName);
		}
		
		static bool ContainsSelfReference(string name)
		{
			return name.StartsWith("self.", StringComparison.InvariantCultureIgnoreCase);
		}
		
		static bool ContainsSelfReferenceOrIsClassField(string name)
		{
			if (ContainsSelfReference(name)) {
				return true;
			}
			return name.StartsWith("@");
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
