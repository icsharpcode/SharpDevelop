// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonPropertyResolver : IPythonResolver
	{
		PythonClassResolver classResolver;
		
		public PythonPropertyResolver(PythonClassResolver classResolver)
		{
			this.classResolver = classResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			IProperty property = FindProperty(resolverContext, expressionResult);
			if (property != null) {
				return CreateMemberResolveResult(property);
			}
			return null;
		}
		
		IProperty FindProperty(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			MemberName memberName = new MemberName(expressionResult.Expression);
			IClass matchingClass = FindClass(resolverContext, memberName.Type);
			if (matchingClass != null) {
				return FindProperty(matchingClass, memberName.Name);
			}
			return null;
		}
		
		MemberResolveResult CreateMemberResolveResult(IProperty property)
		{
			return new MemberResolveResult(null, null, property);
		}
		
		IClass FindClass(PythonResolverContext resolverContext, string typeName)
		{
			return classResolver.GetClass(resolverContext, typeName);
		}
		
		IProperty FindProperty(IClass matchingClass, string memberName)
		{
			foreach (IProperty property in matchingClass.Properties) {
				if (property.Name == memberName) {
					return property;
				}
			}
			return null;
		}
	}
}
