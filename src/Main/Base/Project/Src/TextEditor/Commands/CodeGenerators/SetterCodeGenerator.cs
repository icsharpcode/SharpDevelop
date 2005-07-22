// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class SetterCodeGenerator : AbstractPropertyCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Setter";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose fields to generate setters";
			}
		}
		
		public SetterCodeGenerator(IClass currentClass) : base(currentClass)
		{
		}
		
		protected override void GeneratePropertyBody(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			GenerateSetter(editActionHandler, fw, fileExtension);
		}
	}

}
