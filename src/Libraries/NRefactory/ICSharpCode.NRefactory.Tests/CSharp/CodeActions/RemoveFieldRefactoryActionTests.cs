//
// RemoveFieldRefactoryActionTests.cs
//
// Author:
//       Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class RemoveFieldRefactoryActionTests : ContextActionTestBase
	{
		[Test]
		public void RemoveOneField()
		{
			Test<RemoveFieldRefactoryAction>(
				@"
class A {
	int $field;
}
",
				@"
class A {
}
"
				);}

        [Test]
        public void RemoveOneFieldAndAssignmentValue()
        {
            Test<RemoveFieldRefactoryAction>(
                @"
class A {
	int $field;
    A() {
        field = 3;
    }
}
",
                @"
class A {
    A() {
    }
}
"
                );
        }

	    [Test]
        public void RemoveOneFieldAndAssignmentValueAndMethodCall()
        {
            Test<RemoveFieldRefactoryAction>(
                @"
class A {
	int $field;
    A() {
        field = 3;
if(field!=0)
        Method(field);
    }
}
",
                @"
class A {
    A() {
if(TODO!=0)
        Method(TODO);
    }
}
"
                );
        }
	}
}

