// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;

using MbUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class IndexerDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpIndexerDeclarationTest()
		{
			IndexerDeclaration id = (IndexerDeclaration)ParseUtilCSharp.ParseTypeMember("int this[int a, string b] { get { } set { } }", typeof(IndexerDeclaration));
			Assert.AreEqual(2, id.Parameters.Count);
			Assert.IsTrue(id.HasGetRegion, "No get region found!");
			Assert.IsTrue(id.HasSetRegion, "No set region found!");
		}
		#endregion
		
		#region VB.NET
		// no vb.net representation (indexers are properties named "item" in vb.net)		
		#endregion 
	}
}
