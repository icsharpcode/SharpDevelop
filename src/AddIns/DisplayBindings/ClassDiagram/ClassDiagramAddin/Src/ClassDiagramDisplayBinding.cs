// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

namespace ClassDiagramAddin
{
	public class ClassDiagramDisplayBinding : IDisplayBinding
	{
		public ClassDiagramDisplayBinding ()
		{
		//	ResourceService.RegisterImages("ClassDiagram", Assembly.GetExecutingAssembly());
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new ClassDiagramViewContent(file);
		}
	}
}
