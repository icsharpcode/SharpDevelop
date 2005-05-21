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
	public class EventDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleEventDeclarationTest()
		{
			EventDeclaration ed = (EventDeclaration)ParseUtilCSharp.ParseTypeMember("event System.EventHandler MyEvent;", typeof(EventDeclaration));
			Assert.AreEqual(1, ed.VariableDeclarators.Count);
			Assert.AreEqual("MyEvent", ((VariableDeclaration)ed.VariableDeclarators[0]).Name);
			Assert.AreEqual("System.EventHandler", ed.TypeReference.Type);
			
			Assert.IsFalse(ed.HasAddRegion);
			Assert.IsFalse(ed.HasRemoveRegion);
		}
		
		[Test]
		public void CSharpAddRemoveEventDeclarationTest()
		{
			EventDeclaration ed = (EventDeclaration)ParseUtilCSharp.ParseTypeMember("event System.EventHandler MyEvent { add { } remove { } }", typeof(EventDeclaration));
			Assert.AreEqual("MyEvent", ed.Name);
			Assert.AreEqual("System.EventHandler", ed.TypeReference.Type);
			
			Assert.IsTrue(ed.HasAddRegion);
			Assert.IsTrue(ed.HasRemoveRegion);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
