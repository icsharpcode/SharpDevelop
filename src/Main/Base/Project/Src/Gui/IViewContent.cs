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
	public delegate void SaveEventHandler(object sender, SaveEventArgs e);
	
	public class SaveEventArgs : System.EventArgs
	{
		bool successful;
		
		public bool Successful {
			get {
				return successful;
			}
		}
		
		public SaveEventArgs(bool successful)
		{
			this.successful = successful;
		}
	}
	/// <summary>
	/// IViewContent is the base interface for all editable data
	/// inside SharpDevelop.
	/// </summary>
	public interface IViewContent : IBaseViewContent
	{
		/// <summary>
		/// A generic name for the file, when it does have no file name
		/// (e.g. newly created files)
		/// </summary>
		string UntitledName {
			get;
			set;
		}
		
		/// <summary>
		/// This is the whole name of the content, e.g. the file name or
		/// the url depending on the type of the content.
		/// </summary>
		/// <returns>
		/// Title Name, if not set it returns UntitledName
		/// </returns>
		string TitleName {
			get;
			set;
		}
		
		/// <summary>
		/// Returns the file name (if any) assigned to this view.
		/// </summary>
		string FileName {
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the view is untitled.
		/// </summary>
		/// <returns>
		/// True, if TitleName not set.
		/// </returns>
		bool IsUntitled {
			get;
		}
		
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty {
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the content could not be altered.
		/// </summary>
		bool IsReadOnly {
			get;
		}
		
		/// <summary>
		/// If this property returns true the content can't be written.
		/// </summary>
		bool IsViewOnly {
			get;
		}
		
		/// <summary>
		/// If this property is true, content will be created in the tab page
		/// </summary>
		bool CreateAsSubViewContent {
			get;
		}

		/// <summary>
		/// Saves this content to the last load/save location.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Saves the content to the location <code>fileName</code>
		/// </summary>
		void Save(string fileName);
		
		/// <summary>
		/// Loads the content from the location <code>fileName</code>
		/// </summary>
		void Load(string fileName);
		
		/// <summary>
		/// Is called each time the name for the content has changed.
		/// </summary>
		event EventHandler TitleNameChanged;
		
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler DirtyChanged;
		
		event EventHandler     Saving;
		event SaveEventHandler Saved;
	}
}
