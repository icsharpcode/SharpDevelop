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
