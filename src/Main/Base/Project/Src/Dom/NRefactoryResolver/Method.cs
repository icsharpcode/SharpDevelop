// created on 06.08.2003 at 12:35
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Method : AbstractMethod
	{
		public Method(string name, ReturnType type, Modifier m, IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region     = region;
			this.bodyRegion = bodyRegion;
			Modifiers = (ModifierEnum)m;
			if (Modifiers == ModifierEnum.None) {
				Modifiers = ModifierEnum.Private;
			}
		}
	}
}
