// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;

using Debugger.MetaData;

namespace Debugger.Expressions
{
	public partial class Expression : DebuggerObject
	{
		public Expression AppendIndexer(params int[] indices)
		{
			return new ArrayIndexerExpression(this, indices);
		}
		
		public Expression[] AppendIndexers(ArrayDimensions dimensions)
		{
			List<Expression> elements = new List<Expression>();
			foreach(int[] indices in dimensions.Indices) {
				elements.Add(this.AppendIndexer(indices));
			}
			return elements.ToArray();
		}
		
		public Expression AppendFieldReference(FieldInfo fieldInfo)
		{
			return AppendMemberReference(fieldInfo);
		}
		
		public Expression AppendPropertyReference(PropertyInfo propertyInfo)
		{
			return AppendMemberReference(propertyInfo);
		}
		
		public Expression AppendMemberReference(MemberInfo memberInfo, params Expression[] args)
		{
			return new MemberReferenceExpression(this, memberInfo, args);
		}
		
		public Expression[] AppendObjectMembers(DebugType type, BindingFlags bindingFlags)
		{
			List<Expression> members = new List<Expression>();
			
			foreach(FieldInfo field in type.GetFields(bindingFlags)) {
				members.Add(this.AppendFieldReference(field));
			}
			foreach(PropertyInfo property in type.GetProperties(bindingFlags)) {
				members.Add(this.AppendPropertyReference(property));
			}
			
			members.Sort();
			return members.ToArray();
		}
	}
}
