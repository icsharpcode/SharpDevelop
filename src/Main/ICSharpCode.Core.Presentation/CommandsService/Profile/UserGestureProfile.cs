using System;
using System.Xml;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores user defined gestures for different actions
	/// </summary>
	public class UserGestureProfile : IEnumerable<KeyValuePair<BindingInfoTemplate, InputGestureCollection>>
	{
		private Dictionary<BindingInfoTemplate, InputGestureCollection> userDefinedGestures = new Dictionary<BindingInfoTemplate, InputGestureCollection>();
		private string _path;
		
		/// <summary>
		/// Gets path to the file on disk where this profile is stored
		/// </summary>
		public string Path
		{
			get {
				return _path;
			}
		}
		
		public bool ReadOnly
		{
			get; set;
		}
		
		public string Name
		{
			get; set;
		}

		public string Text
		{
			get; set;
		}
		
		/// <summary>
		/// Creates new instance of <see cref="UserGestureProfile" />
		/// </summary>
		/// <param name="filePath">Path to a file where this profile will be stored</param>
		public UserGestureProfile(string filePath)
		{
			_path = filePath;
		}
		
		/// <summary>
		/// Creates new instance of <see cref="UserGestureProfile" />
		/// </summary>
		/// <param name="filePath">Path to a file where this profile will be stored</param>
		/// <param name="name">Name of the profile</param>
		/// <param name="text">Text presented to user when describing this profile</param>
		/// <param name="readOnly">Determines whether profile is read-only</param>
		public UserGestureProfile(string filePath, string name, string text, bool readOnly)
		{
			_path = filePath;
			Name = name;
			Text = text;
			ReadOnly = readOnly;
		}
		
		/// <summary>
		/// Load user defined gestures from file described in <see cref="UserGestureProfile.Path" />
		/// </summary>
		public void Load()
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(Path);
			
			var rootNode = xmlDocument.SelectSingleNode("//UserGesturesProfile");
			
			Name = rootNode.Attributes["name"].Value;
			Text = rootNode.Attributes["text"].Value;
			ReadOnly = Convert.ToBoolean(rootNode.Attributes["readonly"].Value);

			foreach(XmlElement bindingInfoNode in xmlDocument.SelectNodes("//InputBinding")) {
				string identifierInstanceName = null;
				string identifierTypeName = null;
				var ownerInstanceAttribute = bindingInfoNode.Attributes["ownerinstance"];
				if(ownerInstanceAttribute != null) {
					identifierInstanceName = ownerInstanceAttribute.Value;
				} else {
					var ownerTypeAttribute = bindingInfoNode.Attributes["ownertype"];
					identifierTypeName = ownerTypeAttribute.Value;
				}
				
				var identifier = BindingInfoTemplate.Create(identifierInstanceName, identifierTypeName, bindingInfoNode.Attributes["routedcommand"].Value);
				var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(bindingInfoNode.Attributes["gestures"].Value);
				this[identifier] = gestures;
			}
		}
		
		/// <summary>
		/// Save user defined gestures to file described in <see cref="UserGestureProfile.Path" />
		/// </summary>
		public void Save()
		{
			var xmlDocument = new XmlDocument();
			var rootNode = xmlDocument.CreateElement("UserGesturesProfile");
			
			var nameAttribute = xmlDocument.CreateAttribute("name");
			nameAttribute.Value = Name;
			rootNode.Attributes.Append(nameAttribute);
			
			var textAttribute = xmlDocument.CreateAttribute("text");
			textAttribute.Value = Text;
			rootNode.Attributes.Append(textAttribute);
			
			var readOnlyAttribute = xmlDocument.CreateAttribute("readonly");
			readOnlyAttribute.Value = Convert.ToString(ReadOnly);
			rootNode.Attributes.Append(readOnlyAttribute);
			
			foreach(var definedGestures in this) {
				var bindingInfoNode = xmlDocument.CreateElement("InputBinding");
				
				if(definedGestures.Key.RoutedCommandName != null) {
					var routedCommandAttribute = xmlDocument.CreateAttribute("routedcommand");
					routedCommandAttribute.Value = definedGestures.Key.RoutedCommandName;
					bindingInfoNode.Attributes.Append(routedCommandAttribute);
				} else {
					throw new ApplicationException("InputBindingIdentifier should have routed command name assigned to it");
				}
				
				if(definedGestures.Key.OwnerInstanceName != null) {
					var ownerInstanceAttribute = xmlDocument.CreateAttribute("ownerinstance");
					ownerInstanceAttribute.Value = definedGestures.Key.OwnerInstanceName;
					bindingInfoNode.Attributes.Append(ownerInstanceAttribute);
				} else if(definedGestures.Key.OwnerTypeName != null) {
					var ownerTypeAttribute = xmlDocument.CreateAttribute("ownertype");
					ownerTypeAttribute.Value = definedGestures.Key.OwnerTypeName;
					bindingInfoNode.Attributes.Append(ownerTypeAttribute);
				} else {
					throw new ApplicationException("InputBindingIdentifier should have owner instance name or type name assigned");
				}
				
				var gesturesAttribute = xmlDocument.CreateAttribute("gestures");
				gesturesAttribute.Value = new InputGestureCollectionConverter().ConvertToInvariantString(definedGestures.Value);
				bindingInfoNode.Attributes.Append(gesturesAttribute);
			
				rootNode.AppendChild(bindingInfoNode);
			}
			
			xmlDocument.AppendChild(rootNode);
			xmlDocument.Save(Path);
		}
		
		/// <summary>
		/// Clear all defined gestures
		/// </summary>
		public void Clear()
		{
			var descriptions = new List<GesturesModificationDescription>();
			var args = new NotifyGesturesChangedEventArgs();
			
			foreach(var pair in this) {
				var template = pair.Key;
				var newGestures = SDCommandManager.FindInputGestures(template, null);
				
				descriptions.Add(
					new GesturesModificationDescription(
						pair.Key, 
						userDefinedGestures[pair.Key], 
						newGestures));
			}
			
			userDefinedGestures.Clear();
			
			SDCommandManager.InvokeGesturesChanged(this, args);
		}
		
		/// <summary>
		/// Assigns <see cref="InputGestureCollection" /> to <see cref="BindingInfoTemplate" />
		/// representing <see cref="InputBindingInfo" />
		/// </summary>
		public InputGestureCollection this[BindingInfoTemplate identifier]
		{
			get { return GetInputBindingGesture(identifier); }
			set { SetInputBindingGestures(identifier, value); }
		}

		private InputGestureCollection GetInputBindingGesture(BindingInfoTemplate identifier) 
		{
			InputGestureCollection gestures;
			userDefinedGestures.TryGetValue(identifier, out gestures);
			
			return gestures;
		}
		
		private void SetInputBindingGestures(BindingInfoTemplate identifier, InputGestureCollection inputGestureCollection) 
		{
			var oldGestures = GetInputBindingGesture(identifier);
			var newGestures = inputGestureCollection;
			
			if(oldGestures == null || newGestures == null) {
				var defaultGestures = SDCommandManager.FindInputGestures(identifier, null);
				
				oldGestures = oldGestures ?? defaultGestures;
				newGestures = newGestures ?? defaultGestures;
			}
			
			var args = new NotifyGesturesChangedEventArgs(
				new GesturesModificationDescription(identifier, oldGestures, newGestures));
			
			if(inputGestureCollection != null) {
				userDefinedGestures[identifier] = inputGestureCollection;
			} else {
				userDefinedGestures.Remove(identifier);
			}
			
			InvokeGesturesChanged(this, args);
		}
		
		/// <inheritdoc />
		public IEnumerator<KeyValuePair<BindingInfoTemplate, InputGestureCollection>> GetEnumerator()
		{
			return userDefinedGestures.GetEnumerator();
		}
		
		/// <inheritdoc />
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return userDefinedGestures.GetEnumerator();
		}
		
		/// <summary>
		/// Occurs when any gestures defined in this profile are changed, removed or added
		/// </summary>
		public event NotifyGesturesChangedEventHandler GesturesChanged;
		
		private void InvokeGesturesChanged(object sender, NotifyGesturesChangedEventArgs args)
		{
			if(GesturesChanged != null) {
				GesturesChanged.Invoke(sender, args);
			}
		}
	}
}
