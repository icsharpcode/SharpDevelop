// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Base class for test fixtures that use SD services.
	/// Calls <c>SD.InitializeForUnitTests()</c> in the fixture set up, and <c>SD.TearDownForUnitTests</c> in the fixture tear down.
	/// </summary>
	public class SDTestFixtureBase
	{
		[TestFixtureSetUp]
		public virtual void FixtureSetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[TestFixtureTearDown]
		public virtual void FixtureTearDown()
		{
			SD.TearDownForUnitTests();
		}
	}
}
