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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Represents the "Derived types" sub-node of type nodes in ClassBrowser tree.
	/// </summary>
	public class DerivedTypesTreeNode : SharpTreeNode
	{
		readonly ITypeDefinitionModel definition;
		string text;
		
		public DerivedTypesTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.text = SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.DerivedTypes");
		}

		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			if (IsVisible) {
				definition.Updated += OnDefinitionUpdated;
			} else {
				definition.Updated -= OnDefinitionUpdated;
				LazyLoading = true;
			}
		}
		
		void OnDefinitionUpdated(object sender, EventArgs e)
		{
			// Listing the derived types can be expensive; so it's better to switch back to lazy-loading
			// (collapsing the node if necessary)
			LazyLoading = true;
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			ITypeDefinition currentTypeDef = definition.Resolve();
			if (currentTypeDef != null) {
				foreach (var derivedType in FindReferenceService.FindDerivedTypes(currentTypeDef, true)) {
					ITypeDefinitionModel derivedTypeModel = derivedType.GetModel();
					if (derivedTypeModel != null)
						Children.Add(SD.TreeNodeFactory.CreateTreeNode(derivedTypeModel));
				}
			}
		}
		
		public override object Text {
			get {
				return text;
			}
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("Icons.16x16.OpenFolderBitmap");
			}
		}
	}
}
