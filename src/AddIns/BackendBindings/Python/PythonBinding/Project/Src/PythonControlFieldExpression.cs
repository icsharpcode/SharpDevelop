// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
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
		
		PythonControlFieldExpression(string memberName, string variableName, string methodName, string fullMemberName)
		{
			this.memberName = memberName;
			this.variableName = variableName;
			this.methodName = methodName;
			this.fullMemberName = fullMemberName;
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
		
		public override string ToString()
		{
			return fullMemberName;
		}
		
		public override bool Equals(object obj)
		{
			PythonControlFieldExpression rhs = obj as PythonControlFieldExpression;
			if (rhs != null) {
				return rhs.fullMemberName == fullMemberName;
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
				names.Insert(0, expression.Name.ToString());
				
				NameExpression nameExpression = expression.Target as NameExpression;
				expression = expression.Target as MemberExpression;
				if (expression == null) {
					if (nameExpression != null) {
						names.Insert(0, nameExpression.Name.ToString());
					}
				}
			}
			return names.ToArray();
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
				return null;
			}
			
			string[] memberNames = fullMemberName.Split('.');
			return GetMember(obj, memberNames, 2, memberNames.Length - 1);
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
			return GetMember(obj, memberNames, 1, memberNames.Length - 2);
		}
		
		/// <summary>
		/// Gets the member that matches the last item in the memberNames array.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="memberNames"></param>
		/// <param name="startIndex">The point at which to start looking in the memberNames.</param>
		/// <param name="endIndex">The last memberNames item to look at.</param>
		static object GetMember(object obj, string[] memberNames, int startIndex, int endIndex)
		{
			Type type = obj.GetType();
			for (int i = startIndex; i <= endIndex; ++i) {
				string name = memberNames[i];
				BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance;
				PropertyInfo property = type.GetProperty(name, propertyBindingFlags);
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
		static string GetVariableNameFromSelfReference(string name)
		{
			int startIndex = name.IndexOf('.');
			if (startIndex > 0) {
				name = name.Substring(startIndex + 1);
				int endIndex = name.IndexOf('.');
				if (endIndex > 0) {
					return GetVariableName(name.Substring(0, endIndex));
				}
				return String.Empty;
			}
			return name;
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
	}
}
