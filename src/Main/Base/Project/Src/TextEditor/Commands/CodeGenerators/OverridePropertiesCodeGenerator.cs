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
	public class OverridePropertiesCodeGenerator : OldCodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "Override properties";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose properties to override";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.PropertyIndex;
			}
		}
		
		public OverridePropertiesCodeGenerator(IClass currentClass) : base(currentClass)
		{
			foreach (IClass c in currentClass.ClassInheritanceTree) {
				if (c.FullyQualifiedName != currentClass.FullyQualifiedName) {
					foreach (IProperty property in c.Properties) {
						if (!property.IsPrivate && (property.IsAbstract || property.IsVirtual || property.IsOverride)) {
							Content.Add(new PropertyWrapper(property));
						}
					}
				}
			}
			Content.Sort();
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				PropertyWrapper pw = (PropertyWrapper)items[i];
				
				string parameters = String.Empty;
				string paramList  = String.Empty;
				string returnType = (fileExtension == ".vb" ? vba : csa).Convert(pw.Property.ReturnType);
				
				for (int j = 0; j < pw.Property.Parameters.Count; ++j) {
					paramList  += pw.Property.Parameters[j].Name;
					parameters += (fileExtension == ".vb" ? vba : csa).Convert(pw.Property.Parameters[j]);
					if (j + 1 < pw.Property.Parameters.Count) {
						parameters += ", ";
						paramList  += ", ";
					}
				}
				
				
				if (fileExtension == ".vb"){
					editActionHandler.InsertString(vba.Convert(pw.Property.Modifiers) + "Overrides ");++numOps;
					editActionHandler.InsertString("Property ");++numOps;
					editActionHandler.InsertString(pw.Property.Name + "()");++numOps;
				} else {
					editActionHandler.InsertString(csa.Convert(pw.Property.Modifiers) + "override " + returnType + " " + pw.Property.Name);++numOps;
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
					editActionHandler.InsertString("Get");++numOps;
				} else {
					editActionHandler.InsertString("get");++numOps;
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
					editActionHandler.InsertString("Return MyBase." + pw.Property.Name); ++numOps;
				} else {
					editActionHandler.InsertString("return base." + pw.Property.Name); ++numOps;
				}
				
				Return();

				if(fileExtension == ".vb") {
					editActionHandler.InsertString("End Get"); ++numOps;
				} else {
					editActionHandler.InsertChar('}'); ++numOps;
				}

				Return();
				
				if(fileExtension == ".vb") {
					editActionHandler.InsertString("Set");++numOps;
				} else {
					editActionHandler.InsertString("set");++numOps;
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
					editActionHandler.InsertString("MyBase." + pw.Property.Name + " = value"); ++numOps;
				} else {
					editActionHandler.InsertString("base." + pw.Property.Name + " = value"); ++numOps;
				}
				
				Return();

				if(fileExtension == ".vb") {
					editActionHandler.InsertString("End Set"); ++numOps;
				} else {
					editActionHandler.InsertChar('}'); ++numOps;
				}

				Return();

				if(fileExtension == ".vb") {
					editActionHandler.InsertString("End Property"); ++numOps;
				} else {
					editActionHandler.InsertChar('}'); ++numOps;
				}
				
				Return();
				Return();
				IndentLine();
			}
		}
	
		class PropertyWrapper : IComparable
		{
			IProperty property;
			
			public IProperty Property {
				get {
					return property;
				}
			}
			
			public int CompareTo(object other)
			{
				return property.Name.CompareTo(((PropertyWrapper)other).property.Name);
			}
			
			
			public PropertyWrapper(IProperty property)
			{
				this.property = property;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
				return ambience.Convert(property);
			}
		}
	}
}
