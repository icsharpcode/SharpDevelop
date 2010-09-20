// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AddWebReferenceDialog : System.Windows.Forms.Form
	{
		WebServiceDiscoveryClientProtocol discoveryClientProtocol;
		CredentialCache credentialCache = new CredentialCache();
		string namespacePrefix = String.Empty;
		Uri discoveryUri;
		IProject project;
		WebReference webReference;
		
		delegate DiscoveryDocument DiscoverAnyAsync(string url);
		delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);
		delegate void AuthenticationHandler(Uri uri, string authenticationType);

		public AddWebReferenceDialog(IProject project)
		{
			InitializeComponent();
			AddMruList();
			AddImages();
			AddStringResources();
			// fixes forum-16247: Add Web Reference dialog missing URL on 120 DPI
			AddWebReferenceDialogResize(null, null);
			this.project = project;
		}
		
		/// <summary>
		/// The prefix that will be added to the web service's namespace
		/// (typically the project's namespace).
		/// </summary>
		public string NamespacePrefix {
			get {
				return namespacePrefix;
			}
			set {
				namespacePrefix = value;
			}
		}
		
		/// <summary>
		/// The discovered web reference to add to the project.
		/// </summary>
		public WebReference WebReference {
			get {
				return webReference;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.backButton = new System.Windows.Forms.ToolStripButton();
			this.forwardButton = new System.Windows.Forms.ToolStripButton();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.stopButton = new System.Windows.Forms.ToolStripButton();
			this.urlComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.goButton = new System.Windows.Forms.ToolStripButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.webBrowserTabPage = new System.Windows.Forms.TabPage();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.webServicesTabPage = new System.Windows.Forms.TabPage();
			this.referenceNameLabel = new System.Windows.Forms.Label();
			this.referenceNameTextBox = new System.Windows.Forms.TextBox();
			this.addButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.namespaceTextBox = new System.Windows.Forms.TextBox();
			this.namespaceLabel = new System.Windows.Forms.Label();
			this.webServicesView = new ICSharpCode.SharpDevelop.Gui.WebServicesView();
			this.toolStrip.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.webBrowserTabPage.SuspendLayout();
			this.webServicesTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStrip.CanOverflow = false;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.backButton,
									this.forwardButton,
									this.refreshButton,
									this.stopButton,
									this.urlComboBox,
									this.goButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(515, 25);
			this.toolStrip.Stretch = true;
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip";
			this.toolStrip.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ToolStripPreviewKeyDown);
			this.toolStrip.Enter += new System.EventHandler(this.ToolStripEnter);
			this.toolStrip.Leave += new System.EventHandler(this.ToolStripLeave);
			// 
			// backButton
			// 
			this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(23, 22);
			this.backButton.Text = "Back";
			this.backButton.Enabled = false;
			this.backButton.Click += new System.EventHandler(this.BackButtonClick);
			// 
			// forwardButton
			// 
			this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new System.Drawing.Size(23, 22);
			this.forwardButton.Text = "forward";
			this.forwardButton.Enabled = false;
			this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(23, 22);
			this.refreshButton.Text = "Refresh";
			this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
			// 
			// stopButton
			// 
			this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(23, 22);
			this.stopButton.Text = "Stop";
			this.stopButton.ToolTipText = "Stop";
			this.stopButton.Enabled = false;
			this.stopButton.Click += new System.EventHandler(this.StopButtonClick);
			// 
			// urlComboBox
			// 
			this.urlComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.urlComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.urlComboBox.AutoSize = false;
			this.urlComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
			this.urlComboBox.Name = "urlComboBox";
			this.urlComboBox.Size = new System.Drawing.Size(361, 21);
			this.urlComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UrlComboBoxKeyDown);
			this.urlComboBox.SelectedIndexChanged += new System.EventHandler(this.UrlComboBoxSelectedIndexChanged);
			// 
			// goButton
			// 
			this.goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(24, 22);
			this.goButton.Text = "Go";
			this.goButton.Click += new System.EventHandler(this.GoButtonClick);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.webBrowserTabPage);
			this.tabControl.Controls.Add(this.webServicesTabPage);
			this.tabControl.Location = new System.Drawing.Point(0, 25);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(515, 245);
			this.tabControl.TabIndex = 1;
			// 
			// webBrowserTabPage
			// 
			this.webBrowserTabPage.Controls.Add(this.webBrowser);
			this.webBrowserTabPage.Location = new System.Drawing.Point(4, 22);
			this.webBrowserTabPage.Name = "webBrowserTabPage";
			this.webBrowserTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.webBrowserTabPage.Size = new System.Drawing.Size(507, 219);
			this.webBrowserTabPage.TabIndex = 0;
			this.webBrowserTabPage.Text = "WSDL";
			this.webBrowserTabPage.UseVisualStyleBackColor = true;
			// 
			// webBrowser
			// 
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Location = new System.Drawing.Point(3, 3);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(501, 213);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.TabStop = false;
			this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
			this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
			this.webBrowser.CanGoBackChanged += new System.EventHandler(this.WebBrowserCanGoBackChanged);
			this.webBrowser.CanGoForwardChanged += new System.EventHandler(this.WebBrowserCanGoForwardChanged);
			// 
			// webServicesTabPage
			// 
			this.webServicesTabPage.Controls.Add(this.webServicesView);
			this.webServicesTabPage.Location = new System.Drawing.Point(4, 22);
			this.webServicesTabPage.Name = "webServicesTabPage";
			this.webServicesTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.webServicesTabPage.Size = new System.Drawing.Size(507, 219);
			this.webServicesTabPage.TabIndex = 1;
			this.webServicesTabPage.Text = "Available Web Services";
			this.webServicesTabPage.UseVisualStyleBackColor = true;
			// 
			// webServicesView
			// 
			this.webServicesView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webServicesView.Location = new System.Drawing.Point(3, 3);
			this.webServicesView.Name = "webServicesView";
			this.webServicesView.Size = new System.Drawing.Size(501, 213);
			// 
			// referenceNameLabel
			// 
			this.referenceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.referenceNameLabel.Location = new System.Drawing.Point(9, 280);
			this.referenceNameLabel.Name = "referenceNameLabel";
			this.referenceNameLabel.Size = new System.Drawing.Size(128, 20);
			this.referenceNameLabel.TabIndex = 2;
			this.referenceNameLabel.Text = "&Reference Name:";
			this.referenceNameLabel.UseCompatibleTextRendering = true;
			// 
			// referenceNameTextBox
			// 
			this.referenceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.referenceNameTextBox.Location = new System.Drawing.Point(127, 281);
			this.referenceNameTextBox.Name = "referenceNameTextBox";
			this.referenceNameTextBox.Size = new System.Drawing.Size(305, 21);
			this.referenceNameTextBox.TabIndex = 4;
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Enabled = false;
			this.addButton.Location = new System.Drawing.Point(438, 281);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(73, 21);
			this.addButton.TabIndex = 6;
			this.addButton.Text = "&Add";
			this.addButton.UseCompatibleTextRendering = true;
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(438, 303);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(73, 21);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// namespaceTextBox
			// 
			this.namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.namespaceTextBox.Location = new System.Drawing.Point(127, 303);
			this.namespaceTextBox.Name = "namespaceTextBox";
			this.namespaceTextBox.Size = new System.Drawing.Size(305, 21);
			this.namespaceTextBox.TabIndex = 5;
			// 
			// namespaceLabel
			// 
			this.namespaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.namespaceLabel.Location = new System.Drawing.Point(9, 302);
			this.namespaceLabel.Name = "namespaceLabel";
			this.namespaceLabel.Size = new System.Drawing.Size(128, 20);
			this.namespaceLabel.TabIndex = 3;
			this.namespaceLabel.Text = "&Namespace:";
			this.namespaceLabel.UseCompatibleTextRendering = true;
			// 
			// AddWebReferenceDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(515, 336);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.namespaceTextBox);
			this.Controls.Add(this.namespaceLabel);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.referenceNameTextBox);
			this.Controls.Add(this.referenceNameLabel);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.toolStrip);
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "AddWebReferenceDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Web Reference";
			this.Resize += new System.EventHandler(this.AddWebReferenceDialogResize);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddWebReferenceDialogFormClosing);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.webBrowserTabPage.ResumeLayout(false);
			this.webServicesTabPage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label namespaceLabel;
		private System.Windows.Forms.TextBox namespaceTextBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.TextBox referenceNameTextBox;
		private System.Windows.Forms.Label referenceNameLabel;
		private System.Windows.Forms.TabPage webBrowserTabPage;
		private System.Windows.Forms.TabPage webServicesTabPage;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.WebBrowser webBrowser;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.ToolStripButton goButton;
		private System.Windows.Forms.ToolStripComboBox urlComboBox;
		private System.Windows.Forms.ToolStripButton stopButton;
		private System.Windows.Forms.ToolStripButton refreshButton;
		private System.Windows.Forms.ToolStripButton forwardButton;
		private System.Windows.Forms.ToolStripButton backButton;
		private ICSharpCode.SharpDevelop.Gui.WebServicesView webServicesView;
		#endregion
		
		void AddMruList()
		{
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				if (key != null) {
					foreach (string name in key.GetValueNames()) {
						urlComboBox.Items.Add((string)key.GetValue(name));
					}
				}
			} catch (Exception) { };
		}
		
		/// <summary>
		/// If the user presses the tab key, and the currently selected toolstrip
		/// item is at the end or the beginning of the toolstip, then force the
		/// tab to move to another control instead of staying on the toolstrip.
		/// </summary>
		void ToolStripPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Tab) {
				if (goButton.Selected && e.Modifiers != Keys.Shift) {
					toolStrip.TabStop = true;
				} else if (backButton.Selected && e.Modifiers == Keys.Shift) {
					toolStrip.TabStop = true;
				}
			}
		}
		
		void ToolStripEnter(object sender, EventArgs e)
		{
			toolStrip.TabStop = false;
		}
		
		void ToolStripLeave(object sender, EventArgs e)
		{
			toolStrip.TabStop = true;
		}
		
		void BackButtonClick(object sender, EventArgs e)
		{
			try {
				webBrowser.GoBack();
			} catch (Exception) { }
		}
		
		void ForwardButtonClick(object sender, System.EventArgs e)
		{
			try {
				webBrowser.GoForward();
			} catch (Exception) { }
		}
		
		void StopButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Stop();
			StopDiscovery();
			addButton.Enabled = false;
		}
		
		void RefreshButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Refresh();
		}
		
		void GoButtonClick(object sender, System.EventArgs e)
		{
			BrowseUrl(urlComboBox.Text);
		}
		
		void BrowseUrl(string url)
		{
			webBrowser.Focus();
			webBrowser.Navigate(url);
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			stopButton.Enabled = true;
			webServicesView.Clear();
		}
		
		void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			Cursor = Cursors.Default;
			stopButton.Enabled = false;
			urlComboBox.Text = webBrowser.Url.ToString();
			StartDiscovery(e.Url);
		}
		
		void WebBrowserCanGoForwardChanged(object sender, EventArgs e)
		{
			forwardButton.Enabled = webBrowser.CanGoForward;
		}
		
		void WebBrowserCanGoBackChanged(object sender, EventArgs e)
		{
			backButton.Enabled = webBrowser.CanGoBack;
		}
		
		/// <summary>
		/// Gets the namespace to be used with the generated web reference code.
		/// </summary>
		string GetDefaultNamespace()
		{
			if (namespacePrefix.Length > 0 && discoveryUri != null) {
				return String.Concat(namespacePrefix, ".", discoveryUri.Host);
			} else if (discoveryUri != null) {
				return discoveryUri.Host;
			}
			return String.Empty;
		}
		
		string GetReferenceName()
		{
			if (discoveryUri != null) {
				return discoveryUri.Host;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Starts the search for web services at the specified url.
		/// </summary>
		void StartDiscovery(Uri uri)
		{
			StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, DiscoveryNetworkCredential.DefaultAuthenticationType));
		}
		
		void StartDiscovery(Uri uri, DiscoveryNetworkCredential credential)
		{
			// Abort previous discovery.
			StopDiscovery();
			
			// Start new discovery.
			discoveryUri = uri;
			DiscoverAnyAsync asyncDelegate = new DiscoverAnyAsync(discoveryClientProtocol.DiscoverAny);
			AsyncCallback callback = new AsyncCallback(DiscoveryCompleted);
			discoveryClientProtocol.Credentials = credential;
			IAsyncResult result = asyncDelegate.BeginInvoke(uri.AbsoluteUri, callback, new AsyncDiscoveryState(discoveryClientProtocol, uri, credential));
		}
		
		/// <summary>
		/// Called after an asynchronous web services search has
		/// completed.
		/// </summary>
		void DiscoveryCompleted(IAsyncResult result)
		{
			AsyncDiscoveryState state = (AsyncDiscoveryState)result.AsyncState;
			WebServiceDiscoveryClientProtocol protocol = state.Protocol;
			
			// Check that we are still waiting for this particular callback.
			bool wanted = false;
			lock (this) {
				wanted = Object.ReferenceEquals(discoveryClientProtocol, protocol);
			}
			
			if (wanted) {
				DiscoveredWebServicesHandler handler = new DiscoveredWebServicesHandler(DiscoveredWebServices);
				try {
					DiscoverAnyAsync asyncDelegate = (DiscoverAnyAsync)((AsyncResult)result).AsyncDelegate;
					DiscoveryDocument doc = asyncDelegate.EndInvoke(result);
					if (!state.Credential.IsDefaultAuthenticationType) {
						AddCredential(state.Uri, state.Credential);
					}
					Invoke(handler, new object[] {protocol});
				} catch (Exception ex) {
					if (protocol.IsAuthenticationRequired) {
						HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
						AuthenticationHandler authHandler = new AuthenticationHandler(AuthenticateUser);
						Invoke(authHandler, new object[] {state.Uri, authHeader.AuthenticationType});
					} else {
						LoggingService.Error("DiscoveryCompleted", ex);
						Invoke(handler, new object[] {null});
					}
				}
			}
		}
		
		/// <summary>
		/// Stops any outstanding asynchronous discovery requests.
		/// </summary>
		void StopDiscovery()
		{
			lock (this) {
				if (discoveryClientProtocol != null) {
					try {
						discoveryClientProtocol.Abort();
					} catch (NotImplementedException) {
					} catch (ObjectDisposedException) {
						// Receive this error if the url pointed to a file.
						// The discovery client will already have closed the file
						// so the abort fails.
					}
					discoveryClientProtocol.Dispose();
				}
				discoveryClientProtocol = new WebServiceDiscoveryClientProtocol();
			}
		}

		void AddWebReferenceDialogFormClosing(object sender, FormClosingEventArgs e)
		{
			StopDiscovery();
		}
		
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			urlComboBox.Focus();
		}
		
		ServiceDescriptionCollection GetServiceDescriptions(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			protocol.ResolveOneLevel();
			
			foreach (DictionaryEntry entry in protocol.References) {
				ContractReference contractRef = entry.Value as ContractReference;
				if (contractRef != null) {
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}
		
		void DiscoveredWebServices(DiscoveryClientProtocol protocol)
		{
			if (protocol != null) {
				addButton.Enabled = true;
				namespaceTextBox.Text = GetDefaultNamespace();
				referenceNameTextBox.Text = GetReferenceName();
				webServicesView.Add(GetServiceDescriptions(protocol));
				webReference = new WebReference(project, discoveryUri.AbsoluteUri, referenceNameTextBox.Text, namespaceTextBox.Text, protocol);
			} else {
				webReference = null;
				addButton.Enabled = false;
				webServicesView.Clear();
			}
		}
		
		void UrlComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			BrowseUrl(urlComboBox.Text);
		}
		
		void UrlComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter && urlComboBox.Text.Length > 0) {
				BrowseUrl(urlComboBox.Text);
			}
		}
		
		void AddWebReferenceDialogResize(object sender, EventArgs e)
		{
			int width = toolStrip.ClientSize.Width;
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item != urlComboBox)
					width -= item.Width + 8;
			}
			urlComboBox.Width = width;
		}
		
		void AddButtonClick(object sender,EventArgs e)
		{
			try {
				if (!WebReference.IsValidReferenceName(referenceNameTextBox.Text)) {
					MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidReferenceNameError}"));
					return;
				}
				
				if (!WebReference.IsValidNamespace(namespaceTextBox.Text)) {
					MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidNamespaceError}"));
					return;
				}
				
				webReference.Name = referenceNameTextBox.Text;
				webReference.ProxyNamespace = namespaceTextBox.Text;
				
				DialogResult = DialogResult.OK;
				Close();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void AddImages()
		{
			goButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.RunProgramIcon");
			refreshButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserRefresh");
			backButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserBefore");
			forwardButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserAfter");
			stopButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserCancel");
			Icon = WinFormsResourceService.GetIcon("Icons.16x16.WebSearchIcon");
		}
		
		void AddStringResources()
		{
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DialogTitle}");
			
			refreshButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RefreshButtonTooltip}");
			refreshButton.ToolTipText = refreshButton.Text;
			
			backButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BackButtonTooltip}");
			backButton.ToolTipText = backButton.Text;
			
			forwardButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ForwardButtonTooltip}");
			forwardButton.ToolTipText = forwardButton.Text;

			referenceNameLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ReferenceNameLabel}");
			namespaceLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.NamespaceLabel}");

			goButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.GoButtonTooltip}");
			goButton.ToolTipText = goButton.Text;
			
			addButton.Text = StringParser.Parse("${res:Global.AddButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			
			stopButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.StopButtonTooltip}");
			stopButton.ToolTipText = stopButton.Text;
			
			webServicesTabPage.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.WebServicesTabPageTitle}");
			webServicesTabPage.ToolTipText = webServicesTabPage.Text;
		}
		
		void AuthenticateUser(Uri uri, string authenticationType)
		{
			DiscoveryNetworkCredential credential = (DiscoveryNetworkCredential)credentialCache.GetCredential(uri, authenticationType);
			if (credential != null) {
				StartDiscovery(uri, credential);
			} else {
				using (UserCredentialsDialog credentialsForm = new UserCredentialsDialog(uri.ToString(), authenticationType)) {
					if (DialogResult.OK == credentialsForm.ShowDialog(WorkbenchSingleton.MainWin32Window)) {
						StartDiscovery(uri, credentialsForm.Credential);
					}
				}
			}
		}
		
		void AddCredential(Uri uri, DiscoveryNetworkCredential credential)
		{
			NetworkCredential matchedCredential = credentialCache.GetCredential(uri, credential.AuthenticationType);
			if (matchedCredential != null) {
				credentialCache.Remove(uri, credential.AuthenticationType);
			}
			credentialCache.Add(uri, credential.AuthenticationType, credential);
		}
	}
}
