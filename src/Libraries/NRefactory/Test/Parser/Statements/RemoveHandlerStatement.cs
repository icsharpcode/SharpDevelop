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
	public class RemoveHandlerStatementTests
	{
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetRemoveHandlerTest()
		{
			RemoveHandlerStatement removeHandlerStatement = (RemoveHandlerStatement)ParseUtilVBNet.ParseStatment("RemoveHandler MyHandler, AddressOf MyMethod", typeof(RemoveHandlerStatement));
		}
		#endregion
	}
}
