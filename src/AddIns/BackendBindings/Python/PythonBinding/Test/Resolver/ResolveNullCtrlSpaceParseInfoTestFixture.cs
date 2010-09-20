// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveNullCtrlSpaceParseInfoTestFixture
	{
		/// <summary>
		/// Tests that the resolver handles the parse info being null
		/// </summary>
		[Test]
		public void ResolveCtrlSpaceDoesNotThrowExceptionWhenNullParseInfoIsNull()
		{
			PythonResolver resolver = new PythonResolver();
			List<ICompletionEntry> results = resolver.CtrlSpace(0, 0, null, "abc", ExpressionContext.Namespace);
			Assert.AreEqual(0, results.Count);
		}
	}
}
