// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcModelClassViewModelsForSelectedFolder
	{
		ISelectedMvcFolder selectedFolder;
		List<MvcModelClassViewModel> modelClasses;
		
		public MvcModelClassViewModelsForSelectedFolder(ISelectedMvcFolder selectedFolder)
		{
			this.selectedFolder = selectedFolder;
		}
		
		public IEnumerable<MvcModelClassViewModel> ModelClasses {
			get {
				if (modelClasses != null) {
					return modelClasses;
				}
				return new MvcModelClassViewModel[0];
			}
		}
		
		public void GetModelClasses()
		{
			if (modelClasses == null) {
				modelClasses = new List<MvcModelClassViewModel>();
				
				foreach (IMvcClass mvcClass in GetModelClassesFromProject()) {
					var modelClassViewModel = new MvcModelClassViewModel(mvcClass);
					modelClasses.Add(modelClassViewModel);
				}
			}
		}
		
		IEnumerable<IMvcClass> GetModelClassesFromProject()
		{
			return selectedFolder.Project.GetModelClasses();
		}
	}
}
