// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{
	// The top panel of the app, shows any license information on the
	// right, and a line of status text on the top

	internal class StatusPanel : Panel
	{
		protected static String     _host = Constants.NOGOOP_URL;
		protected static String     _file = "/download.html";
		protected static Label              _statusTextLabel;
		protected static String             _statusText;
		protected static StatusPanel        _statusPanel;

		internal static String StatusText {
			set {
				_statusText = value;
				if (_statusTextLabel != null)
					_statusTextLabel.Text = value;
			}
		}
		
		internal StatusPanel()
		{
			_statusPanel = this;
			ComputeControls();
		}
		
		internal static void Clear()
		{
			// Sometimes the status panel might not exist
			if (_statusPanel != null) {
				_statusTextLabel.Text = String.Empty;
				_statusPanel.Update();
			}
		}


		// Figure out what to say based on the license
		protected static void ComputeControls()
		{
			_statusPanel.BorderStyle = BorderStyle.Fixed3D;

			_statusPanel.Controls.Clear();

			_statusTextLabel = new Label();
			_statusTextLabel.Width = 300;
			_statusTextLabel.Dock = DockStyle.Left;
			_statusTextLabel.Text = _statusText;
			//_statusTextLabel.AutoSize = true;
			_statusTextLabel.Font = new Font(_statusTextLabel.Font, FontStyle.Bold);
			_statusPanel.Controls.Add(_statusTextLabel);

			/*if (license.ShowLicense)
			{

				Label l = new Label();
				l.Dock = DockStyle.Right;
				l.Text = license.DisplayName;
				l.Width = 110;
				_statusPanel._licenseWidth += l.Width;
				//l.AutoSize = true;
				_statusPanel.Controls.Add(l);

				if (license.RegisterOption)
				{
					Button b = Utils.MakeButton("Register");
					b.Dock = DockStyle.Right;
					b.Click += new EventHandler(_statusPanel.RegisterClick);
					b.Width = 55;
					_statusPanel._licenseWidth += b.Width;
					_statusPanel.Controls.Add(b);
				}

				if (license.PurchaseOption)
				{
					Button b = Utils.MakeButton("Purchase");
					b.Dock = DockStyle.Right;
					b.Click += new EventHandler(_statusPanel.PurchaseClick);
					b.Width = 60;
					_statusPanel._licenseWidth += b.Width;
					_statusPanel.Controls.Add(b);
					_purchaseUrl = license.PurchaseUrl;
				}
			}*/

			// This is high enough for two lines of label
			_statusPanel.Height = 30;
			_statusPanel.Layout +=  new LayoutEventHandler(_statusPanel.LayoutEvent);
		}

		protected void LayoutEvent(Object sender, LayoutEventArgs e)
		{
			_statusTextLabel.Width = Width - 3;
		}

		/*protected void RegisterClick(Object sender, EventArgs e)
		{
			NogoopLicense license;

			license = NetLicenseProvider.GetLicense
				(ObjectBrowser.ObjBrowser, 
				 NetLicenseProvider.ALLOW_EXCEPTIONS,
				 NetLicenseProvider.IGNORE_WIRED);

			// Validate the license (make sure its not expired, etc)
			if (license != null)
			{
				NetLicenseProvider.ValidateLicense(ObjectBrowser.ObjBrowser,
												   license,
												   NetLicenseProvider.
												   REPORT_ERROR);
				ObjectBrowser.License = license;
				ComputeControls(license);
			}
		}

		protected void PurchaseClick(Object sender, EventArgs e)
		{
			Utils.InvokeBrowser(new Uri(_purchaseUrl));
		}*/
	}
}
