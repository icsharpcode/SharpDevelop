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
	public abstract class AbstractPropertyCodeGenerator : AbstractFieldCodeGenerator
	{
		public AbstractPropertyCodeGenerator(IClass currentClass) : base(currentClass)
		{
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.PropertyIndex;
			}
		}
		
		public static string GetPropertyName(string fieldName)
		{
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				FieldWrapper fw = (FieldWrapper)items[i];
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("Public " + (fw.Field.IsStatic ? "Shared " : "") + "Property " + GetPropertyName(fw.Field.Name) + " As " + vba.Convert(fw.Field.ReturnType));
				} else {
					editActionHandler.InsertString("public " + (fw.Field.IsStatic ? "static " : "") + csa.Convert(fw.Field.ReturnType) + " " + GetPropertyName(fw.Field.Name));
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");
					} else {
						Return();
						editActionHandler.InsertString("{");
					}
					++numOps;
				}
				++numOps;
				
				
				Return();
				
				GeneratePropertyBody(editActionHandler, fw, fileExtension);
				
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("End Property");
				} else {
					editActionHandler.InsertChar('}');
				}
				
				++numOps;
				Return();
				IndentLine();
			}
		}
		
		protected void GenerateGetter(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Get");
			} else {
				editActionHandler.InsertString("get");
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");
				} else {
					Return();
					editActionHandler.InsertString("{");
				}
				++numOps;
				
			}
			++numOps;
			Return();
			Indent();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Return " + fw.Field.Name);
			} else {
				editActionHandler.InsertString("return " + fw.Field.Name+ ";");
			}
			++numOps;
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Get");
			} else {
				editActionHandler.InsertString("}");
			}
			++numOps;
			Return();
		}
		
		protected void GenerateSetter(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Set");
			} else {
				editActionHandler.InsertString("set");
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");
				} else {
					Return();
					editActionHandler.InsertString("{");
				}
				++numOps;
				
			}
			++numOps;
			Return();
			Indent();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString(fw.Field.Name+ " = Value");
			} else {
				editActionHandler.InsertString(fw.Field.Name+ " = value;");
			}
			++numOps;
			Return();
			
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Set");
			} else {
				editActionHandler.InsertString("}");
			}
			++numOps;
			Return();
		}
		
		protected abstract void GeneratePropertyBody(TextArea editActionHandler, FieldWrapper fw, string fileExtension);
	}
}
