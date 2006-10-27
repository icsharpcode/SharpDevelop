// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// A basic command interface. A command has simply an owner which "runs" the command
	/// and a Run method which invokes the command.
	/// </summary>
	public interface ICommand
	{
		
		/// <summary>
		/// Returns the owner of the command.
		/// </summary>
		object Owner {
			get;
			set;
		}
		
		/// <summary>
		/// Invokes the command.
		/// </summary>
		void Run();
		
		/// <summary>
		/// Is called when the Owner property is changed.
		/// </summary>
		event EventHandler OwnerChanged;
	}
}
