/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.10.2011
 * Time: 20:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.Widgets.Resources;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	/// <summary>
	/// Description of AddServiceReferenceViewModel.
	/// </summary>
	public class AddServiceReferenceViewModel:ViewModelBase
	{
		string header1 = "To see a list of available services on a specific server, ";
		string header2 = "enter a service URL and click Go. To browse for available services click Discover.";
		string noUrl = "Please enter the address of the Service.";
		string title =  "Add Service Reference";
		string waitMessage = "Please wait....";
		string defaultNameSpace;
		string serviceDescriptionMessage;
		string namespacePrefix = String.Empty;
		
		private  ObservableCollection<ImageAndDescription> twoValues;
		
		private List<string> mruServices = new List<string>();
		private string selectedService;
		private IProject project;
		
		List<ServiceItem> items = new List <ServiceItem>();
		ServiceItem myItem;
		
		Uri discoveryUri;
		ServiceDescriptionCollection serviceDescriptionCollection = new ServiceDescriptionCollection();
		CredentialCache credentialCache = new CredentialCache();
		WebServiceDiscoveryClientProtocol discoveryClientProtocol;
		
		delegate DiscoveryDocument DiscoverAnyAsync(string url);
		delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);
		delegate void AuthenticationHandler(Uri uri, string authenticationType);	
		
		
		public AddServiceReferenceViewModel(IProject project)
		{
			project = project;
			discoverButtonContend = "Disvover";
			HeadLine = header1 + header2;
			
			MruServices = ServiceReferenceHelper.AddMruList();
			SelectedService = MruServices[0];
			
			GoCommand = new RelayCommand(ExecuteGo,CanExecuteGo);
			DiscoverCommand = new RelayCommand(ExecuteDiscover,CanExecuteDiscover);
			AdvancedDialogCommand = new RelayCommand(ExecuteAdvancedDialogCommand,CanExecuteAdvancedDialogCommand);
			TwoValues = new ObservableCollection<ImageAndDescription>();
		}
		
		
		#region Go
		
		public System.Windows.Input.ICommand GoCommand {get; private set;}
		
		private void ExecuteGo ()
		{
			if (String.IsNullOrEmpty(SelectedService)) {
				MessageBox.Show (noUrl);
			}
			ServiceDescriptionMessage = waitMessage;
			Uri uri = new Uri(SelectedService);
			StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, DiscoveryNetworkCredential.DefaultAuthenticationType));
		}
		
		
		private bool CanExecuteGo()
		{
			return true;
		}
		
		#endregion
		
		
		#region Discover Command
		
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
		
		#region AdvancedDialogCommand
		
		public System.Windows.Input.ICommand AdvancedDialogCommand {get;private set;}
		
		private bool CanExecuteAdvancedDialogCommand ()
		{
			return true;
		}
		
		private void ExecuteAdvancedDialogCommand ()
		{
			var vm = new AdvancedServiceViewModel();
			var view = new AdvancedServiceDialog();
			view.DataContext = vm;
			view.ShowDialog();
		}
			
		#endregion
		
		
		#region discover service Code from Matt

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
		/// 
		
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
					DiscoveryDocument handlerdoc = asyncDelegate.EndInvoke(result);
					if (!state.Credential.IsDefaultAuthenticationType) {
						AddCredential(state.Uri, state.Credential);
					}
					handler (protocol);
				} catch (Exception ex) {
					if (protocol.IsAuthenticationRequired) {
						HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
						AuthenticationHandler authHandler = new AuthenticationHandler(AuthenticateUser);
//	trouble					Invoke(authHandler, new object[] {state.Uri, authHeader.AuthenticationType});
					} else {
						LoggingService.Error("DiscoveryCompleted", ex);
//	trouble					Invoke(handler, new object[] {null});
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
				serviceDescriptionCollection = ServiceReferenceHelper.GetServiceDescriptions(protocol);
				
				ServiceDescriptionMessage = String.Format("{0} service(s) found at address {1}",
				                                          serviceDescriptionCollection.Count,
				                                          discoveryUri);
				DefaultNameSpace =  GetDefaultNamespace();
				FillItems (serviceDescriptionCollection);
				var referenceName = GetReferenceName(discoveryUri);
			}
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
		
		
		static string GetReferenceName(Uri uri)
		{
			if (uri != null) {
				return uri.Host;
			}
			return String.Empty;
		}

		#endregion
		
		
		#region new binding
		
		public string Title
		{
			get {return title;}
			set {title = value;
				base.RaisePropertyChanged(() =>Title);
			}
		}
		

		public string HeadLine {get; set;}
		
		private string discoverButtonContend;
		
		public string DiscoverButtonContend {
			get { return discoverButtonContend; }
			set { discoverButtonContend = value;
			base.RaisePropertyChanged(() =>DiscoverButtonContend);}
		}
		
		
		public List<string> MruServices {
			get {
				return mruServices; }
			set { mruServices = value;
				base.RaisePropertyChanged(() =>MruServices);
			}
		}
		
		
		public string SelectedService {
			get { return selectedService; }
			set { selectedService = value;
				base.RaisePropertyChanged(() =>SelectedService);}
		}
	
		
		public List <ServiceItem> ServiceItems {
			get {return items; }
			
			set {
				items = value;
				base.RaisePropertyChanged(() =>ServiceItems);
			}
		}
		
		
		public ServiceItem ServiceItem {
			get { return myItem; }
			set { myItem = value;
				UpdateListView();
				base.RaisePropertyChanged(() =>ServiceItem);
			}
		}
		
		
		public string ServiceDescriptionMessage {
			get { return serviceDescriptionMessage; }
			set { serviceDescriptionMessage = value;
				base.RaisePropertyChanged(() =>ServiceDescriptionMessage);
			}
		}
		
		
		public string DefaultNameSpace {
			get { return defaultNameSpace; }
			set { defaultNameSpace = value;
			base.RaisePropertyChanged(() =>DefaultNameSpace);}
		}
		
		
		public ObservableCollection<ImageAndDescription> TwoValues {
			get { return twoValues; }
			set {
				twoValues = value;
				base.RaisePropertyChanged(() =>TwoValues);
			}
		}
		
		//http://mikehadlow.blogspot.com/2006/06/simple-wsdl-object.html
		
		void UpdateListView ()
		{
			ServiceDescription desc = null;
			TwoValues.Clear();
			if(ServiceItem.Tag is ServiceDescription) {
				desc = (ServiceDescription)ServiceItem.Tag;
				var tv = new ImageAndDescription(PresentationResourceService.GetBitmapSource("Icons.16x16.Interface"),
				                                 desc.RetrievalUrl);
				TwoValues.Add(tv);
			}
			else if(ServiceItem.Tag is PortType)
			{
				PortType portType = (PortType)ServiceItem.Tag;
				foreach (Operation op in portType.Operations)
				{
					TwoValues.Add(new ImageAndDescription(PresentationResourceService.GetBitmapSource("Icons.16x16.Method"),
					                                      op.Name));
				}
			}
		}
			
		
		void FillItems (ServiceDescriptionCollection descriptions)
		{
			foreach (ServiceDescription element in descriptions)
			{
				Add (element);
			}
		}
		
		
		void Add(ServiceDescription description)
		{
			List<ServiceItem> l = new List<ServiceItem>();
			var name = ServiceReferenceHelper.GetServiceName(description);
			var rootNode = new ServiceItem(null,name);
			rootNode.Tag = description;

			foreach(Service service in description.Services) {
				var serviceNode = new ServiceItem(null,service.Name);
				serviceNode.Tag = service;
				l.Add(serviceNode);
				foreach (PortType portType  in description.PortTypes) {
					var portNode = new ServiceItem(PresentationResourceService.GetBitmapSource("Icons.16x16.Interface"),portType.Name);
					portNode.Tag = portType;
					serviceNode.SubItems.Add(portNode);
				}
			}
			ServiceItems = l;
		}

		
		#endregion
	}
	
	
	public class ImageAndDescription
	{
		public ImageAndDescription(BitmapSource bitmapSource,string description)
		{
			Image = bitmapSource;
			Description = description;
		}
		
		public BitmapSource Image {get;set;}
		public string Description {get;set;}
	}
	

	
	public class ServiceItem:ImageAndDescription
	{
		public ServiceItem (BitmapSource bitmapSource,string description):base(bitmapSource,description)
		{
			SubItems = new List<ServiceItem>();
		}
		public object Tag {get;set;}
		public List<ServiceItem> SubItems {get;set;}
	}
	
	public class CheckableImageAndDescription :ImageAndDescription
	{
		public CheckableImageAndDescription(BitmapSource bitmapSource,string description):base(bitmapSource,description)
		{
			
		}
		
		private bool itemChecked;
		
		public bool ItemChecked {
			get { return itemChecked; }
			set { itemChecked = value;}
//			base.RaisePropertyChanged(() =>IsChecked);}
		}
		
	}
}
