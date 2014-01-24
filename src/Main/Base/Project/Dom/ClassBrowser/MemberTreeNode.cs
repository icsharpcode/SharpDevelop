// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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


