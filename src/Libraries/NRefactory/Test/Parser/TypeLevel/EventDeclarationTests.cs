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
		[Test]
		public void VBNetSimpleEventDeclarationTest()
		{
			EventDeclaration ed = (EventDeclaration)ParseUtilVBNet.ParseTypeMember("event MyEvent(x as Integer)", typeof(EventDeclaration));
			Assert.AreEqual(1, ed.Parameters.Count);
			Assert.AreEqual("MyEvent", ed.Name);
			Assert.AreEqual("System.EventHandler", ed.TypeReference.Type);
			Assert.IsFalse(ed.HasAddRegion);
			Assert.IsFalse(ed.HasRemoveRegion);
		}
		#endregion 
	}
}
