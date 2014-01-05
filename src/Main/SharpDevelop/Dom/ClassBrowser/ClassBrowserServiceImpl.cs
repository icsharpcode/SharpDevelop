// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of ClassBrowserServiceImpl.
	/// </summary>
	[SDService("SD.ClassBrowser")]
	class ClassBrowserServiceImpl : IClassBrowser
	{
		readonly WorkspaceModel workspace = new WorkspaceModel();

		public WorkspaceModel CurrentWorkspace {
			get { return workspace; }
		}
		
		public ICollection<IAssemblyList> AssemblyLists {
			get { return workspace.AssemblyLists; }
		}

		public IAssemblyList MainAssemblyList {
			get { return workspace.MainAssemblyList; }
			set { workspace.MainAssemblyList = value; }
		}
		
		public IAssemblyList UnpinnedAssemblies {
			get { return workspace.UnpinnedAssemblies; }
			set { workspace.UnpinnedAssemblies = value; }
		}
		
		public IAssemblyModel FindAssemblyModel(FileName fileName)
		{
			return workspace.FindAssemblyModel(fileName);
		}
		
		public bool GoToEntity(ICSharpCode.NRefactory.TypeSystem.IEntity entity)
		{
			var pad = SD.Workbench.GetPad(typeof(ClassBrowserPad));
			pad.BringPadToFront();
			var content = (ClassBrowserPad)pad.PadContent;
			return content.GoToEntity(entity);
		}
		
		public bool GotoAssemblyModel(IAssemblyModel assemblyModel)
		{
			var pad = SD.Workbench.GetPad(typeof(ClassBrowserPad));
			pad.BringPadToFront();
			var content = (ClassBrowserPad)pad.PadContent;
			return content.GotoAssemblyModel(assemblyModel);
		}
	}
}
