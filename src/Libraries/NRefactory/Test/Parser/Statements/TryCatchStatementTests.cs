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
	public class TryCatchStatementTests
	{
		#region C#
		[Test]
		public void CSharpSimpleTryCatchStatementTest()
		{
			TryCatchStatement tryCatchStatement = (TryCatchStatement)ParseUtilCSharp.ParseStatment("try { } catch { } ", typeof(TryCatchStatement));
			// TODO : Extend test.
		}
		
		[Test]
		public void CSharpSimpleTryCatchStatementTest2()
		{
			TryCatchStatement tryCatchStatement = (TryCatchStatement)ParseUtilCSharp.ParseStatment("try { } catch (Exception e) { } ", typeof(TryCatchStatement));
			// TODO : Extend test.
		}
		
		[Test]
		public void CSharpSimpleTryCatchFinallyStatementTest()
		{
			TryCatchStatement tryCatchStatement = (TryCatchStatement)ParseUtilCSharp.ParseStatment("try { } catch (Exception) { } finally { } ", typeof(TryCatchStatement));
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
