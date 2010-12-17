// 
// CSharpModifierToken.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp
{
	
	public class CSharpModifierToken : CSharpTokenNode
	{
		Modifiers modifier;
		
		public Modifiers Modifier {
			get { return modifier; }
			set {
				modifier = value;
				if (!lengthTable.TryGetValue (modifier, out tokenLength))
					throw new InvalidOperationException ("Modifier " + modifier + " is invalid.");
			}
		}
		
		static Dictionary<Modifiers, int> lengthTable = new Dictionary<Modifiers, int> ();
		static CSharpModifierToken ()
		{
			lengthTable[Modifiers.New] = "new".Length;
			lengthTable[Modifiers.Public] = "public".Length;
			lengthTable[Modifiers.Protected] = "protected".Length;
			lengthTable[Modifiers.Private] = "private".Length;
			lengthTable[Modifiers.Internal] = "internal".Length;
			lengthTable[Modifiers.Abstract] = "abstract".Length;
			lengthTable[Modifiers.Virtual] = "virtual".Length;
			lengthTable[Modifiers.Sealed] = "sealed".Length;
			lengthTable[Modifiers.Static] = "static".Length;
			lengthTable[Modifiers.Override] = "override".Length;
			lengthTable[Modifiers.Readonly] = "readonly".Length;
			lengthTable[Modifiers.Const] = "const".Length;
			lengthTable[Modifiers.Partial] = "partial".Length;
			lengthTable[Modifiers.Extern] = "extern".Length;
			lengthTable[Modifiers.Volatile] = "volatile".Length;
			lengthTable[Modifiers.Unsafe] = "unsafe".Length;
			lengthTable[Modifiers.Override] = "override".Length; 
		}
		
		public CSharpModifierToken (DomLocation location, Modifiers modifier) : base (location, 0)
		{
			this.Modifier = modifier;
		}
	}
}