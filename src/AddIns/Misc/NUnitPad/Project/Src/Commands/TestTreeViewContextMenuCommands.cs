// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NUnitPad
{
	/// <summary>
	/// Description of RunTestsCommand
	/// </summary>
	public class RunTestsCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Creates a new RunTestsCommand
		/// </summary>
		public RunTestsCommand()
		{
		}
	
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			((TestTreeView)Owner).RunTests();
		}
	}
	/// <summary>
	/// Description of GotoDefinitionCommand
	/// </summary>
	public class GotoDefinitionCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Creates a new GotoDefinitionCommand
		/// </summary>
		public GotoDefinitionCommand()
		{
		}
	
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			((TestTreeView)Owner).GotoDefinition();
		}
	}
}
