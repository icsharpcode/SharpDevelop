/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 03.02.2006
 * Time: 23:13
 */

using System;
using Codeblast;

namespace NUnit.ConsoleRunner
{
	public class ExtendedConsoleOptions : ConsoleOptions
	{
		public ExtendedConsoleOptions(string[] args) : base(args) {}
				
		[Option(Description="File to receive test results as each test is run")]
		public string results;
		
		public bool IsResults
		{
			get 
			{
				return (results != null) && (results.Length != 0);
			}
		}				
	}
}
