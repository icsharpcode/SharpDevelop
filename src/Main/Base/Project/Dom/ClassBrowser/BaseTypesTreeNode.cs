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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Represents the "Base types" sub-node of type nodes in ClassBrowser tree.
	/// </summary>
	public class BaseTypesTreeNode : SharpTreeNode
	{
		readonly ITypeDefinitionModel definition;
		string text;
		
		public BaseTypesTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.text = SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.BaseTypes");
			LazyLoading = true;
		}
		
		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			definition.Updated += OnDefinitionUpdated;
		}
		
		void OnDefinitionUpdated(object sender, EventArgs e)
		{
			// If the list of children was created, update it
			if (!LazyLoading)
				LoadChildren();
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			ITypeDefinition currentTypeDef = definition.Resolve();
			if (currentTypeDef != null) {
				foreach (var baseType in currentTypeDef.DirectBaseTypes) {
					ITypeDefinition baseTypeDef = baseType.GetDefinition();
					if (baseTypeDef != null) {
						ITypeDefinitionModel baseTypeModel = baseTypeDef.GetModel();
						if (baseTypeModel != null)
							Children.Add(SD.TreeNodeFactory.CreateTreeNode(baseTypeModel));
					}
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
