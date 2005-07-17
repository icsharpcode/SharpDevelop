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
	public class InterfaceImplementorCodeGenerator : CodeGenerator
	{
		ICompilationUnit unit;
		
		public override string CategoryName {
			get {
				return "Interface implementation";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose interfaces to implement";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.InterfaceIndex;
			}
		}
		
		public InterfaceImplementorCodeGenerator(IClass currentClass) : base(currentClass)
		{
			
			
			foreach (string className in currentClass.BaseTypes) {
				IClass baseType = ParserService.CurrentProjectContent.GetClass(className);
				
				if (baseType == null) {
					baseType = ParserService.CurrentProjectContent.GetClass(currentClass.Namespace + "." + className);
				}

				if (baseType == null) {
					this.unit = currentClass == null ? null : currentClass.CompilationUnit;
					if (unit != null) {
						foreach (IUsing u in unit.Usings) {
							baseType = u.SearchType(className);
							if (baseType != null) {
								break;
							}
						}
					}
				}
				
				if (baseType != null && baseType.ClassType == ClassType.Interface) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				ClassWrapper cw = (ClassWrapper)items[i];
				Queue interfaces = new Queue();
				interfaces.Enqueue(cw.Class);
				while (interfaces.Count > 0) {
					IClass intf = (IClass)interfaces.Dequeue();
					GenerateInterface(intf);
					
					// search an enqueue all base interfaces
					foreach (string interfaceName in intf.BaseTypes) {
						
						// first look if the interface is in the same namespace
						IClass baseType = ParserService.CurrentProjectContent.GetClass(intf.Namespace + "." + interfaceName);
						
						if (baseType == null && unit != null && unit.Usings != null) {
							foreach (IUsing u in unit.Usings) {
								baseType = u.SearchType(interfaceName);
								if (baseType != null) {
									break;
								}
							}
						}
						if (baseType != null) {
							interfaces.Enqueue(baseType);
						}
					}
				}
			}
		}
		
		void GenerateInterface(IClass intf)
		{
			Return();
			Return();
			editActionHandler.InsertString("#region " + intf.FullyQualifiedName + " interface implementation\n\t\t");++numOps;
			
			foreach (IProperty property in intf.Properties) {
				string returnType = csa.Convert(property.ReturnType);
				editActionHandler.InsertString("public " + returnType + " " + property.Name);++numOps;
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");++numOps;
				} else {
					Return();
					editActionHandler.InsertString("{");++numOps;
				}
				Return();
				
				if (property.CanGet) {
					editActionHandler.InsertString("\tget");++numOps;
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");++numOps;
					} else {
						Return();
						editActionHandler.InsertString("{");++numOps;
					}
					Return();
					Indent();Indent();
					editActionHandler.InsertString("return " + GetReturnValue(returnType) +";");++numOps;
					Return();
					Indent();
					editActionHandler.InsertString("}");++numOps;
					Return();
				}
				
				if (property.CanSet) {
					Indent();
					editActionHandler.InsertString("set");++numOps;
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");++numOps;
					} else {
						Return();
						editActionHandler.InsertString("{");++numOps;
					}
					Return();
					Indent();
					editActionHandler.InsertString("}");++numOps;
					Return();
				}
				
				editActionHandler.InsertChar('}');++numOps;
				Return();
				Return();
				IndentLine();
			}
			
			for (int i = 0; i < intf.Methods.Count; ++i) {
				IMethod method = intf.Methods[i];
				string parameters = String.Empty;
				string returnType = csa.Convert(method.ReturnType);
				
				for (int j = 0; j < method.Parameters.Count; ++j) {
					parameters += csa.Convert(method.Parameters[j]);
					if (j + 1 < method.Parameters.Count) {
						parameters += ", ";
					}
				}
				
				editActionHandler.InsertString("public " + returnType + " " + method.Name + "(" + parameters + ")");++numOps;
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");++numOps;
				} else {
					Return();
					editActionHandler.InsertString("{");++numOps;
				}
				
				Return();
				
				switch (returnType) {
					case "void":
						break;
					default:
						Indent();
						editActionHandler.InsertString("return " + GetReturnValue(returnType) + ";");++numOps;
						break;
				}
				Return();
				
				editActionHandler.InsertChar('}');++numOps;
				if (i + 1 < intf.Methods.Count) {
					Return();
					Return();
					IndentLine();
				} else {
					IndentLine();
				}
			}
			
			Return();
			editActionHandler.InsertString("#endregion");++numOps;
			Return();
		}
		
		string GetReturnValue(string returnType)
		{
			switch (returnType) {
				case "string":
					return "String.Empty";
				case "char":
					return "'\\0'";
				case "bool":
					return "false";
				case "int":
				case "long":
				case "short":
				case "byte":
				case "uint":
				case "ulong":
				case "ushort":
				case "double":
				case "float":
				case "decimal":
					return "0";
				default:
					return "null";
			}
		}
		
		class ClassWrapper
		{
			IClass c;
			public IClass Class {
				get {
					return c;
				}
			}
			public ClassWrapper(IClass c)
			{
				this.c = c;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(c);
			}
		}
	}
}
