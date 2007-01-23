/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 24/09/2006
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
