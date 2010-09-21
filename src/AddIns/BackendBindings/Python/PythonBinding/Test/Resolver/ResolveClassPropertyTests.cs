// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ScriptingUtils = ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveClassPropertyTests
	{
		ResolveResult result;
		ParseInformation parseInfo;
		IProperty myClassProperty;
		MockClass myClass;
		
		void ResolveClassProperty()
		{
			PythonResolverContext context = new PythonResolverContext(parseInfo);
			ExpressionResult expression = new ExpressionResult("MyClass.MyProperty");
			
			PythonResolver resolver = new PythonResolver();
			result = resolver.Resolve(context, expression);
		}
		
		void CreateParseInfoWithOneClass()
		{
			ScriptingUtils.MockProjectContent projectContent = new ScriptingUtils.MockProjectContent();
			myClass = new MockClass(projectContent, "MyClass");
			
			projectContent.ClassToReturnFromGetClass = myClass;
			projectContent.ClassNameForGetClass = "MyClass";
			
			myClassProperty = CreateProperty(myClass);
			myClass.Properties.Add(myClassProperty);
			
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			parseInfo = new ParseInformation(unit);
		}
		
		IProperty CreateProperty(IClass c)
		{
			return new DefaultProperty(c, "MyProperty");
		}
		
		[Test]
		public void Resolve_ClassHasOneProperty_ReturnsMemberResolveResult()
		{
			CreateParseInfoWithOneClass();
			ResolveClassProperty();
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			
			Assert.IsNotNull(memberResolveResult);
		}
		
		[Test]
		public void Resolve_ClassHasOneProperty_MemberResolveResultResolvedTypeIsMyClassProperty()
		{
			CreateParseInfoWithOneClass();
			ResolveClassProperty();
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			IMember resolvedMember = memberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
		
		[Test]
		public void Resolve_ClassHasTwoProperties_MemberResolveResultResolvedTypeIsSecondClassProperty()
		{
			CreateParseInfoWithOneClass();
			DefaultProperty extraProperty = new DefaultProperty(myClass, "ExtraProperty");
			myClass.Properties.Insert(0, extraProperty);
			ResolveClassProperty();
			
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			IMember resolvedMember = memberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
	}
}
