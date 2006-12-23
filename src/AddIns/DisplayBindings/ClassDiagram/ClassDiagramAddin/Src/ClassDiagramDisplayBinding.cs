// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
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
		
		public IViewContent CreateContentForFile(string fileName)
		{
			ClassDiagramViewContent vc = new ClassDiagramViewContent();
			vc.Load(fileName);
			return vc;
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return false;
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			throw new NotImplementedException();
		}
	}
}
