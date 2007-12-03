// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.FormsDesigner;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	public class PythonDesignerLoaderProvider : IDesignerLoaderProvider
	{
		IDocument document;
		
		public PythonDesignerLoaderProvider(IDocument document)
		{
			this.document = document;
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new PythonDesignerLoader(document, generator);
		}	
	}
}
