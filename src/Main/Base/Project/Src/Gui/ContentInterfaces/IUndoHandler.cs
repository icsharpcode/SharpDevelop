// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IUndoHandler
	{
		bool EnableUndo {
			get;
		}
		
		bool EnableRedo {
			get;
		}
		
		void Undo();
		void Redo();
	}
}
