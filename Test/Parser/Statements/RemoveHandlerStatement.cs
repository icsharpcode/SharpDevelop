// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
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
			RemoveHandlerStatement removeHandlerStatement = ParseUtilVBNet.ParseStatement<RemoveHandlerStatement>("RemoveHandler MyHandler, AddressOf MyMethod");
		}
		#endregion
	}
}
