// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// A reference to a class or class member.
	/// </summary>
	public class Reference
	{
		DomRegion region;
		ResolveResult resolveResult;
		
		public Reference(DomRegion region, ResolveResult resolveResult)
		{
			if (region.IsEmpty)
				throw new ArgumentException("Region must not be empty");
			if (resolveResult == null)
				throw new ArgumentNullException("resolveResult");
			this.region = region;
			this.resolveResult = resolveResult;
		}
		
		public FileName FileName {
			get { return FileName.Create(region.FileName); }
		}
		
		public DomRegion Region {
			get { return region; }
		}
		
		public ResolveResult ResolveResult {
			get { return resolveResult; }
		}
	}
}
