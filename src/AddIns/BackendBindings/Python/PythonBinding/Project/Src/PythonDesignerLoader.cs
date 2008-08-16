// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.FormsDesigner;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	public class PythonDesignerLoader : AbstractCodeDomDesignerLoader
	{
		public PythonDesignerLoader(IDesignerGenerator generator)
			: base(generator)
		{
		}
		
		/// <summary>
		/// Gets the form or user control's code from the document and 
		/// parses it using the PythonProvider class.
		/// </summary>
		protected override CodeCompileUnit Parse()
		{
			return PythonDesignerCodeDomGenerator.Parse(this.Generator.ViewContent.DesignerCodeFileContent);
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			this.Generator.MergeFormChanges(unit);
		}
		
		protected override CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			string content = this.Generator.ViewContent.DesignerCodeFileContent;
			
			if (content.Contains(@"resources.GetObject('$this.AutoScroll')")) {
				return CodeDomLocalizationModel.PropertyAssignment;
			} else if (content.Contains(@"resources.ApplyResources(")) {
				return CodeDomLocalizationModel.PropertyReflection;
			}
			
			return CodeDomLocalizationModel.None;
		}
	}
}
