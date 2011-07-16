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
	public class AddHandlerStatementTests
	{
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetAddHandlerTest()
		{
			AddHandlerStatement addHandlerStatement = ParseUtilVBNet.ParseStatement<AddHandlerStatement>("AddHandler Obj.Ev_Event, AddressOf EventHandler");
		}
		#endregion
	}
}
