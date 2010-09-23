// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Resolves properties, events and fields.
	/// </summary>
	public class PythonMemberResolver : IPythonResolver
	{
		PythonClassResolver classResolver;
		
		public PythonMemberResolver(PythonClassResolver classResolver)
		{
			this.classResolver = classResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			IMember member = FindMember(resolverContext, expressionResult.Expression);
			if (member != null) {
				return CreateMemberResolveResult(member);
			}
			return null;
		}
		
		IMember FindMember(PythonResolverContext resolverContext, string expression)
		{
			MemberName memberName = new MemberName(expression);
			if (memberName.HasName) {
				IClass c = FindClass(resolverContext, memberName.Type);
				if (c != null) {
					return FindMemberInClass(c, memberName.Name);
				} else {
					return FindMember(resolverContext, memberName);
				}
			}
			return null;
		}
		
		IClass FindClass(PythonResolverContext resolverContext, string className)
		{
			 return classResolver.GetClass(resolverContext, className);
		}
		
		MemberResolveResult CreateMemberResolveResult(IMember member)
		{
			return new MemberResolveResult(null, null, member);
		}
		
		IMember FindMemberInClass(IClass matchingClass, string memberName)
		{
			List<IMember> members = GetMembers(matchingClass);
			foreach (IMember member in members) {
				if (member.Name == memberName) {
					return member;
				}
			}
			return null;
		}
		
		List<IMember> GetMembers(IClass c)
		{
			List<IMember> members = new List<IMember>();
			members.AddRange(c.Events);
			members.AddRange(c.Fields);
			members.AddRange(c.Properties);
			return members;
		}
		
		IMember FindMember(PythonResolverContext resolverContext, MemberName memberName)
		{
			IMember parentMember = FindMember(resolverContext, memberName.Type);
			if (parentMember != null) {
				return FindMemberInParent(parentMember, memberName.Name);
			}
			return null;
		}
		
		IMember FindMemberInParent(IMember parentMember, string propertyName)
		{
			IClass parentMemberClass = parentMember.ReturnType.GetUnderlyingClass();
			return FindMemberInClass(parentMemberClass, propertyName);
		}
	}
}
