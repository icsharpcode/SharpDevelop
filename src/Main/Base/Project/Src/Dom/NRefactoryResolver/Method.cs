// created on 06.08.2003 at 12:35
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Method : AbstractMethod
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Method(string name, ReturnType type, Modifier m, IRegion region, IRegion bodyRegion)
		{
			FullyQualifiedName = name;
			returnType = type;
			this.region     = region;
			this.bodyRegion = bodyRegion;
			modifiers = (ModifierEnum)m;
			if (modifiers == ModifierEnum.None) {
				modifiers = ModifierEnum.Private;
			}
		}
	}
}
