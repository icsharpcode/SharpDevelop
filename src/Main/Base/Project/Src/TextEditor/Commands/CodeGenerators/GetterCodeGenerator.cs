// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GetterCodeGenerator : AbstractPropertyCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Getter";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose fields to generate getters";
			}
		}
		
		public GetterCodeGenerator(IClass currentClass) : base(currentClass)
		{
		}
		
		protected override void GeneratePropertyBody(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			GenerateGetter(editActionHandler, fw, fileExtension);
		}
	}

}
