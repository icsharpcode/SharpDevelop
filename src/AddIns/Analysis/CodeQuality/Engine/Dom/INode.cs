// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	/// <summary>
	/// Description of INode.
	/// </summary>
	public interface INode
	{
		string Name { get; }
		IEnumerable<INode> Children { get; }
		IEnumerable<INode> Uses { get; }
		IEnumerable<INode> UsedBy { get; }
	}
	
	public class AssemblyNode : INode
	{
		public IAssembly AssemblyInfo { get; private set; }
		
		public string Name {
			get { return AssemblyInfo.AssemblyName; }
		}
		
		public IEnumerable<INode> Children {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<INode> Uses {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<INode> UsedBy {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
