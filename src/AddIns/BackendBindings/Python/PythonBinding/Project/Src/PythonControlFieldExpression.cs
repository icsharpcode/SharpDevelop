// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		
		PythonControlFieldExpression(string memberName, string fullMemberName)
		{
			this.memberName = memberName;
			this.fullMemberName = fullMemberName;
		}
		
		public string MemberName {
			get { return memberName; }
		}
				
		public string FullMemberName {
			get { return fullMemberName; }
		}
		
		/// <summary>
		/// Creates a PythonControlField from a member expression:
		/// 
		/// self._textBox1
		/// self._textBox1.Name
		/// </summary>
		public static PythonControlFieldExpression Create(MemberExpression expression)
		{
			string memberName = expression.Name.ToString();
			string fullMemberName = PythonControlFieldExpression.GetMemberName(expression);
			return new PythonControlFieldExpression(memberName, fullMemberName);
		}
		
		/// <summary>
		/// Gets the variable name from an expression of the form:
		/// 
		/// self._textBox1.Name
		/// 
		/// Returns "textBox1"
		/// </summary>
		public static string GetVariableNameFromSelfReference(string name)
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
				
		/// <summary>
		/// Gets the variable name of the control being added.
		/// </summary>
		public static string GetControlNameBeingAdded(CallExpression node)
		{
			//if (node.Args.Length > 0) {
				Arg arg = node.Args[0];
				MemberExpression memberExpression = arg.Expression as MemberExpression;
				return GetVariableName(memberExpression.Name.ToString());
			//}
			//return null;
		}
		
		/// <summary>
		/// Removes the underscore from the variable name.
		/// </summary>
		public static string GetVariableName(string name)
		{
			if (!String.IsNullOrEmpty(name)) {
				if (name.Length > 1) {
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
			StringBuilder typeName = new StringBuilder();
			
			while (expression != null) {
				typeName.Insert(0, expression.Name);
				typeName.Insert(0, ".");
				
				NameExpression nameExpression = expression.Target as NameExpression;
				expression = expression.Target as MemberExpression;
				if (expression == null) {
					if (nameExpression != null) {
						typeName.Insert(0, nameExpression.Name);
					}
				}
			}
			
			return typeName.ToString();
		}		
	}
}
