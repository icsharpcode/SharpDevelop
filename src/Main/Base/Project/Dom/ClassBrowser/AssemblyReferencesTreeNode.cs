// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// "References" tree node.
	/// </summary>
	public class AssemblyReferencesTreeNode : ModelCollectionTreeNode
	{
		private IAssemblyModel assemblyModel;
		private string text;
		private SimpleModelCollection<IAssemblyModel> references;
		
		public AssemblyReferencesTreeNode(IAssemblyModel assemblyModel)
		{
			if (assemblyModel == null)
				throw new ArgumentNullException("assemblyModel");
			this.assemblyModel = assemblyModel;
			this.text = SD.ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText");
			references = new SimpleModelCollection<IAssemblyModel>();
			UpdateReferences();
		}
		
		protected override IModelCollection<object> ModelChildren
		{
			get {
				return references;
			}
		}
		
		protected override System.Collections.Generic.IComparer<ICSharpCode.TreeView.SharpTreeNode> NodeComparer
		{
			get {
				return NodeTextComparer;
			}
		}
		
		public override object Text
		{
			get {
				return text;
			}
		}
		
		public override object Icon
		{
			get {
				return SD.ResourceService.GetImageSource("ProjectBrowser.ReferenceFolder.Closed");
			}
		}
		
		void UpdateReferences()
		{
			references.Clear();
			var assemblyParserService = SD.GetRequiredService<IAssemblyParserService>();
			if (assemblyModel.References != null) {
				foreach (var referencedAssemblyName in assemblyModel.References) {
					DefaultAssemblySearcher searcher = new DefaultAssemblySearcher(assemblyModel.Location);
					var resolvedFile = searcher.FindAssembly(referencedAssemblyName);
					var referenceAssemblyModel = assemblyParserService.GetAssemblyModel(resolvedFile);
					references.Add(referenceAssemblyModel);
				}
			}
		}
	}
}
