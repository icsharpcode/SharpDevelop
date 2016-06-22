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

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base interface for list of assembly references.
	/// </summary>
	public interface IAssemblyReferencesModel
	{
		IModelCollection<IAssemblyReferenceModel> AssemblyNames { get; }
		IAssemblyModel ParentAssemblyModel { get; }
	}
	
	/// <summary>
	/// Implements an empty assembly references model.
	/// </summary>
	public class EmptyAssemblyReferencesModel : IAssemblyReferencesModel
	{
		public static readonly IAssemblyReferencesModel Instance = new EmptyAssemblyReferencesModel();

		private static readonly SimpleModelCollection<IAssemblyReferenceModel> EmptyReferenceCollection =
			new SimpleModelCollection<IAssemblyReferenceModel>();
		
		public IModelCollection<IAssemblyReferenceModel> AssemblyNames {
			get {
				return EmptyReferenceCollection;
			}
		}
		
		public IAssemblyModel ParentAssemblyModel
		{
			get {
				return EmptyAssemblyModel.Instance;
			}
		}
	}
}
