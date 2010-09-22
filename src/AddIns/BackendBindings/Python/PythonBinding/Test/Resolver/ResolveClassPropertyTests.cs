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
		ScriptingUtils.MockProjectContent projectContent;
		IProperty myClassProperty;
		MockClass myClass;
		IProperty nestedClassProperty;
		
		void ResolvePropertyExpression(string expression)
		{
			PythonResolverContext context = new PythonResolverContext(parseInfo);
			ExpressionResult expressionResult = new ExpressionResult(expression);
			
			PythonResolver resolver = new PythonResolver();
			result = resolver.Resolve(context, expressionResult);
		}
		
		void CreateParseInfoWithOneClassWithOneProperty()
		{
			projectContent = new ScriptingUtils.MockProjectContent();
			myClass = new MockClass(projectContent, "MyClass");
			
			myClassProperty = AddPropertyToClass("MyProperty", myClass);
			
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			parseInfo = new ParseInformation(unit);
			
			projectContent.SetClassToReturnFromGetClass("MyClass", myClass);
		}
		
		IProperty AddPropertyToClass(string propertyName, IClass c)
		{
			IProperty property = CreateProperty(propertyName, c);
			c.Properties.Add(property);
			return property;
		}
		
		IProperty CreateProperty(string propertyName, IClass c)
		{
			return new DefaultProperty(c, propertyName);
		}
		
		[Test]
		public void Resolve_ExpressionIsForPropertyOnClassWithOneProperty_ReturnsMemberResolveResult()
		{
			CreateParseInfoWithOneClassWithOneProperty();
			ResolvePropertyExpression("MyClass.MyProperty");
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			
			Assert.IsNotNull(memberResolveResult);
		}
		
		[Test]
		public void Resolve_ExpressionIsForPropertyOnClassWithOneProperty_MemberResolveResultResolvedTypeIsMyClassProperty()
		{
			CreateParseInfoWithOneClassWithOneProperty();
			ResolvePropertyExpression("MyClass.MyProperty");
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			IMember resolvedMember = memberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionIsForSecondPropertyOnClassWithTwoProperties_MemberResolveResultResolvedTypeIsSecondClassProperty()
		{
			CreateParseInfoWithOneClassWithOneProperty();
			InsertNewPropertyOnClassBeforeExistingProperty();
			ResolvePropertyExpression("MyClass.MyProperty");
			
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			IMember resolvedMember = memberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
		
		void InsertNewPropertyOnClassBeforeExistingProperty()
		{
			IProperty extraProperty = CreateProperty("ExtraProperty", myClass);
			myClass.Properties.Insert(0, extraProperty);
		}
		
		[Test]
		public void Resolve_ExpressionRefersToNestedProperty_MemberResolveResultResolvedTypeIsNestedProperty()
		{
			CreateParseInfoWithOneClassWithOneProperty();
			AddNestedPropertyToExistingProperty();
			ResolvePropertyExpression("MyClass.MyProperty.MyNestedProperty");
			
			MemberResolveResult memberResolveResult = result as MemberResolveResult;
			IMember resolvedMember = memberResolveResult.ResolvedMember;
			
			Assert.AreEqual(nestedClassProperty, resolvedMember);
		}
		
		void AddNestedPropertyToExistingProperty()
		{
			MockClass nestedPropertyClass = new MockClass(projectContent, "MyNestedPropertyClass");
			nestedClassProperty = AddPropertyToClass("MyNestedProperty", nestedPropertyClass);
			myClassProperty.ReturnType = new DefaultReturnType(nestedPropertyClass);
		}
	}
}
