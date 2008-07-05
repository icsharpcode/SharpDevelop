${StandardHeader.C#}
using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;


namespace ${StandardNamespace}
{
	/// <summary>
	/// ConfigurationSection with PlaneDisaster settings.
	/// </summary>
	public sealed class RecentFilesCollection : ConfigurationElementCollection
	{
		#region Properties

		/// <summary>
		/// Gets the CollectionType of the ConfigurationElementCollection.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}
	   

		/// <summary>
		/// Gets the Name of Elements of the collection.
		/// </summary>
		protected override string ElementName
		{
		get { return "PlaneDisaster"; }
		}
			   
	   
		/// <summary>
		/// Retrieve and item in the collection by index.
		/// </summary>
		public RecentFileElement this[int index]
		{
			get   { return (RecentFileElement)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}
		
		
		/// <summary>
		/// The maximum number of RecentFileElements to store
		/// </summary>
		[ConfigurationProperty("maxCount", IsKey=true, DefaultValue=(short)5)]
		public short MaxCount {
			get { return (short)this["maxCount"]; }
			set { this["maxCount"] = value; }
		}


		#endregion

		/// <summary>
		/// Adds a PlaneDisasterElement to the configuration file.
		/// </summary>
		public void Add(RecentFileElement element)
		{
			RecentFileElement [] NewFiles = new RecentFileElement [this.Count];
			short FileCount = this.MaxCount;
			
			this.CopyTo(NewFiles, 0);
			this.Clear();
			int i = 1;
			this.BaseAdd(element);
			foreach (RecentFileElement curFile in NewFiles) {
				if (curFile.Name != element.Name) {
					this.BaseAdd(curFile);
					i++;
					if (i >= FileCount) break;
				}
			}
		}
		
		
		/// <summary>
		/// Adds a PlaneDisasterElement to the configuration file.
		/// </summary>
		public void Add(string FileName)
		{
			Add (new RecentFileElement(Path.GetFullPath(FileName)));
		}
		
		
		private void AddRecentFileToMenu 
			(string FileName, 
			 ToolStripDropDownItem  oToolStripItem,
			 EventHandler menu_Click)
		{
			oToolStripItem.Enabled = true;
			ToolStripMenuItem RecentFileMenu = new ToolStripMenuItem(FileName);
			RecentFileMenu.Click  += menu_Click;
			oToolStripItem.DropDownItems.Add
				(RecentFileMenu);
		}
		
		/// <summary>
		/// Adds a group of to the configuration file.
		/// </summary>
		public void AddRange(string [] Files) {
			foreach (string File in Files) {
				RecentFileElement RecentFile = 
					new RecentFileElement(File);
				this.Add(RecentFile);
			}
		}
		
		
		/// <summary>
		/// Clears all PlaneDisasterElements to the collection.
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}
	   
	   
		/// <summary>
		/// Creates a new PlaneDisasterElement.
		/// </summary>
		/// <returns>A new <c>PlaneDisasterElement</c></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new RecentFileElement();
		}
			   
	   
		/// <summary>
		/// Checks for the existance of a given file in the
		/// <c>RecentFilesCollection</c>.
		/// </summary>
		/// <param name="FileName">The name of the file.</param>
		/// <returns>True if the file exists. False otherwise.</returns>
		public  bool FileExists(string FileName)
		{
			FileName = Path.GetFullPath(FileName);
			foreach (RecentFileElement File in this) {
				if (File.Name == FileName) return true;
			} return false;
			
		}
		
		/// <summary>
		/// Generates a group of drop <c>ToolStipItem(s)</c>
		/// under the given <c>ToolStripDropDownItem</c>
		/// The <c>EventHandler</c> menu_Click is assign
		/// to each <c>ToolStripDropDownItem.Click</c> event.
		/// </summary>
		/// <remarks>
		/// <example>
		/// The developer can assume that <c>ToolStripDropDownItem.Text</c>
		/// is the name of the file. It is assumed that the developer would do
		/// something similar to the code below.
		/// <code>
		/// ToolStripDropDownItem menuItem = (ToolStripDropDownItem) sender;
		/// string FileName = menuItem.Text;
		/// 
		/// //Open the file
		/// </code>
		/// </example>
		/// This function will be marked <c>[Obsolete]</c> in the event that 
		/// changes to it are made. I might assign the file name to
		/// <c>ToolStripDropDownItem.Tag</c>
		/// </remarks>
		/// <param name="menuParent">
		/// Parent <c>ToolStripDropDownItem</c>
		/// </param>
		/// <param name="menu_Click">
		/// <c>EventHandler</c> for Click Events
		/// </param>
		public void GenerateOpenRecentMenu
			(ToolStripDropDownItem menuParent, EventHandler menu_Click) {
			menuParent.DropDownItems.Clear();
			foreach (RecentFileElement RecentFile in this) {
				AddRecentFileToMenu
					(RecentFile.Name, menuParent, menu_Click);
			}
		}
		
		
		/// <summary>
		/// Gets the key of an element based on it's Id.
		/// </summary>
		/// <param name="element">Element to get the key of.</param>
		/// <returns>The key of <c>element</c>.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((RecentFileElement)element).Name;
		}
	   
	   
		/// <summary>
		/// Removes a PlaneDisasterElement with the given name.
		/// </summary>
		/// <param name="name">The name of the PlaneDisasterElement to remove.</param>
		public void Remove (string name) {
			base.BaseRemove(name);
		}

	}
}

