// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser.Ast;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class OverrideMethodsCodeGenerator : CodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.OverrideMethods}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.OverrideMethods.Hint}";
			}
		}
		
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.MethodIndex;
			}
		}
		
		protected override void InitContent()
		{
			foreach (IClass c in currentClass.ClassInheritanceTree) {
				if (c.FullyQualifiedName != currentClass.FullyQualifiedName) {
					foreach (IMethod method in c.Methods) {
						if (!method.IsPrivate && (method.IsAbstract || method.IsVirtual || method.IsOverride)) {
							MethodWrapper newWrapper = new MethodWrapper(method);
							if (!Content.Contains(newWrapper)) {
								Content.Add(newWrapper);
							}
						}
					}
				}
			}
			Content.Sort();
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (MethodWrapper wrapper in items) {
				nodes.Add(codeGen.GetOverridingMethod(wrapper.Method, this.classFinderContext));
			}
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
				return ToString().CompareTo(((MethodWrapper)other).ToString());
			}
			
			public MethodWrapper(IMethod method)
			{
				this.method = method;
			}
			
			public override bool Equals(object obj)
			{
				MethodWrapper other = (MethodWrapper)obj;
				if (method.Name != other.method.Name)
					return false;
				return 0 == ICSharpCode.SharpDevelop.DiffUtility.Compare(method.Parameters, other.method.Parameters);
			}
			
			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}
			
			string cachedStringRepresentation;
			
			public override string ToString()
			{
				if (cachedStringRepresentation == null) {
					IAmbience ambience = AmbienceService.CurrentAmbience;
					ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
					cachedStringRepresentation = ambience.Convert(method);
				}
				return cachedStringRepresentation;
			}
		}
	}
}
