// created on 06.08.2003 at 12:34

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Indexer : AbstractIndexer
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Indexer(ReturnType type, List<IParameter> parameters, Modifier m, IRegion region, IRegion bodyRegion)
		{
			returnType      = type;
			this.Parameters = parameters;
			this.region     = region;
			this.bodyRegion = bodyRegion;
			modifiers = (ModifierEnum)m;
			if (modifiers == ModifierEnum.None) {
				modifiers = ModifierEnum.Private;
			}
		}
	}
}
