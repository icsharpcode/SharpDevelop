// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class Build : AbstractMenuCommand
	{
		public override void Run()
		{
			new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
			ProjectService.OpenSolution.Build();
		}
	}
	
	public class Rebuild : AbstractMenuCommand
	{
		public override void Run()
		{
			new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
			ProjectService.OpenSolution.Rebuild();
		}
	}
	
	public class Clean : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.OpenSolution.Clean();
		}
	}
	
	public class Publish : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.OpenSolution.Clean();
		}
	}
	
	public class BuildProject : AbstractMenuCommand
	{
		public override void Run()
		{
			new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
			ProjectService.CurrentProject.Build();
		}
	}
	
	public class RebuildProject : AbstractMenuCommand
	{
		public override void Run()
		{
			new ICSharpCode.SharpDevelop.Commands.SaveAllFiles().Run();
			ProjectService.CurrentProject.Rebuild();
		}
	}
	
	public class CleanProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.CurrentProject.Clean();
		}
	}
	
	public class PublishProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.CurrentProject.Clean();
		}
	}
}
