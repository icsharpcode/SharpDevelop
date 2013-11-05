/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 10/6/2012
 * Time: 18:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
