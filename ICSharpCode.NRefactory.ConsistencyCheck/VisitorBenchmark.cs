// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	public class VisitorBenchmark
	{
		public static void Run(IEnumerable<CompilationUnit> files)
		{
			files = files.ToList();
			RunTest("DepthFirstAstVisitor", files, cu => cu.AcceptVisitor(new DepthFirst()));
			RunTest("DepthFirstAstVisitor<object>", files, cu => cu.AcceptVisitor(new DepthFirst<object>()));
			RunTest("DepthFirstAstVisitor<int>", files, cu => cu.AcceptVisitor(new DepthFirst<int>()));
			RunTest("DepthFirstAstVisitor<object, object>", files, cu => cu.AcceptVisitor(new DepthFirst<object, object>(), null));
			RunTest("DepthFirstAstVisitor<int, int>", files, cu => cu.AcceptVisitor(new DepthFirst<int, int>(), 0));
			RunTest("ObservableAstVisitor", files, cu => cu.AcceptVisitor(new ObservableAstVisitor()));
			RunTest("ObservableAstVisitor<object, object>", files, cu => cu.AcceptVisitor(new ObservableAstVisitor<object, object>(), null));
			RunTest("ObservableAstVisitor<int, int>", files, cu => cu.AcceptVisitor(new ObservableAstVisitor<int, int>(), 0));
		}
		
		class DepthFirst : DepthFirstAstVisitor {}
		class DepthFirst<T> : DepthFirstAstVisitor<T> {}
		class DepthFirst<T, S> : DepthFirstAstVisitor<T, S> {}
		
		static void RunTest(string text, IEnumerable<CompilationUnit> files, Action<CompilationUnit> action)
		{
			Stopwatch w = Stopwatch.StartNew();
			foreach (var file in files) {
				for (int i = 0; i < 20; i++) {
					action(file);
				}
			}
			w.Stop();
			Console.WriteLine(text.PadRight(40) + ": " + w.Elapsed);
		}
	}
}
