// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class AbstractClassImplementorCodeGenerator : InterfaceOrAbstractClassCodeGenerator
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.AbstractClass}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.AbstractClass.Hint}";
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (IProperty property in currentClass.DefaultReturnType.GetProperties()) {
				if (property.IsAbstract) {
					AttributedNode node = CodeGenerator.ConvertMember(property, classFinderContext);
					node.Modifier &= ~(Modifiers.Abstract | Modifiers.Virtual);
					node.Modifier |= Modifiers.Override;
					nodes.Add(node);
				}
			}
			foreach (IMethod method in currentClass.DefaultReturnType.GetMethods()) {
				if (method.IsAbstract) {
					AttributedNode node = CodeGenerator.ConvertMember(method, classFinderContext);
					node.Modifier &= ~(Modifiers.Abstract | Modifiers.Virtual);
					node.Modifier |= Modifiers.Override;
					nodes.Add(node);
				}
			}
		}
		
		protected override void InitContent()
		{
			if (currentClass.ClassType != ICSharpCode.SharpDevelop.Dom.ClassType.Class)
				return;
			for (int i = 0; i < currentClass.BaseTypes.Count; i++) {
				IReturnType baseType = currentClass.GetBaseType(i);
				IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
				if (baseClass != null && baseClass.ClassType == ICSharpCode.SharpDevelop.Dom.ClassType.Class && baseClass.IsAbstract) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
	}
}
