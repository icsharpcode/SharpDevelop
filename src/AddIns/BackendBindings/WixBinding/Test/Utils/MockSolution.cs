/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 2/27/2013
 * Time: 00:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;
using Rhino.Mocks;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Description of MockSolution.
	/// </summary>
	public class MockSolution
	{
		public static ISolution Create()
		{
			ISolution solution = MockRepository.GenerateStrictMock<ISolution>();
			solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			solution.Stub(s => s.ActiveConfiguration).Return(new ConfigurationAndPlatform("Debug", "Any CPU"));
			return solution;
		}
	}
}
