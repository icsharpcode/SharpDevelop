// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ConstructorCodeGenerator : AbstractFieldCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Constructor";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose fields to initialize by constructor";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.MethodIndex;
			}
		}
		
		public ConstructorCodeGenerator(IClass currentClass) : base(currentClass)
		{
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Public Sub New(");
			} else {
				editActionHandler.InsertString("public " + currentClass.Name + "(");
			}
			++numOps;
			
			for (int i = 0; i < items.Count; ++i) {
				FieldWrapper fw = (FieldWrapper)items[i];
				if (fileExtension == ".vb") {
					editActionHandler.InsertString(fw.Field.Name + " As " + vba.Convert(fw.Field.ReturnType));
				} else {
					editActionHandler.InsertString(csa.Convert(fw.Field.ReturnType) + " " + fw.Field.Name);
				}
				++numOps;
				if (i + 1 < items.Count) {
					editActionHandler.InsertString(", ");
					++numOps;
				}
			}
			
			editActionHandler.InsertChar(')');++numOps;
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("MyBase.New");
			} else {
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");
				} else {
					Return();
					editActionHandler.InsertString("{");
				}
			}
			++numOps;
			Return();
			for (int i = 0; i < items.Count; ++i) {
				Indent();
				FieldWrapper fw = (FieldWrapper)items[i];
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("Me." + fw.Field.Name + " = " + fw.Field.Name);
				} else {
					editActionHandler.InsertString("this." + fw.Field.Name + " = " + fw.Field.Name + ";");
				}
				++numOps;
				Return();
			}
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Sub");
			} else {
				editActionHandler.InsertChar('}');
			}
			++numOps;
			Return();
			IndentLine();
		}
	}
}
