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
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Resolves properties, events and fields.
	/// </summary>
	public class PythonMemberResolver : IPythonResolver
	{
		PythonClassResolver classResolver;
		PythonLocalVariableResolver localVariableResolver;
		PythonResolverContext resolverContext;
		PythonSelfResolver selfResolver = new PythonSelfResolver();
		
		public PythonMemberResolver(PythonClassResolver classResolver, PythonLocalVariableResolver localVariableResolver)
		{
			this.classResolver = classResolver;
			this.localVariableResolver = localVariableResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			IMember member = FindMember(resolverContext);
			return CreateResolveResult(member);
		}
		
		public IMember FindMember(PythonResolverContext resolverContext)
		{
			this.resolverContext = resolverContext;
			return FindMember();
		}
		
		IMember FindMember()
		{
			return FindMember(resolverContext.Expression);
		}
		
		IMember FindMember(string expression)
		{
			MemberName memberName = new MemberName(expression);
			if (memberName.HasName) {
				IClass c = FindClass(memberName.Type);
				if (c != null) {
					return FindMemberInClass(c, memberName.Name);
				} else {
					return FindMemberInParent(memberName);
				}
			}
			return null;
		}
		
		IClass FindClass(string className)
		{
			IClass c = FindClassFromClassResolver(className);
			if (c != null) {
				return c;
			}
			if (PythonSelfResolver.IsSelfExpression(className)) {
				return FindClassFromSelfResolver();
			}
			return FindClassFromLocalVariableResolver(className);
		}
		
		IClass FindClassFromClassResolver(string className)
		{
			return classResolver.GetClass(resolverContext, className);
		}
		
		IClass FindClassFromLocalVariableResolver(string localVariableName)
		{
			 MemberName memberName = new MemberName(localVariableName);
			 if (!memberName.HasName) {
			 	string typeName = localVariableResolver.Resolve(localVariableName, resolverContext.FileContent);
		 		return FindClassFromClassResolver(typeName);
			 }
			 return null;
		}
		
		IClass FindClassFromSelfResolver()
		{
			PythonResolverContext newContext = resolverContext.Clone("self");
			ResolveResult result = selfResolver.Resolve(newContext);
			if (result != null) {
				return result.ResolvedType.GetUnderlyingClass();
			}
			return null;
		}
		
		ResolveResult CreateResolveResult(IMember member)
		{
			if (member != null) {
				if (member is IMethod) {
					return new PythonMethodGroupResolveResult(member.DeclaringType, member.Name);
				}
				return new MemberResolveResult(null, null, member);
			}
			return null;
		}
		
		IMember FindMemberInClass(IClass matchingClass, string memberName)
		{
			PythonClassMembers classMembers = new PythonClassMembers(matchingClass);
			return classMembers.FindMember(memberName);
		}
		
		IMember FindMemberInParent(MemberName memberName)
		{
			IMember parentMember = FindMember(memberName.Type);
			if (parentMember != null) {
				return FindMemberInParent(parentMember, memberName.Name);
			}
			return null;
		}
		
		IMember FindMemberInParent(IMember parentMember, string memberName)
		{
			IReturnType returnType = parentMember.ReturnType;
			if (returnType != null) {
				IClass parentMemberClass = returnType.GetUnderlyingClass();
				return FindMemberInClass(parentMemberClass, memberName);
			}
			return null;
		}
	}
}
