// created on 06.08.2003 at 12:37

using System;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Class : AbstractClass
	{
		public Class(ICompilationUnit cu, ClassType t, Modifier m, IRegion region, IClass declaringType) : base(cu, declaringType)
		{
			classType = t;
			this.region = region;
			modifiers = (ModifierEnum)m;
		}
		
		public void UpdateModifier()
		{
			if (classType == ClassType.Enum) {
				foreach (Field f in Fields) {
					f.AddModifier(ModifierEnum.Public);
				}
				return;
			}
			if (classType != ClassType.Interface) {
				return;
			}
			foreach (Class c in InnerClasses) {
				c.modifiers = c.modifiers | ModifierEnum.Public;
			}
			foreach (IMethod m in Methods) {
				if (m is Constructor) {
					((Constructor)m).AddModifier(ModifierEnum.Public);
				} else if (m is Method) {
					((Method)m).AddModifier(ModifierEnum.Public);
				} else {
					Debug.Assert(false, "Unexpected type in method of interface. Can not set modifier to public!");
				}
			}
			foreach (Event e in Events) {
				e.AddModifier(ModifierEnum.Public);
			}
			foreach (Field f in Fields) {
				f.AddModifier(ModifierEnum.Public);
			}
			foreach (Indexer i in Indexer) {
				i.AddModifier(ModifierEnum.Public);
			}
			foreach (Property p in Properties) {
				p.AddModifier(ModifierEnum.Public);
			}
			
		}
	}
}
