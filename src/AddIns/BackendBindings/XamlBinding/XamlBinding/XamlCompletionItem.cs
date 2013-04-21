// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlCompletionItem.
	/// </summary>
	class XamlCompletionItem : DefaultCompletionItem
	{
		public XamlCompletionItem(string text)
			: base(text)
		{
			this.entity = null;
			this.Image = ClassBrowserIconService.Namespace;
		}
		
		public XamlCompletionItem(IEntity entity)
			: base(entity.Name)
		{
			this.entity = entity;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		public XamlCompletionItem(string text, IEntity entity)
			: base(text)
		{
			this.entity = entity;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		IEntity entity;
		public IEntity Entity {
			get {
				return entity;
			}
		}
	}
}
