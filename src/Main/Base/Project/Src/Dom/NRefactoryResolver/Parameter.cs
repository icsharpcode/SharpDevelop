// created on 07.08.2003 at 20:12

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Parameter : AbstractParameter
	{
		public Parameter(string name, ReturnType type, IRegion region)
		{
			Name = name;
			returnType = type;
			this.region = region;
		}
	}
}
