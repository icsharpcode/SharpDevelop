// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Text;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// A container for the text editor options.
	/// </summary>
	[Serializable]
	public class TextEditorOptions : INotifyPropertyChanged
	{
		#region PropertyChanged handling
		/// <inheritdoc/>
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
		
		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		#endregion
		
		#region ShowSpaces / ShowTabs / ShowBoxForControlCharacters
		bool showSpaces;
		
		/// <summary>
		/// Gets/Sets whether to show · for spaces.
		/// </summary>
		/// <remarks>The default value is <c>false</c>.</remarks>
		[DefaultValue(false)]
		public virtual bool ShowSpaces {
			get { return showSpaces; }
			set {
				if (showSpaces != value) {
					showSpaces = value;
					OnPropertyChanged("ShowSpaces");
				}
			}
		}
		
		bool showTabs;
		
		/// <summary>
		/// Gets/Sets whether to show » for tabs.
		/// </summary>
		/// <remarks>The default value is <c>false</c>.</remarks>
		[DefaultValue(false)]
		public virtual bool ShowTabs {
			get { return showTabs; }
			set {
				if (showTabs != value) {
					showTabs = value;
					OnPropertyChanged("ShowTabs");
				}
			}
		}
		
		bool showBoxForControlCharacters = true;
		
		/// <summary>
		/// Gets/Sets whether to show a box with the hex code for control characters.
		/// </summary>
		/// <remarks>The default value is <c>true</c>.</remarks>
		[DefaultValue(true)]
		public virtual bool ShowBoxForControlCharacters {
			get { return showBoxForControlCharacters; }
			set {
				if (showBoxForControlCharacters != value) {
					showBoxForControlCharacters = value;
					OnPropertyChanged("ShowBoxForControlCharacters");
				}
			}
		}
		#endregion
		
		#region TabSize / IndentationSize / ConvertTabsToSpaces / GetIndentationString
		// I'm using '_' prefixes for the fields here to avoid confusion with the local variables
		// in the methods below.
		// The fields should be accessed only by their property - the fields might not be used
		// if someone overrides the property.
		
		int _indentationSize = 4;
		
		/// <summary>
		/// Gets/Sets the width of one indentation unit.
		/// </summary>
		/// <remarks>The default value is 4.</remarks>
		[DefaultValue(4)]
		public virtual int IndentationSize {
			get { return _indentationSize; }
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", value, "value must be positive");
				if (_indentationSize != value) {
					_indentationSize = value;
					OnPropertyChanged("IndentationSize");
				}
			}
		}
		
		bool _convertTabsToSpaces;
		
		/// <summary>
		/// Gets/Sets whether to use spaces for indentation instead of tabs.
		/// </summary>
		/// <remarks>The default value is <c>false</c>.</remarks>
		[DefaultValue(false)]
		public virtual bool ConvertTabsToSpaces {
			get { return _convertTabsToSpaces; }
			set {
				if (_convertTabsToSpaces != value) {
					_convertTabsToSpaces = value;
					OnPropertyChanged("ConvertTabsToSpaces");
				}
			}
		}
		
		/// <summary>
		/// Gets the text used for indentation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public string IndentationString {
			get { return GetIndentationString(0); }
		}
		
		/// <summary>
		/// Gets text required to indent from the specified <paramref name="column"/> to the next indentation level.
		/// </summary>
		public virtual string GetIndentationString(int column)
		{
			if (column < 0)
				throw new ArgumentOutOfRangeException("column", column, "Value must be non-negative.");
			int indentationSize = this.IndentationSize;
			if (ConvertTabsToSpaces) {
				return new string(' ', indentationSize - (column % indentationSize));
			} else {
				return "\t";
			}
		}
		#endregion
		
		bool cutCopyWholeLine = true;
		
		/// <summary>
		/// Gets/Sets whether copying without a selection copies the whole current line.
		/// </summary>
		public virtual bool CutCopyWholeLine {
			get { return cutCopyWholeLine; }
			set {
				cutCopyWholeLine = value;
				OnPropertyChanged("CutCopyWholeLine");
			}
		}
	}
}
