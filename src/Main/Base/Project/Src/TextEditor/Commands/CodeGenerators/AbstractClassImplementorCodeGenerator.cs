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
	public class AbstractClassImplementorCodeGenerator : CodeGenerator
	{
		
		public override string CategoryName {
			get {
				return "Abstract class overridings";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose abstract class to override";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.InterfaceIndex;
			}
		}
		
		public AbstractClassImplementorCodeGenerator(IClass currentClass) : base(currentClass)
		{
			for (int i = 0; i < currentClass.BaseTypes.Count; i++) {
				IReturnType baseType = currentClass.GetBaseType(i);
				IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
				if (baseClass != null && baseClass.ClassType == ClassType.Class && baseClass.IsAbstract) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				ClassWrapper cw = (ClassWrapper)items[i];
				GenerateInterface(cw.ClassType, fileExtension);
			}
		}
		
		void GenerateInterface(IReturnType intf, string fileExtension)
		{
			Return();Return();
			
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("#Region \"" + intf.FullyQualifiedName + " abstract class implementation\"\n\t\t");
			} else {
				editActionHandler.InsertString("#region " + intf.FullyQualifiedName + " abstract class implementation\n\t\t");
			}
			++numOps;
			
			foreach (IProperty property in intf.GetProperties()) {
				if (!property.IsAbstract) {
					continue;
				}
				string returnType = (fileExtension == ".vb" ? vba : csa).Convert(property.ReturnType);
				if (property.IsProtected) {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("Protected ");
					} else {
						editActionHandler.InsertString("protected ");
					}
					++numOps;
				} else {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("Public ");
					} else {
						editActionHandler.InsertString("public ");
					}
					++numOps;
				}
				
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("override " + returnType + " " + property.Name);
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");++numOps;
					} else {
						Return();
						editActionHandler.InsertString("{");++numOps;
					}
				} else {
					editActionHandler.InsertString("Overrides Property " + property.Name + " As " + returnType + "\n");
				}
				++numOps;
				if (property.CanGet) {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("\tGet");++numOps;
						Return();
						editActionHandler.InsertString("\t\tReturn " + GetReturnValue(returnType));++numOps;
						Return();
						editActionHandler.InsertString("\tEnd Get");++numOps;
						Return();
					} else {
						editActionHandler.InsertString("\tget");++numOps;
						if (StartCodeBlockInSameLine) {
							editActionHandler.InsertString(" {");++numOps;
						} else {
							Return();
							editActionHandler.InsertString("{");++numOps;
						}
						
						Return();
						editActionHandler.InsertString("\t\treturn " + GetReturnValue(returnType) +";");++numOps;
						Return();
						editActionHandler.InsertString("\t}");++numOps;
						Return();
					}
				}
				
				if (property.CanSet) {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("\tSet");++numOps;
						Return();
						editActionHandler.InsertString("\tEnd Set");++numOps;
						Return();
					} else {
						editActionHandler.InsertString("\tset");++numOps;
						if (StartCodeBlockInSameLine) {
							editActionHandler.InsertString(" {");++numOps;
						} else {
							Return();
							editActionHandler.InsertString("{");++numOps;
						}
						
						Return();
						editActionHandler.InsertString("\t}");++numOps;
						Return();
					}
				}
				
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("End Property");++numOps;
				} else {
					editActionHandler.InsertChar('}');++numOps;
				}
				
				Return();
				Return();
				IndentLine();
			}
			
			foreach (IMethod method in intf.GetMethods()) {
				string parameters = String.Empty;
				string returnType = (fileExtension == ".vb" ? vba : csa).Convert(method.ReturnType);
				if (!method.IsAbstract) {
					continue;
				}
				for (int j = 0; j < method.Parameters.Count; ++j) {
					parameters += (fileExtension == ".vb" ? vba : csa).Convert(method.Parameters[j]);
					if (j + 1 < method.Parameters.Count) {
						parameters += ", ";
					}
				}
				if (method.IsProtected) {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("Protected ");
					} else {
						editActionHandler.InsertString("protected ");
					}
				} else {
					if (fileExtension == ".vb") {
						editActionHandler.InsertString("Public ");
					} else {
						editActionHandler.InsertString("public ");
					}
				}
				bool isSub = returnType == "void";
				if (fileExtension == ".vb") {
					
					editActionHandler.InsertString("Overrides " + (isSub ? "Sub " : "Function ") + method.Name + "(" + parameters + ") As " + returnType);++numOps;
					Return();
				} else {
					editActionHandler.InsertString("override " + returnType + " " + method.Name + "(" + parameters + ")");++numOps;
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");++numOps;
					} else {
						Return();
						editActionHandler.InsertString("{");++numOps;
					}
					Return();
				}
				
				switch (returnType) {
					case "void":
						break;
					default:
						if (fileExtension == ".vb") {
							editActionHandler.InsertString("Return " + GetReturnValue(returnType));++numOps;
						} else {
							editActionHandler.InsertString("return " + GetReturnValue(returnType) + ";");++numOps;
						}
						break;
				}
				Return();
				
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("End " + (isSub ? "Sub" : "Function"));
				} else {
					editActionHandler.InsertChar('}');++numOps;
				}
				Return();
				Return();
				IndentLine();
			}
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("#End Region");++numOps;
			} else {
				editActionHandler.InsertString("#endregion");++numOps;
			}
			
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
			IReturnType c;
			public IReturnType ClassType {
				get {
					return c;
				}
			}
			public ClassWrapper(IReturnType c)
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
