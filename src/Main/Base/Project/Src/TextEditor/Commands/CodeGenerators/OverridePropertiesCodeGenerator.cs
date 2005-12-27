// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser.AST;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class OverridePropertiesCodeGenerator : CodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "Override properties";
			}
		}
		
		public override string Hint {
			get {
				return "Choose properties to override";
			}
		}
		
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.PropertyIndex;
			}
		}
		
		protected override void InitContent()
		{
			foreach (IClass c in currentClass.ClassInheritanceTree) {
				if (c.FullyQualifiedName != currentClass.FullyQualifiedName) {
					foreach (IProperty property in c.Properties) {
						if (!property.IsPrivate && (property.IsAbstract || property.IsVirtual || property.IsOverride)) {
							bool alreadyAdded = false;
							foreach (PropertyWrapper w in Content) {
								if (w.Property.Name == property.Name) {
									alreadyAdded = true;
									break;
								}
							}
							if (!alreadyAdded) {
								Content.Add(new PropertyWrapper(property));
							}
						}
					}
				}
			}
			Content.Sort();
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (PropertyWrapper wrapper in items) {
				nodes.Add(codeGen.GetOverridingMethod(wrapper.Property, this.classFinderContext));
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
