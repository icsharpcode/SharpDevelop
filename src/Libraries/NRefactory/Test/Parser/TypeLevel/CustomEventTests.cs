// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class CustomEventTests
	{
		#region C#
			// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetCustomEventsStatementTest()
		{
			string code = @" Public Custom Event TestEvent As EventHandler
        AddHandler(ByVal value As EventHandler)
            Handlers = CType([Delegate].Combine(Handlers, value), _
                EventHandler)
        End AddHandler

        RemoveHandler(ByVal value as EventHandler)
            Handlers = CType([Delegate].Remove(Handlers, value), _
                EventHandler)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            Dim TempHandlers As EventHandler = Handlers

            If TempHandlers IsNot Nothing Then
                TempHandlers(sender, e)
            End If
        End RaiseEvent
    End Event";
			EventDeclaration customEventDecl = (EventDeclaration)ParseUtilVBNet.ParseTypeMember(code, typeof(EventDeclaration));
			Assert.IsNotNull(customEventDecl);
			Assert.AreEqual("TestEvent", customEventDecl.Name);
		}
		#endregion
	}
}
