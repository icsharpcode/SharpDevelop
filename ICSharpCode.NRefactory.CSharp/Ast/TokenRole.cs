using System;

namespace ICSharpCode.NRefactory.CSharp
{
	public sealed class TokenRole : Role<CSharpTokenNode>
	{
		public string Token {
			get;
			private set;
		}
		
		public int Length {
			get;
			private set;
		}
		
		public TokenRole (string token) : base (token, CSharpTokenNode.Null)
		{
			this.Token = token;
			this.Length = token.Length;
		}
	}
}

