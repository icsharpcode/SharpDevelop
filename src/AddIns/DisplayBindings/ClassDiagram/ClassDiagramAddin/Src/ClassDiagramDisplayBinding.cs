// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

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
