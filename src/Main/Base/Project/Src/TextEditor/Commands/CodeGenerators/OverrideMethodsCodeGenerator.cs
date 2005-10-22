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
	public class OverrideMethodsCodeGenerator : OldCodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "Override methods";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose methods to override";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.MethodIndex;
			}
		}
		
		public OverrideMethodsCodeGenerator(IClass currentClass) : base(currentClass)
		{
			foreach (IClass c in currentClass.ClassInheritanceTree) {
				if (c.FullyQualifiedName != currentClass.FullyQualifiedName) {
					foreach (IMethod method in c.Methods) {
						if (!method.IsPrivate && (method.IsAbstract || method.IsVirtual || method.IsOverride)) {
							Content.Add(new MethodWrapper(method));
						}
					}
				}
			}
			Content.Sort();
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
//			bool moveToMethod = sf.SelectedItems.Count == 1;
//			int  caretPos     = 0;
			for (int i = 0; i < items.Count; ++i) {
				MethodWrapper mw = (MethodWrapper)items[i];
				
				string parameters = String.Empty;
				string paramList  = String.Empty;
				string returnType = (fileExtension == ".vb" ? vba : csa).Convert(mw.Method.ReturnType);
				
				for (int j = 0; j < mw.Method.Parameters.Count; ++j) {
					paramList  += mw.Method.Parameters[j].Name;
					parameters += (fileExtension == ".vb" ? vba : csa).Convert(mw.Method.Parameters[j]);
					if (j + 1 < mw.Method.Parameters.Count) {
						parameters += ", ";
						paramList  += ", ";
					}
				}
				if (fileExtension == ".vb"){
					editActionHandler.InsertString(vba.Convert(mw.Method.Modifiers) + "Overrides ");++numOps;
					if (mw.Method.ReturnType.FullyQualifiedName != "System.Void") {
						editActionHandler.InsertString("Function ");++numOps;
					} else {
						editActionHandler.InsertString("Sub ");++numOps;
					}
					editActionHandler.InsertString(mw.Method.Name + "(" + parameters + ")");++numOps;
					if (mw.Method.ReturnType.FullyQualifiedName != "System.Void") {
						editActionHandler.InsertString(" As " + returnType);++numOps;
					}
				} else {
					editActionHandler.InsertString(csa.Convert(mw.Method.Modifiers) + "override " + returnType + " " + mw.Method.Name + "(" + parameters + ")");++numOps;
					if (StartCodeBlockInSameLine) {
						editActionHandler.InsertString(" {");
					} else {
						Return();
						editActionHandler.InsertString("{");
					}
					++numOps;
				}
				
				
				Return();
				
				if(fileExtension == ".vb") {
					if (mw.Method.ReturnType.FullyQualifiedName != "System.Void") {
						editActionHandler.InsertString("Return MyBase." + mw.Method.Name + "(" + paramList + ")");++numOps;
						Return();
						editActionHandler.InsertString("End Function");
					} else {
						editActionHandler.InsertString("MyBase." + mw.Method.Name + "(" + paramList + ")");++numOps;
						Return();
						editActionHandler.InsertString("End Sub");
					}
				} else {
					if (mw.Method.ReturnType.FullyQualifiedName != "System.Void") {
						string str = "return base." + mw.Method.Name + "(" + paramList + ");";
						editActionHandler.InsertString(str);++numOps;
					} else {
						string str = "base." + mw.Method.Name + "(" + paramList + ");";
						editActionHandler.InsertString(str);++numOps;
					}
					Return();
					editActionHandler.InsertChar('}');
				}
				++numOps;
				
//				caretPos = editActionHandler.Document.Caret.Offset;

				
				Return();
				IndentLine();
			}
//			if (moveToMethod) {
//				editActionHandler.Document.Caret.Offset = caretPos;
//			}
		}
		
		class MethodWrapper : IComparable
		{
			IMethod method;
			
			public IMethod Method {
				get {
					return method;
				}
			}
			
			public int CompareTo(object other)
			{
				return method.Name.CompareTo(((MethodWrapper)other).method.Name);
			}
			
			
			public MethodWrapper(IMethod method)
			{
				this.method = method;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
				return ambience.Convert(method);
			}
		}
	}
}
