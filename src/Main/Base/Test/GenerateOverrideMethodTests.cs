// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;
using System.Text;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Description of GenerateOverrideMethod.
	/// </summary>
	[TestFixture]
	public class GenerateOverrideMethodTests
	{
		NRefactoryResolverTests helper = new NRefactoryResolverTests();
		
		void Run(string input, string expectedOutput)
		{
			ICompilationUnit cu = helper.Parse("a.cs", input + "\nclass DerivedClass {\n \n}");
			Assert.AreEqual(2, cu.Classes.Count);
			Assert.AreEqual(1, cu.Classes[0].Methods.Count + cu.Classes[0].Properties.Count);
			IMember virtualMember;
			if (cu.Classes[0].Properties.Count > 0)
				virtualMember = cu.Classes[0].Properties[0];
			else
				virtualMember = cu.Classes[0].Methods[0];
			CSharpCodeGenerator ccg = new CSharpCodeGenerator();
			AttributedNode result = ccg.GetOverridingMethod(virtualMember, new ClassFinder(cu.Classes[1], 3, 1));
			Assert.IsNotNull(result);
			string output = ccg.GenerateCode(result, "");
			Assert.AreEqual(expectedOutput, NormalizeWhitespace(output));
		}
		
		string NormalizeWhitespace(string t)
		{
			StringBuilder b = new StringBuilder();
			bool wasWhitespace = true;
			foreach (char c in t) {
				if (char.IsWhiteSpace(c)) {
					if (!wasWhitespace) {
						wasWhitespace = true;
						b.Append(' ');
					}
				} else {
					b.Append(c);
					wasWhitespace = false;
				}
			}
			return b.ToString().Trim();
		}
		
		[Test]
		public void ReadonlyVirtualProperty()
		{
			Run("class BaseClass { public virtual int Prop { get { return 0; } } }",
			    "public override int Prop { get { return base.Prop; } }");
		}
		
		[Test]
		public void ReadonlyAbstractProperty()
		{
			Run("class BaseClass { public abstract int Prop { get; } }",
			    "public override int Prop { get { throw new NotImplementedException(); } }");
		}
		
		[Test]
		public void ReadWriteProperty()
		{
			Run("class BaseClass { public virtual int Prop { get { return 0; } set { } } }",
			    "public override int Prop { get { return base.Prop; } set { base.Prop = value; } }");
		}
		
		[Test]
		public void PublicReadProtectedWriteProperty()
		{
			Run("class BaseClass { public virtual int Prop { get { return 0; } protected set { } } }",
			    "public override int Prop { get { return base.Prop; } protected set { base.Prop = value; } }");
		}
	}
}
