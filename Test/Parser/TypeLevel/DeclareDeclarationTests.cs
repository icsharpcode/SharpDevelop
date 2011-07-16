// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
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
			DeclareDeclaration dd = ParseUtilVBNet.ParseTypeMember<DeclareDeclaration>(program);
			Assert.AreEqual("System.Int32", dd.TypeReference.Type);
			Assert.AreEqual("GetUserName", dd.Name);
			Assert.AreEqual("advapi32.dll", dd.Library);
			Assert.AreEqual("GetUserNameA", dd.Alias);
			Assert.AreEqual(CharsetModifier.Ansi, dd.Charset);
		}
		#endregion
		
	}
}
