// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class LanguageProperties
	{
		public readonly static LanguageProperties CSharp = new CSharpProperties();
		public readonly static LanguageProperties VBNet = new VBNetProperties();
		
		StringComparer nameComparer;
		
		public LanguageProperties(StringComparer nameComparer, CodeGenerator codeGenerator)
		{
			this.nameComparer = nameComparer;
			this.codeGenerator = codeGenerator;
		}
		
		public StringComparer NameComparer {
			get {
				return nameComparer;
			}
		}
		
		CodeGenerator codeGenerator;
		
		public CodeGenerator CodeGenerator {
			get {
				return codeGenerator;
			}
		}
		
		/// <summary>
		/// Gets if the language supports calling C# 3-style extension methods
		/// (first parameter = instance parameter)
		/// </summary>
		public virtual bool SupportsExtensionMethods {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if the language supports calling extension properties
		/// (first parameter = instance parameter)
		/// </summary>
		public virtual bool SupportsExtensionProperties {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if extension methods/properties are searched in imported classes (returns true) or if
		/// only the extensions from the current class, imported classes and imported modules are used
		/// (returns false). This property has no effect if the language doesn't support
		/// </summary>
		public virtual bool SearchExtensionsInClasses {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if namespaces can be imported (i.e. Imports System, Dim a As Collections.ArrayList)
		/// </summary>
		public virtual bool ImportNamespaces {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if modules are imported with their namespace (i.e. Microsoft.VisualBasic.Randomize()).
		/// </summary>
		public virtual bool ImportModules {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if classes can be imported (i.e. Imports System.Math)
		/// </summary>
		public virtual bool CanImportClasses {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Allow invoking an object constructor outside of ExpressionContext.ObjectCreation.
		/// Used for Boo, which creates instances like this: 'self.Size = Size(10, 20)'
		/// </summary>
		public virtual bool AllowObjectConstructionOutsideContext {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Gets if the language supports implicit interface implementations.
		/// </summary>
		public virtual bool SupportsImplicitInterfaceImplementation {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Gets the token that denotes a possible beginning of an indexer expression.
		/// </summary>
		public virtual string IndexerExpressionStartToken {
			get {
				return "[";
			}
		}
		
		public virtual bool ShowInNamespaceCompletion(IClass c)
		{
			return true;
		}
		
		public virtual bool ShowMember(IMember member, bool showStatic)
		{
			if (member is IProperty && ((IProperty)member).IsIndexer) {
				return false;
			}
			return member.IsStatic == showStatic;
		}
		
		/// <summary>
		/// Generates the default imports statements a new application for this language should use.
		/// </summary>
		public virtual IUsing CreateDefaultImports(IProjectContent pc)
		{
			return null;
		}
		
		public override string ToString()
		{
			return "[" + base.ToString() + "]";
		}
		
		private class CSharpProperties : LanguageProperties
		{
			public CSharpProperties() : base(StringComparer.InvariantCulture, CSharpCodeGenerator.Instance) {}
			
			public override string ToString()
			{
				return "[LanguageProperties: C#]";
			}
		}
		
		private class VBNetProperties : LanguageProperties
		{
			public VBNetProperties() : base(StringComparer.InvariantCultureIgnoreCase, VBNetCodeGenerator.Instance) {}
			
			public override bool ShowMember(IMember member, bool showStatic)
			{
				if (member is ArrayReturnType.ArrayIndexer) {
					return false;
				}
				return member.IsStatic || !showStatic;
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
			
			public override bool SupportsImplicitInterfaceImplementation {
				get {
					return false;
				}
			}
			
			public override bool CanImportClasses {
				get {
					return true;
				}
			}
			
			public override string IndexerExpressionStartToken {
				get {
					return "(";
				}
			}
			
			public override bool ShowInNamespaceCompletion(IClass c)
			{
				foreach (IAttribute attr in c.Attributes) {
					if (NameComparer.Equals(attr.Name, "Microsoft.VisualBasic.HideModuleNameAttribute"))
						return false;
					if (NameComparer.Equals(attr.Name, "HideModuleNameAttribute"))
						return false;
					if (NameComparer.Equals(attr.Name, "Microsoft.VisualBasic.HideModuleName"))
						return false;
					if (NameComparer.Equals(attr.Name, "HideModuleName"))
						return false;
				}
				return base.ShowInNamespaceCompletion(c);
			}
			
			public override IUsing CreateDefaultImports(IProjectContent pc)
			{
				DefaultUsing u = new DefaultUsing(pc);
				u.Usings.Add("Microsoft.VisualBasic");
				u.Usings.Add("System");
				u.Usings.Add("System.Collections");
				u.Usings.Add("System.Collections.Generic");
				u.Usings.Add("System.Drawing");
				u.Usings.Add("System.Diagnostics");
				u.Usings.Add("System.Windows.Forms");
				return u;
			}
			
			public override string ToString()
			{
				return "[LanguageProperties: VB.NET]";
			}
		}
	}
}
