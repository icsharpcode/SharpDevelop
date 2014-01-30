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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class AddServiceReferenceViewModel : ViewModelBase
	{
		string header = "To see a list of available services on a specific server, enter a service URL and click Go.";
		string noUrl = "Please enter the address of the Service.";
		string title =  "Add Service Reference";
		string waitMessage = "Please wait....";
		string defaultNameSpace;
		string serviceDescriptionMessage;
		
		ObservableCollection<ImageAndDescription> twoValues;
		
		ServiceReferenceUrlHistory urlHistory = new ServiceReferenceUrlHistory();
		string selectedService;
		IProject project;
		ServiceReferenceGenerator serviceGenerator;
		List<CheckableAssemblyReference> assemblyReferences;
		
		List<ServiceItem> items = new List<ServiceItem>();
		ServiceItem myItem;
		
		Uri discoveryUri;
		ServiceReferenceDiscoveryClient serviceReferenceDiscoveryClient;
		
		public AddServiceReferenceViewModel(IProject project)
		{
			this.project = project;
			this.serviceGenerator = new ServiceReferenceGenerator(project);
			this.serviceGenerator.Complete += ServiceReferenceGenerated;
			this.assemblyReferences = serviceGenerator.GetCheckableAssemblyReferences().ToList();
			HeadLine = header;
			
			GoCommand = new RelayCommand(DiscoverServices);
			AdvancedDialogCommand = new RelayCommand(ShowAdvancedOptions);
			TwoValues = new ObservableCollection<ImageAndDescription>();
		}
		
		public ICommand GoCommand { get; private set; }
		
		void DiscoverServices()
		{
			Uri uri = TryGetUri();
			if (uri != null) {
				ServiceDescriptionMessage = waitMessage;
				StartDiscovery(uri);
			}
		}
		
		Uri TryGetUri()
		{
			return TryGetUri(selectedService);
		}
		
		Uri TryGetUri(string url)
		{
			if (String.IsNullOrEmpty(url)) {
				ServiceDescriptionMessage = noUrl;
				return null;
			}
			
			try {
				return new Uri(url);
			} catch (Exception ex) {
				ServiceDescriptionMessage = ex.Message;
			}
			return null;
		}
		
		public ICommand AdvancedDialogCommand { get; private set; }
		
		void ShowAdvancedOptions()
		{
			var vm = new AdvancedServiceViewModel(serviceGenerator.Options.Clone());
			vm.AssembliesToReference.AddRange(assemblyReferences);
			var view = new AdvancedServiceDialog();
			view.DataContext = vm;
			if (view.ShowDialog() ?? false) {
				serviceGenerator.Options = vm.Options;
				serviceGenerator.UpdateAssemblyReferences(assemblyReferences);
			}
		}
		
		void StartDiscovery(Uri uri)
		{
			if (serviceReferenceDiscoveryClient != null) {
				serviceReferenceDiscoveryClient.DiscoveryComplete -= ServiceReferenceDiscoveryComplete;
			}
			serviceReferenceDiscoveryClient = new ServiceReferenceDiscoveryClient();
			serviceReferenceDiscoveryClient.DiscoveryComplete += ServiceReferenceDiscoveryComplete;
			
			discoveryUri = uri;
			serviceReferenceDiscoveryClient.Discover(uri);
		}
		
		void ServiceReferenceGenerated(object sender, GeneratorCompleteEventArgs e)
		{
			if (e.IsSuccess) {
				new RefreshProjectBrowser().Run();
			}
		}

		void ServiceReferenceDiscoveryComplete(object sender, ServiceReferenceDiscoveryEventArgs e)
		{
			if (Object.ReferenceEquals(serviceReferenceDiscoveryClient, sender)) {
				if (e.HasError) {
					OnWebServiceDiscoveryError(e.Error);
				} else {
					DiscoveredWebServices(e.Services);
				}
			}
		}
		
		void OnWebServiceDiscoveryError(Exception ex)
		{
			ServiceDescriptionMessage = ex.Message;
			ICSharpCode.Core.LoggingService.Debug("DiscoveryCompleted: " + ex.ToString());
		}
		
		void DiscoveredWebServices(ServiceDescriptionCollection services)
		{
			ServiceDescriptionMessage = String.Format("{0} service(s) found at address {1}", services.Count, discoveryUri);
			if (services.Count > 0) {
				AddUrlToHistory(discoveryUri);
			}
			DefaultNameSpace = GetDefaultNamespace();
			FillItems(services);
			string referenceName = ServiceReferenceHelper.GetReferenceName(discoveryUri);
		}
		
		void AddUrlToHistory(Uri discoveryUri)
		{
			urlHistory.AddUrl(discoveryUri);
			OnPropertyChanged("MruServices");
		}
		
		/// <summary>
		/// Gets the namespace to be used with the generated web reference code.
		/// </summary>
		string GetDefaultNamespace()
		{
			if (discoveryUri != null) {
				return discoveryUri.Host;
			}
			return String.Empty;
		}
		
		public string Title
		{
			get { return title; }
			set {
				title = value;
				OnPropertyChanged();
			}
		}
		
		public string HeadLine { get; set; }
		
		public List<string> MruServices {
			get { return urlHistory.Urls; }
		}
		
		public string SelectedService {
			get { return selectedService; }
			set {
				selectedService = value;
				OnPropertyChanged();
			}
		}
	
		public List <ServiceItem> ServiceItems {
			get { return items; }
			set {
				items = value;
				OnPropertyChanged();
			}
		}
		
		public ServiceItem ServiceItem {
			get { return myItem; }
			set {
				myItem = value;
				UpdateListView();
				OnPropertyChanged();
			}
		}
		
		public string ServiceDescriptionMessage {
			get { return serviceDescriptionMessage; }
			set {
				serviceDescriptionMessage = value;
				OnPropertyChanged();
			}
		}
		
		public string DefaultNameSpace {
			get { return defaultNameSpace; }
			set {
				defaultNameSpace = value;
				OnPropertyChanged();
			}
		}
		
		public ObservableCollection<ImageAndDescription> TwoValues {
			get { return twoValues; }
			set {
				twoValues = value;
				OnPropertyChanged();
			}
		}
		
		//http://mikehadlow.blogspot.com/2006/06/simple-wsdl-object.html
		
		void UpdateListView()
		{
			TwoValues.Clear();
			if (ServiceItem.Tag is ServiceDescription) {
				ServiceDescription desc = (ServiceDescription)ServiceItem.Tag;
				var tv = new ImageAndDescription("Icons.16x16.Interface", desc.RetrievalUrl);
				TwoValues.Add(tv);
			} else if (ServiceItem.Tag is PortType) {
				PortType portType = (PortType)ServiceItem.Tag;
				foreach (Operation op in portType.Operations) {
					TwoValues.Add(new ImageAndDescription("Icons.16x16.Method", op.Name));
				}
			}
		}
		
		void FillItems(ServiceDescriptionCollection descriptions)
		{
			foreach (ServiceDescription element in descriptions) {
				Add(element);
			}
		}
		
		void Add(ServiceDescription description)
		{
			var items = new List<ServiceItem>();
			string name = ServiceReferenceHelper.GetServiceName(description);
			var rootNode = new ServiceItem(name);
			rootNode.Tag = description;

			foreach (Service service in description.Services) {
				var serviceNode = new ServiceItem(service.Name);
				serviceNode.Tag = service;
				items.Add(serviceNode);
				foreach (PortType portType  in description.PortTypes) {
					var portNode = new ServiceItem("Icons.16x16.Interface", portType.Name);
					portNode.Tag = portType;
					serviceNode.SubItems.Add(portNode);
				}
			}
			ServiceItems = items;
		}
		
		public bool CanAddServiceReference()
		{
			return GetServiceUri() != null && ValidateNamespace();
		}
		
		bool ValidateNamespace()
		{
			if (String.IsNullOrEmpty(defaultNameSpace)) {
				ServiceDescriptionMessage = "No namespace specified.";
				return false;
			}
				
			if (!WebReference.IsValidNamespace(defaultNameSpace) || !WebReference.IsValidReferenceName(defaultNameSpace)) {
				ServiceDescriptionMessage = "Namespace contains invalid characters.";
			}
			return true;
		}
		
		public void AddServiceReference()
		{
			CompilerMessageView.Instance.BringToFront();
			Uri uri = GetServiceUri();
			if (uri == null)
				return;
			
			try {
				serviceGenerator.Options.ServiceName = defaultNameSpace;
				serviceGenerator.Options.Url = uri.ToString();
				serviceGenerator.AddServiceReference();
			} catch (Exception ex) {
				ICSharpCode.Core.LoggingService.Error("Failed to add service reference.", ex);
			}
		}
		
		Uri GetServiceUri()
		{
			if (discoveryUri != null) {
				return discoveryUri;
			}
			return TryGetUri();
		}
	}
	
	public class ImageAndDescription
	{
		public ImageAndDescription(BitmapSource bitmapSource, string description)
		{
			Image = bitmapSource;
			Description = description;
		}
		
		public ImageAndDescription(string resourceName, string description)
			: this(PresentationResourceService.GetBitmapSource(resourceName), description)
		{
		}
		
		public BitmapSource Image { get; set; }
		public string Description { get; set; }
	}
	
	public class ServiceItem : ImageAndDescription
	{
		List<ServiceItem> subItems = new List<ServiceItem>();
			
		public ServiceItem(string description)
			: base((BitmapSource)null, description)
		{
		}
		
		public ServiceItem(string resourceName, string description)
			: base(resourceName, description)
		{
		}
		
		public object Tag { get; set; }
		
		public List<ServiceItem> SubItems {
			get { return subItems; }
			set { subItems = value; }
		}
	}
	
	public class CheckableAssemblyReference : ImageAndDescription
	{
		static BitmapSource ReferenceImage;

		ReferenceProjectItem projectItem;
		
		public CheckableAssemblyReference(ReferenceProjectItem projectItem)
			: this(projectItem.AssemblyName.ShortName)
		{
			this.projectItem = projectItem;
		}
		
		protected CheckableAssemblyReference(string description)
			: base(GetReferenceImage(), description)
		{
		}
		
		static BitmapSource GetReferenceImage()
		{
			try {
				if (ReferenceImage == null) {
					ReferenceImage = PresentationResourceService.GetBitmapSource("Icons.16x16.Reference");
				}
				return ReferenceImage;
			} catch (Exception) {
				return null;
			}
		}
		
		public bool ItemChecked { get; set; }
		
		public string GetFileName()
		{
			if (projectItem != null) {
				return projectItem.FileName;
			}
			return Description;
		}
	}
}
