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
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class MemberTreeNode : ModelCollectionTreeNode
	{
		IMemberModel model;
		
		public MemberTreeNode(IMemberModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.model = model;
			this.model.PropertyChanged += OnModelPropertyChanged;
			// disable lazy loading to avoid showing a useless + sign in the tree.
			// remove this line if you add child nodes
			LazyLoading = false;
		}
		
		private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// Update properties of tree node, since member model has been changed
			cachedText = null;
			RaisePropertyChanged(null);
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		public override object Icon {
			get { return ClassBrowserIconService.GetIcon(model.UnresolvedMember).ImageSource; }
		}
		
		object cachedText;
		
		public override object Text {
			get {
				if (cachedText == null)
					cachedText = GetText();
				return cachedText;
			}
		}
		
		object GetText()
		{
			var member = model.Resolve();
			if (member == null)
				return model.Name;
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
			return ambience.ConvertSymbol(member);
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return ImmutableModelCollection<object>.Empty; }
		}
		
		protected override System.Collections.Generic.IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var target = model.Resolve();
			if (target != null)
				NavigationService.NavigateTo(target);
		}
		
		public override void ShowContextMenu()
		{
			var entityModel = this.Model as IEntityModel;
			if (entityModel != null) {
				var ctx = MenuService.ShowContextMenu(null, entityModel, "/SharpDevelop/EntityContextMenu");
			}
		}
	}
}
