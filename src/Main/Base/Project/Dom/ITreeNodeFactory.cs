// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
