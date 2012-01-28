// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media.Imaging;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class EventNode : INode
	{
		public IEvent Event { get; private set; }
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDependency Dependency {
			get {
				throw new NotImplementedException();
			}
		}
		
		public BitmapSource Icon {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string GetInfo()
		{
			throw new NotImplementedException();
		}
		
		public Relationship GetRelationship(INode node)
		{
			throw new NotImplementedException();
		}
	}
}
