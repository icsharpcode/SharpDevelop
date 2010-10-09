// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class UnaryOperatorTests
	{
		[Test]
		public void TestMethod()
		{
			//var a = ~new X();
			char a = 'a';
			++a;
			a++;
			float b= 1;
			++b;
			b++;
		}
	}
	
	class X
	{
		//public static implicit operator int(X a) { return 0; }
		public static implicit operator LoaderOptimization(X a) { return 0; }
	}
}
