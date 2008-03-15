// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class OverridePropertiesCodeGenerator : CodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.OverrideProperties}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.OverrideProperties.Hint}";
			}
		}
		
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.PropertyIndex;
			}
		}
		
		protected override void InitContent()
		{
			foreach (IProperty p in OverrideCompletionDataProvider.GetOverridableProperties(currentClass)) {
				Content.Add(new PropertyWrapper(p));
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
				IAmbience ambience = AmbienceService.GetCurrentAmbience();
				ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
				return ambience.Convert(property);
			}
		}
	}
}
