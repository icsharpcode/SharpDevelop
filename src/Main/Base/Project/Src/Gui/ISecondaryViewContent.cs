// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The base interface for secondary view contents
	/// (designer, doc viewer etc.)
	/// </summary>
	public interface ISecondaryViewContent : IBaseViewContent
	{
		/// <summary>
		/// Is called before the save operation of the main IViewContent
		/// </summary>
		void NotifyBeforeSave();
		
		void NotifyAfterSave(bool successful);
	}
}
