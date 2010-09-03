// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	class Wrapper<T> where T : IEntity
	{
		public T Entity { get; set; }
		public bool IsChecked { get; set; }
		
		public object Create(Action<Wrapper<T>> checkChange)
		{
			CheckBox box = new CheckBox() {
				Content = Entity.ProjectContent.Language.GetAmbience().Convert(Entity)
			};
			
			box.Checked += delegate {
				this.IsChecked = true;
				if (checkChange != null)
					checkChange(this);
			};
			box.Unchecked += delegate {
				this.IsChecked = false;
				if (checkChange != null)
					checkChange(this);
			};
			
			return box;
		}
	}
}
