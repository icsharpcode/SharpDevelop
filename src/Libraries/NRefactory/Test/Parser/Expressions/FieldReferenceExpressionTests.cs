// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class FieldReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpSimpleFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("myTargetObject.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is IdentifierExpression);
			Assert.AreEqual("myTargetObject", ((IdentifierExpression)fre.TargetObject).Identifier);
		}
		
		[Test]
		public void CSharpGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("SomeClass<string>.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void CSharpFullNamespaceGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("Namespace.Subnamespace.SomeClass<string>.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("Namespace.Subnamespace.SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void CSharpGlobalFullNamespaceGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("global::Namespace.Subnamespace.SomeClass<string>.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.IsFalse(tr is InnerClassTypeReference);
			Assert.AreEqual("Namespace.Subnamespace.SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
			Assert.IsTrue(tr.IsGlobal);
		}
		
		[Test]
		public void CSharpNestedGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilCSharp.ParseExpression("MyType<string>.InnerClass<int>.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			InnerClassTypeReference ic = (InnerClassTypeReference)((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("InnerClass", ic.Type);
			Assert.AreEqual(1, ic.GenericTypes.Count);
			Assert.AreEqual("System.Int32", ic.GenericTypes[0].SystemType);
			Assert.AreEqual("MyType", ic.BaseType.Type);
			Assert.AreEqual(1, ic.BaseType.GenericTypes.Count);
			Assert.AreEqual("System.String", ic.BaseType.GenericTypes[0].SystemType);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("myTargetObject.myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is IdentifierExpression);
			Assert.AreEqual("myTargetObject", ((IdentifierExpression)fre.TargetObject).Identifier);
		}
		
		
		[Test]
		public void VBNetGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("SomeClass(of string).myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void VBNetFullNamespaceGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("System.Subnamespace.SomeClass(of string).myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("System.Subnamespace.SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void VBNetGlobalFullNamespaceGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("Global.System.Subnamespace.SomeClass(of string).myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			TypeReference tr = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.IsFalse(tr is InnerClassTypeReference);
			Assert.AreEqual("System.Subnamespace.SomeClass", tr.Type);
			Assert.AreEqual(1, tr.GenericTypes.Count);
			Assert.AreEqual("System.String", tr.GenericTypes[0].SystemType);
			Assert.IsTrue(tr.IsGlobal);
		}
		
		[Test]
		public void VBNetNestedGenericFieldReferenceExpressionTest()
		{
			FieldReferenceExpression fre = (FieldReferenceExpression)ParseUtilVBNet.ParseExpression("MyType(of string).InnerClass(of integer).myField", typeof(FieldReferenceExpression));
			Assert.AreEqual("myField", fre.FieldName);
			Assert.IsTrue(fre.TargetObject is TypeReferenceExpression);
			InnerClassTypeReference ic = (InnerClassTypeReference)((TypeReferenceExpression)fre.TargetObject).TypeReference;
			Assert.AreEqual("InnerClass", ic.Type);
			Assert.AreEqual(1, ic.GenericTypes.Count);
			Assert.AreEqual("System.Int32", ic.GenericTypes[0].SystemType);
			Assert.AreEqual("MyType", ic.BaseType.Type);
			Assert.AreEqual(1, ic.BaseType.GenericTypes.Count);
			Assert.AreEqual("System.String", ic.BaseType.GenericTypes[0].SystemType);
		}
		#endregion
	}
}
