// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Factory for model tree nodes.
	/// </summary>
	/// <remarks>
	/// The global instance <c>SD.TreeNodeFactory</c> is a composite of all factories
	/// registered in the AddIn-Tree path <c>/SharpDevelop/TreeNodeFactories</c>.
	/// </remarks>
	[SDService("SD.TreeNodeFactory")]
	public interface ITreeNodeFactory
	{
		/// <summary>
		/// Gets the derived-most type of the model that is directly supported by this factory.
		/// 
		/// For example, a factory that supports <c>ITest</c> objects and has
		/// no additional behavior specified to <c>ITestProject</c>, will
		/// return <c>typeof(ITest)</c> when passed an <c>ITestProject</c>.
		/// 
		/// If the model type is not supported by this factory, this method returns null.
		/// </summary>
		/// <example>
		/// A usual implementation of this method looks like this:
		/// <code>
		/// return model is MyModelType ? typeof(MyModelType) : null;
		/// </code>
		/// </example>
		Type GetSupportedType(object model);
		
		/// <summary>
		/// Creates a tree node for the specified model object.
		/// </summary>
		/// <remarks>
		/// This method returns null if and only if the model is not supported.
		/// </remarks>
		SharpTreeNode CreateTreeNode(object model);
	}
}
