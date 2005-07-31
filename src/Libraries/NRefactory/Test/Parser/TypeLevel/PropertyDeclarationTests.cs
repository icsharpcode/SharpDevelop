// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class PropertyDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilCSharp.ParseTypeMember("int MyProperty { get {} set {} } ", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		[Test]
		public void CSharpSimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilCSharp.ParseTypeMember("int MyProperty { get {} } ", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(!pd.HasSetRegion);
		}
		
		[Test]
		public void CSharpSimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilCSharp.ParseTypeMember("int MyProperty { set {} } ", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(!pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleGetSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilVBNet.ParseTypeMember("Property MyProperty As Integer \n Get \n End Get \n Set \n End Set\nEnd Property", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		
		[Test]
		public void VBNetSimpleGetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilVBNet.ParseTypeMember("Property MyProperty \nGet\nEnd Get\nEnd Property", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsTrue(pd.HasGetRegion);
			Assert.IsFalse(pd.HasSetRegion);
		}
		
		[Test]
		public void VBNetSimpleSetPropertyDeclarationTest()
		{
			PropertyDeclaration pd = (PropertyDeclaration)ParseUtilVBNet.ParseTypeMember("Property MyProperty \n Set\nEnd Set\nEnd Property ", typeof(PropertyDeclaration));
			Assert.AreEqual("MyProperty", pd.Name);
			Assert.IsFalse(pd.HasGetRegion);
			Assert.IsTrue(pd.HasSetRegion);
		}
		#endregion 
	}
}
