// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ResourceEditor
{
	delegate void ResourceChangedEventHandler(object sender, ResourceEventArgs e);
	
	interface IResourceView : IDisposable
	{
		bool WriteProtected
		{
			get;
			set;
		}
		
		ResourceItem ResourceItem
		{
			get;
			set;
		}
		
		event ResourceChangedEventHandler ResourceChanged;
	}
	
	class ResourceEventArgs
	{
		string resourceName;
		object resourceValue;
		
		public ResourceEventArgs(string resourceName, object resourceValue)
		{
			this.resourceName = resourceName;
			this.resourceValue = resourceValue;
		}
		
		public string ResourceName
		{
			get {
				return resourceName;
			}
			set {
				resourceName = value;
			}
		}
		
		public object ResourceValue
		{
			get {
				return resourceValue;
			}
			set {
				resourceValue = value;
			}
		}
	}
}
