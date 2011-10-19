/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.10.2011
 * Time: 20:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Discovery;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;
using ICSharpCode.SharpDevelop.Widgets;

namespace Gui.Dialogs.ReferenceDialog
{
	/// <summary>
	/// Description of AddServiceReferenceViewModel.
	/// </summary>
	public class AddServiceReferenceViewModel:ViewModelBase
	{
		string header1 = "To see a list of available services on an specific Server, ";
		string header2 = "enter a service URL and click Go. To browse for available services click Discover";
		string noUrl = "Please enter the address of the Service.";
//		string discoverMenu ="Services in Solution";
		Uri discoveryUri;
		public AddServiceReferenceViewModel(IProject project)
		{
			Project = project;
			title =  "Add Service Reference";
			discoverButtonContend = "Disvover";
			HeadLine = header1 + header2;
			
			MruServices = AddMruList();
			SelectedService = MruServices[0];
			
			GoCommand = new RelayCommand(ExecuteGo,CanExecuteGo);
			DiscoverCommand = new RelayCommand(ExecuteDiscover,CanExecuteDiscover);
		}
		
		private string art;
		
		public string Art {
			get { return art; }
			set { art = value;
				base.RaisePropertyChanged(() =>Art);
			}
		}
		
		
		private string title;
		public string Title
		{
			get {return title;}
			set {title = value;
				base.RaisePropertyChanged(() =>Title);
			}
		}
		

		public string HeadLine {get; private set;}
		
		private string discoverButtonContend;
		
		public string DiscoverButtonContend {
			get { return discoverButtonContend; }
			set { discoverButtonContend = value;
			base.RaisePropertyChanged(() =>DiscoverButtonContend);}
		}

		
		private IProject project;
		
		public IProject Project
		{
			get {return project;}
			set {project = value;
				base.RaisePropertyChanged(() =>Project);
			}
		}
	
		#region Create List of services
		
		// Modifyed Code from Matt
		
		List <string>  AddMruList()
		{
			var list = new List<string>();
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				if (key != null) {
					foreach (string name in key.GetValueNames()) {
						list.Add ((string)key.GetValue(name));
					}
				}
			} catch (Exception)
			{
			};
			return list;
		}
		
		private List<string> mruServices;
		
		public List<string> MruServices {
			get {
				if (mruServices == null) {
					mruServices = new List<string>();
				}
				return mruServices; }
			set { mruServices = value;
				base.RaisePropertyChanged(() =>MruServices);
			}
		}
		
		private string selectedService;
		
		public string SelectedService {
			get { return selectedService; }
			set { selectedService = value;
				base.RaisePropertyChanged(() =>SelectedService);}
		}
		
		#endregion
		
		#region Go
		
		public System.Windows.Input.ICommand GoCommand {get; private set;}
		
		private void ExecuteGo ()
		{
			if (String.IsNullOrEmpty(SelectedService)) {
				MessageBox.Show (noUrl);
			}
			Uri uri = new Uri(SelectedService);
			StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, DiscoveryNetworkCredential.DefaultAuthenticationType));
		}
		
		private bool CanExecuteGo()
		{
			return true;
		}
		
		#endregion
		
		#region Discover
		
		public System.Windows.Input.ICommand DiscoverCommand {get;private set;}
		
		private bool CanExecuteDiscover ()
		{
			return true;
		}
		
		private void ExecuteDiscover ()
		{
			MessageBox.Show ("<Discover> is not implemented at the Moment");
		}
			
		#endregion
		
		
		#region discover service Code from Matt
		
			
		CredentialCache credentialCache = new CredentialCache();
		WebServiceDiscoveryClientProtocol discoveryClientProtocol;
		WebReference webReference;
		
		delegate DiscoveryDocument DiscoverAnyAsync(string url);
		delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);
		delegate void AuthenticationHandler(Uri uri, string authenticationType);	
		
			
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
//					Invoke(handler, new object[] {protocol});
				} catch (Exception ex) {
					if (protocol.IsAuthenticationRequired) {
						HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
						AuthenticationHandler authHandler = new AuthenticationHandler(AuthenticateUser);
//						Invoke(authHandler, new object[] {state.Uri, authHeader.AuthenticationType});
					} else {
						LoggingService.Error("DiscoveryCompleted", ex);
//						Invoke(handler, new object[] {null});
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
		
		
		void AuthenticateUser(Uri uri, string authenticationType)
		{
			DiscoveryNetworkCredential credential = (DiscoveryNetworkCredential)credentialCache.GetCredential(uri, authenticationType);
			if (credential != null) {
				StartDiscovery(uri, credential);
			} else {
				using (UserCredentialsDialog credentialsForm = new UserCredentialsDialog(uri.ToString(), authenticationType)) {
//					if (DialogResult.OK == credentialsForm.ShowDialog(WorkbenchSingleton.MainWin32Window)) {
//						StartDiscovery(uri, credentialsForm.Credential);
//					}
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
		
		void DiscoveredWebServices(DiscoveryClientProtocol protocol)
		{
			if (protocol != null) {
//				addButton.Enabled = true;
//				namespaceTextBox.Text = GetDefaultNamespace();
//				referenceNameTextBox.Text = GetReferenceName();
//				webServicesView.Add(GetServiceDescriptions(protocol));
//				webReference = new WebReference(project, discoveryUri.AbsoluteUri, referenceNameTextBox.Text, namespaceTextBox.Text, protocol);
			} else {
				webReference = null;
//				addButton.Enabled = false;
//				webServicesView.Clear();
			}
		}
		#endregion
	}
}
