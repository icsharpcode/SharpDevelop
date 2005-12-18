// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is interface allows to create Memento that can create view
	/// from the from memento
	/// </summary>
	public interface IViewContentMementoCreator
	{
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		IViewContentMemento CreateViewContentMemento();		
	}

	public interface IViewContentMemento
	{
		/// <summary>
		/// Sets the state to the given memento and create the view
		/// </summary>
		IViewContent SetViewContentMemento(IViewContentMemento memento);
	}
}
