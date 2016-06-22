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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	public abstract class ConsistencyCheckCommand : SimpleCommand
	{
		public sealed override void Execute(object parameter)
		{
			Execute(parameter as Solution ?? new Solution(ProjectService.OpenSolution));
		}
		
		public abstract void Execute(Solution solution);
		
		protected void RunTestOnAllFiles(Solution solution, Action<CSharpFile> runTest)
		{
			using (new Timer(GetType().Name + "... ")) {
				foreach (var file in solution.AllFiles) {
					runTest(file);
				}
			}
		}
	}
	
	public class RunAllTestsCommand : ConsistencyCheckCommand
	{
		public override void Execute(Solution solution)
		{
			new ResolverTestCommand().Execute(solution);
			new ResolverTestWithoutUnresolvedFileCommand().Execute(solution);
			new ResolverTestRandomizedOrderCommand().Execute(solution);
		}
	}
	
	public class ResolverTestCommand : ConsistencyCheckCommand
	{
		public override void Execute(Solution solution)
		{
			RunTestOnAllFiles(solution, ResolverTest.RunTest);
		}
	}
	
	public class ResolverTestWithoutUnresolvedFileCommand : ConsistencyCheckCommand
	{
		public override void Execute(Solution solution)
		{
			RunTestOnAllFiles(solution, ResolverTest.RunTestWithoutUnresolvedFile);
		}
	}
	
	public class ResolverTestRandomizedOrderCommand : ConsistencyCheckCommand
	{
		public override void Execute(Solution solution)
		{
			RunTestOnAllFiles(solution, RandomizedOrderResolverTest.RunTest);
		}
	}
}
