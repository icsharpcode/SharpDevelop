// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Commands;

using ICSharpCode.SharpDevelop.AddIns.AssemblyScout;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout.Commands
{
	public class LoadAssemblyCommand : AssemblyScoutCommand
	{
		public override void Run()
		{
			AssemblyScoutViewContent vc = GetViewContent();
			if (vc == null) return;
			
			vc.Tree.LoadAssembly(null, null);
		}
	}
	
	public class LoadRefAssembliesCommand : AssemblyScoutCommand
	{
		public override void Run()
		{
			AssemblyScoutViewContent vc = GetViewContent();
			if (vc == null) return;
			
			vc.LoadRefAssemblies();
		}
		
	}
	
	public class LoadStdAssembliesCommand : AssemblyScoutCommand
	{
		public override void Run()
		{
			AssemblyScoutViewContent vc = GetViewContent();
			if (vc == null) return;
			
			vc.LoadStdAssemblies();
		}
		
	}
	
	public class BackCommand : AssemblyScoutCommand
	{
		public override void Run()
		{
			AssemblyScoutViewContent vc = GetViewContent();
			if (vc == null) return;
			
			vc.Tree.GoBack();
		}
		
	}
	
	public class AssemblyScoutCommand : AbstractMenuCommand
	{
		protected AssemblyScoutViewContent GetViewContent()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			return (window.ViewContent as AssemblyScoutViewContent);
		}
		
		public override void Run()
		{
			
		}
	}
}
