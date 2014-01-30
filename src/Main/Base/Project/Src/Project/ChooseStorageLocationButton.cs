// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Button that can be used to choose the location where a property is stored.
	/// </summary>
	public sealed class ChooseStorageLocationButton : Button
	{
		ToolStripMenuItem[] menuItems;
		
		public ChooseStorageLocationButton()
		{
			this.Size = new Size(20, 20);
			ContextMenuStrip = new ContextMenuStrip();
			menuItems = new ToolStripMenuItem[] {
				CreateMenuItem("${res:Dialog.ProjectOptions.ConfigurationSpecific}", PropertyStorageLocations.ConfigurationSpecific),
				CreateMenuItem("${res:Dialog.ProjectOptions.PlatformSpecific}", PropertyStorageLocations.PlatformSpecific),
				CreateMenuItem("${res:Dialog.ProjectOptions.StoreInUserFile}", PropertyStorageLocations.UserFile)
			};
			ContextMenuStrip.Items.AddRange(menuItems);
			
			ContextMenuStrip.Items.Add(new ToolStripSeparator());
			// TODO: Link to the SharpDevelop documentation and explain not only the location-thing, but also what the property clicked actually is.
			ContextMenuStrip.Items.Add(StringParser.Parse("${res:Global.HelpButtonText}"), null, delegate {
			                           	MessageService.ShowMessage("${res:Dialog.ProjectOptions.StorageLocationHelp}");
			                           });
		}
		
		ToolStripMenuItem CreateMenuItem(string text, PropertyStorageLocations location)
		{
			ToolStripMenuItem item = new ToolStripMenuItem(StringParser.Parse(text));
			item.CheckOnClick = true;
			item.CheckedChanged += delegate {
				if (item.Checked) {
					StorageLocation |= location;
				} else {
					StorageLocation &= ~location;
				}
			};
			return item;
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			ContextMenuStrip.Show(this, new Point(Width / 2, Height / 2));
		}
		
		PropertyStorageLocations storageLocation;
		public event EventHandler StorageLocationChanged;
		
		public PropertyStorageLocations StorageLocation {
			get {
				return storageLocation;
			}
			set {
				if ((value & PropertyStorageLocations.ConfigurationAndPlatformSpecific) != 0) {
					// remove 'Base' flag if any of the specific flags is set
					value &= ~PropertyStorageLocations.Base;
				} else {
					// otherwise, add 'Base' flag
					value |= PropertyStorageLocations.Base;
				}
				if (storageLocation != value) {
					storageLocation = value;
					Image oldImage = Image;
					Image = CreateImage(value);
					if (oldImage != null) {
						oldImage.Dispose();
					}
					menuItems[0].Checked = (value & PropertyStorageLocations.ConfigurationSpecific) == PropertyStorageLocations.ConfigurationSpecific;
					menuItems[1].Checked = (value & PropertyStorageLocations.PlatformSpecific) == PropertyStorageLocations.PlatformSpecific;
					menuItems[2].Checked = (value & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile;
					if (StorageLocationChanged != null) {
						StorageLocationChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				Image.Dispose();
				ContextMenuStrip.Dispose();
			}
		}
		
		public static Image CreateImage(PropertyStorageLocations location)
		{
			Bitmap bmp = new Bitmap(12, 12);
			using (Graphics g = Graphics.FromImage(bmp)) {
				g.Clear(Color.Transparent);
				Brush circleBrush;
				switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
					case PropertyStorageLocations.ConfigurationSpecific:
						circleBrush = Brushes.Blue;
						break;
					case PropertyStorageLocations.PlatformSpecific:
						circleBrush = Brushes.Red;
						break;
					case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
						circleBrush = Brushes.Violet;
						break;
					default:
						circleBrush = Brushes.Black;
						break;
				}
				if ((location & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile) {
					g.FillEllipse(circleBrush, 0, 0, 7, 7);
					DrawU(g, 7, 5);
				} else {
					g.FillEllipse(circleBrush, 2, 2, 8, 8);
				}
			}
			return bmp;
		}
		
		/// <summary>draws the letter 'U'</summary>
		static void DrawU(Graphics g, int x, int y)
		{
			const int width  = 4;
			const int height = 6;
			g.DrawLine(Pens.DarkGreen, x, y, x, y + height - 1);
			g.DrawLine(Pens.DarkGreen, x + width, y, x + width, y + height - 1);
			g.DrawLine(Pens.DarkGreen, x + 1, y + height, x + width - 1, y + height);
		}
	}
}
