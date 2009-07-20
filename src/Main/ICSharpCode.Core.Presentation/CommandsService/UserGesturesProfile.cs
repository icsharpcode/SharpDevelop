/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 7/1/2009
 * Time: 12:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of UserGesturesProfile.
	/// </summary>
	public class UserGesturesProfile : IEnumerable<KeyValuePair<InputBindingIdentifier, InputGestureCollection>>, ICloneable
	{
		private Dictionary<InputBindingIdentifier, InputGestureCollection> userDefinedGestures = new Dictionary<InputBindingIdentifier, InputGestureCollection>();
		
		public string Path
		{
			get; set;
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
		
		public UserGesturesProfile()
		{
			
		}
		
		public UserGesturesProfile(string name, string text, bool readOnly)
		{
			Name = name;
			Text = text;
			ReadOnly = readOnly;
		}
		
		
		/// <summary>
		/// Load user defined gesturs from specified file
		/// </summary>
		/// <param name="sourcePath">Path to the file containing user defined gestures</param>
		public void Load()
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(Path);
			
			var rootNode = xmlDocument.SelectSingleNode("//UserGesturesProfile");
			
			Name = rootNode.Attributes["name"].Value;
			Text = rootNode.Attributes["text"].Value;
			ReadOnly = Convert.ToBoolean(rootNode.Attributes["read-only"].Value);

			foreach(XmlElement bindingInfoNode in xmlDocument.SelectNodes("//InputBinding")) {
				var identifier = new InputBindingIdentifier();
				identifier.RoutedCommandName = bindingInfoNode.Attributes["routed-command"].Value;
				
				var ownerInstanceAttribute = bindingInfoNode.Attributes["owner-instance"];
				if(ownerInstanceAttribute != null) {
					identifier.OwnerInstanceName = ownerInstanceAttribute.Value;
				} else {
					var ownerTypeAttribute = bindingInfoNode.Attributes["owner-type"];
					identifier.OwnerTypeName = ownerTypeAttribute.Value;
				}
				
				var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(bindingInfoNode.Attributes["gestures"].Value);
				this[identifier] = gestures;
			}
		}
		
		/// <summary>
		/// Save user defined gestures to specified file
		/// </summary>
		/// <param name="destinationPath">Path to the file containing user defined gestures</param>
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
			
			var readOnlyAttribute = xmlDocument.CreateAttribute("read-only");
			readOnlyAttribute.Value = Convert.ToString(ReadOnly);
			rootNode.Attributes.Append(readOnlyAttribute);
			
			foreach(var definedGestures in this) {
				var bindingInfoNode = xmlDocument.CreateElement("InputBinding");
				
				if(definedGestures.Key.RoutedCommandName != null) {
					var routedCommandAttribute = xmlDocument.CreateAttribute("routed-command");
					routedCommandAttribute.Value = definedGestures.Key.RoutedCommandName;
					bindingInfoNode.Attributes.Append(routedCommandAttribute);
				} else {
					throw new ApplicationException("InputBindingIdentifier should have routed command name assigned to it");
				}
				
				if(definedGestures.Key.OwnerInstanceName != null) {
					var ownerInstanceAttribute = xmlDocument.CreateAttribute("owner-instance");
					ownerInstanceAttribute.Value = definedGestures.Key.OwnerInstanceName;
					bindingInfoNode.Attributes.Append(ownerInstanceAttribute);
				} else if(definedGestures.Key.OwnerTypeName != null) {
					var ownerTypeAttribute = xmlDocument.CreateAttribute("owner-type");
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
		
		public event InputBindingGesturesChangedHandler InputBindingGesturesChanged;
		
		public void Clear()
		{
			if(InputBindingGesturesChanged != null) {
				var identifiers = new List<InputBindingIdentifier>();
				foreach(var userDefinedGesture in userDefinedGestures) {
					InputBindingGesturesChanged.Invoke(this, new InputBindingGesturesChangedArgs { InputBindingIdentifier = userDefinedGesture.Key});
				}
			}
			
			userDefinedGestures.Clear();
		}
		
		public InputGestureCollection this[InputBindingIdentifier identifier]
		{
			get { return GetInputBindingGesture(identifier); }
			set { SetInputBindingGestures(identifier, value); }
		}

		/// <summary>
		/// Get user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding</param>
		/// <returns>Gestures assigned to this input binding</returns>
		private InputGestureCollection GetInputBindingGesture(InputBindingIdentifier identifier) 
		{
			InputGestureCollection gestures;
			userDefinedGestures.TryGetValue(identifier, out gestures);
			
			return gestures;
		}
		
		/// <summary>
		/// Set user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding name</param>
		/// <param name="inputGestureCollection">Gesture assigned to this input binding</param>
		private void SetInputBindingGestures(InputBindingIdentifier identifier, InputGestureCollection inputGestureCollection) 
		{
			if(InputBindingGesturesChanged != null) {
				InputBindingGesturesChanged.Invoke(this, new InputBindingGesturesChangedArgs { InputBindingIdentifier = identifier});
			}
			
			userDefinedGestures[identifier] = inputGestureCollection;
		}
		
		public IEnumerator<KeyValuePair<InputBindingIdentifier, InputGestureCollection>> GetEnumerator()
		{
			return userDefinedGestures.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return userDefinedGestures.GetEnumerator();
		}
		
		public object Clone()
		{
			var profile = new UserGesturesProfile(Name, Text, ReadOnly);
			
			foreach(var definedGesture in userDefinedGestures) {
				profile.userDefinedGestures.Add(definedGesture.Key, new InputGestureCollection(definedGesture.Value));
			}
			
			return profile;
		}
	}
	
	public class InputBindingGesturesChangedArgs
	{
		public InputBindingIdentifier InputBindingIdentifier
		{
			get; set;
		}
	}
	
	public delegate void InputBindingGesturesChangedHandler(object sender, InputBindingGesturesChangedArgs args);
}
