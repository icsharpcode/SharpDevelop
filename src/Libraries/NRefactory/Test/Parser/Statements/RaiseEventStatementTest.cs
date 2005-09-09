// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class RaiseEventStatementTest
	{
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetRaiseEventStatementTest()
		{
			RaiseEventStatement raiseEventStatement = (RaiseEventStatement)ParseUtilVBNet.ParseStatment("RaiseEvent MyEvent(a, 5, (6))", typeof(RaiseEventStatement));
		}
		#endregion
	}
}
