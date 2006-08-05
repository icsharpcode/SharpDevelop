// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace Grunwald.BooBinding
{
	public class BooLanguageProperties : LanguageProperties
	{
		public readonly static BooLanguageProperties Instance = new BooLanguageProperties();
		
		public BooLanguageProperties() : base(StringComparer.InvariantCulture) {}
		
		public override bool SupportsExtensionMethods {
			get {
				return true;
			}
		}
		
		public override bool ImportNamespaces {
			get {
				return true;
			}
		}
		
		public override bool ImportModules {
			get {
				return true;
			}
		}
		
		public override bool CanImportClasses {
			get {
				return true;
			}
		}
		
		public override bool AllowObjectConstructionOutsideContext {
			get {
				return true;
			}
		}
		
		public override bool SupportsImplicitInterfaceImplementation {
			get {
				return true;
			}
		}
		
		public override RefactoringProvider RefactoringProvider {
			get {
				return CodeCompletion.BooRefactoringProvider.BooProvider;
			}
		}
		
		public override CodeGenerator CodeGenerator {
			get {
				return BooCodeGenerator.Instance;
			}
		}
		
		public override System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				return new Boo.Lang.CodeDom.BooCodeProvider();
			}
		}
	}
}
