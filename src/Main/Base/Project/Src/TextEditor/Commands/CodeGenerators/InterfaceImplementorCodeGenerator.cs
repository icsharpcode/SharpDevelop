// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class InterfaceOrAbstractClassCodeGenerator : CodeGeneratorBase
	{
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.InterfaceIndex;
			}
		}
		
		protected class ClassWrapper
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
				IAmbience ambience = AmbienceService.GetCurrentAmbience();
				ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
				return ambience.Convert(c);
			}
		}
	}
	
	public class InterfaceImplementorCodeGenerator : InterfaceOrAbstractClassCodeGenerator
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.ImplementInterface}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.ImplementInterface.Hint}";
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (ClassWrapper w in items) {
				codeGen.ImplementInterface(nodes, w.ClassType,
				                           !currentClass.ProjectContent.Language.SupportsImplicitInterfaceImplementation,
				                           currentClass);
			}
		}
		
		protected override void InitContent()
		{
			for (int i = 0; i < currentClass.BaseTypes.Count; i++) {
				IReturnType baseType = currentClass.GetBaseType(i);
				IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
				if (baseClass != null && baseClass.ClassType == ICSharpCode.SharpDevelop.Dom.ClassType.Interface) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
	}
}
