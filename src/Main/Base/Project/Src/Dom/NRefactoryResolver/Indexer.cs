// created on 06.08.2003 at 12:34

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Indexer : AbstractIndexer
	{
		public Indexer(ReturnType type, List<IParameter> parameters, Modifier m, IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType)
		{
			this.ReturnType      = type;
			this.Parameters = parameters;
			this.Region     = region;
			this.bodyRegion = bodyRegion;
			Modifiers = (ModifierEnum)m;
			if (Modifiers == ModifierEnum.None) {
				Modifiers = ModifierEnum.Private;
			}
		}
	}
}
