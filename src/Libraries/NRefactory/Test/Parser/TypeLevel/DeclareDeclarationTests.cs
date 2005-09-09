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
	public class DeclareDeclarationTests
	{
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetDeclareDeclarationTest()
		{
			string program = "Declare Ansi Function GetUserName Lib \"advapi32.dll\" Alias \"GetUserNameA\" (ByVal lpBuffer As String, ByRef nSize As Integer) As Integer\n";
			DeclareDeclaration dd = (DeclareDeclaration)ParseUtilVBNet.ParseTypeMember(program, typeof(DeclareDeclaration));
			Assert.AreEqual("System.Int32", dd.TypeReference.SystemType);
			Assert.AreEqual("GetUserName", dd.Name);
			Assert.AreEqual("\"advapi32.dll\"", dd.Library);
			Assert.AreEqual("\"GetUserNameA\"", dd.Alias);
			Assert.AreEqual(CharsetModifier.ANSI, dd.Charset);
		}
		#endregion
		
	}
}
