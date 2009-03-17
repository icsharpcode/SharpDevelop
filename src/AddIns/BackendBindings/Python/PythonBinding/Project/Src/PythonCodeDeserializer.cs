// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.Reflection;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Creates objects from python code.
	/// </summary>
	public class PythonCodeDeserializer
	{
		IDesignerHost designerHost;
		const BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance;
		
		public PythonCodeDeserializer(IDesignerHost designerHost)
		{
			this.designerHost = designerHost;
		}
		
		/// <summary>
		/// Creates or gets the object specified in the python AST.
		/// </summary>
		/// <returns>
		/// Null if the node cannot be deserialized.
		/// </returns>
		public object Deserialize(Node node)
		{
			if (node == null) {
				throw new ArgumentNullException("node");
			}
			
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(node as MemberExpression);			
			Type type = designerHost.GetType(PythonControlFieldExpression.GetPrefix(field.FullMemberName));
			if (type != null) {
				PropertyInfo propertyInfo = type.GetProperty(field.MemberName, propertyBindingFlags);
				if (propertyInfo != null) {
					return propertyInfo.GetValue(type, null);
				}
			}
			return null;
		}
	}
}
