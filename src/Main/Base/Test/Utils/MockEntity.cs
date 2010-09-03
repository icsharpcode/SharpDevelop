// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockEntity : AbstractEntity
	{
		public MockEntity() : base(null)
		{
		}
		
		public override string DocumentationTag {
			get {
				return String.Empty;
			}
		}
		
		public override ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override EntityType EntityType {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
