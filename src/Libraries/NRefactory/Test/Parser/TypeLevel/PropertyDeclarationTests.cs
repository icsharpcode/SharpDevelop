/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			// TODO
		#endregion 
	}
}
