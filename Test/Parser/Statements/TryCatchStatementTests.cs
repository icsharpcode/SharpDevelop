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
	public class TryCatchStatementTests
	{
		#region C#
		[Test]
		public void CSharpSimpleTryCatchStatementTest()
		{
			TryCatchStatement tryCatchStatement = ParseUtilCSharp.ParseStatement<TryCatchStatement>("try { } catch { } ");
			Assert.IsTrue(tryCatchStatement.FinallyBlock.IsNull);
			Assert.AreEqual(1, tryCatchStatement.CatchClauses.Count);
			Assert.IsTrue(tryCatchStatement.CatchClauses[0].TypeReference.IsNull);
			Assert.IsEmpty(tryCatchStatement.CatchClauses[0].VariableName);
		}
		
		[Test]
		public void CSharpSimpleTryCatchStatementTest2()
		{
			TryCatchStatement tryCatchStatement = ParseUtilCSharp.ParseStatement<TryCatchStatement>("try { } catch (Exception e) { } ");
			Assert.IsTrue(tryCatchStatement.FinallyBlock.IsNull);
			Assert.AreEqual(1, tryCatchStatement.CatchClauses.Count);
			Assert.AreEqual("Exception", tryCatchStatement.CatchClauses[0].TypeReference.Type);
			Assert.AreEqual("e", tryCatchStatement.CatchClauses[0].VariableName);
		}
		
		[Test]
		public void CSharpSimpleTryCatchFinallyStatementTest()
		{
			TryCatchStatement tryCatchStatement = ParseUtilCSharp.ParseStatement<TryCatchStatement>("try { } catch (Exception) { } catch { } finally { } ");
			Assert.IsFalse(tryCatchStatement.FinallyBlock.IsNull);
			Assert.AreEqual(2, tryCatchStatement.CatchClauses.Count);
			Assert.AreEqual("Exception", tryCatchStatement.CatchClauses[0].TypeReference.Type);
			Assert.IsEmpty(tryCatchStatement.CatchClauses[0].VariableName);
			Assert.IsTrue(tryCatchStatement.CatchClauses[1].TypeReference.IsNull);
			Assert.IsEmpty(tryCatchStatement.CatchClauses[1].VariableName);
		}
		#endregion
		
		#region VB.NET
		// TODO
		#endregion
	}
}
