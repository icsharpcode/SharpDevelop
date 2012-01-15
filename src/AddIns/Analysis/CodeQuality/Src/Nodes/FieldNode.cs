// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class FieldNode : INode
	{
		public IField Field { get; private set; }
		public TypeNode DeclaringType { get; private set; }
		public TypeNode ReturnType { get; private set; }
		
		public string Name {
			get { return Field.Name; }
		}
		
		public FieldNode(IField field, TypeNode declaringType, TypeNode returnType)
		{
			this.Field = field;
			this.DeclaringType = declaringType;
			this.ReturnType = returnType;
		}
		
		public IDependency Dependency {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public BitmapSource Icon {
			get {
				return NodeIconService.GetIcon(this);
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
		
		public IEnumerable<TypeNode> GetAllTypes()
		{
			throw new NotImplementedException();
		}
	}
}
