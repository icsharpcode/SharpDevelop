
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	internal class Modifiers
	{
		Modifier cur;
		
		public Modifier Modifier {
			get {
				return cur;
			}
		}
		
		public bool isNone { get { return cur == Modifier.None; } }
		
		public bool Contains(Modifier m)
		{
			return ((cur & m) != 0);
		}
		
		public void Add(Modifier m) 
		{
			if ((cur & m) == 0) {
				cur |= m;
			} else {
//				parser.Error("modifier " + m + " already defined");
			}
		}
		
		public void Add(Modifiers m)
		{
			Add(m.cur);
		}
		
		public void Check(Modifier allowed)
		{
			Modifier wrong = cur & (allowed ^ Modifier.All);
			if (wrong != Modifier.None) {
//				parser.Error("modifier(s) " + wrong + " not allowed here");
			}
		}
	}
}
