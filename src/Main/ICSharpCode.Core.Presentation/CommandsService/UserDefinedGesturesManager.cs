using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Input;
using System.IO;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Manages user defined gestures
	/// </summary>
	internal static class UserDefinedGesturesManager
	{
		private static Dictionary<string, InputGestureCollection> userDefinedGestures = new Dictionary<string, InputGestureCollection>();
		
		/// <summary>
		/// Save user defined gestures to specified file
		/// </summary>
		/// <param name="destinationPath">Path to the file containing user defined gestures</param>
		public static void Save(string destinationPath)
		{
			var xmlDocument = new XmlDocument();
			var rootNode = xmlDocument.CreateElement("UserDefinedGestures");
				
			foreach(var definedGestures in userDefinedGestures) {
				var bindingInfoNode = xmlDocument.CreateElement("InputBindingInfo");
				
				var idAttribute = xmlDocument.CreateAttribute("id");
				idAttribute.Value = definedGestures.Key;
				bindingInfoNode.Attributes.Append(idAttribute);
				
				var gesturesAttribute = xmlDocument.CreateAttribute("gestures");
				gesturesAttribute.Value = new InputGestureCollectionConverter().ConvertToInvariantString(definedGestures.Value);
				bindingInfoNode.Attributes.Append(gesturesAttribute);
			
				rootNode.AppendChild(bindingInfoNode);
			}
			
			xmlDocument.AppendChild(rootNode);
			xmlDocument.Save(destinationPath);
		}
		
		/// <summary>
		/// Load user defined gesturs from specified file
		/// </summary>
		/// <param name="sourcePath">Path to the file containing user defined gestures</param>
		public static void Load(string sourcePath)
		{
			userDefinedGestures.Clear();
			
			if(File.Exists(sourcePath)) {
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(sourcePath);
				
				foreach(XmlElement bindingInfoNode in xmlDocument.SelectNodes("//InputBindingInfo")) {
					var identifier = bindingInfoNode.Attributes["id"].Value;
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(bindingInfoNode.Attributes["gestures"].Value);
					
					userDefinedGestures[identifier] = gestures;
				}
			}
		}
		
		/// <summary>
		/// Get user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding</param>
		/// <returns>Gestures assigned to this input binding</returns>
		public static InputGestureCollection GetInputBindingGesture(InputBindingInfo inputBindingInfo) 
		{
			var identifier = GetInputBindingInfoIdentifier(inputBindingInfo);
			if(identifier != null) {
				InputGestureCollection gestures;
				userDefinedGestures.TryGetValue(identifier, out gestures);
				
				return gestures;
			} 
			
			return null;
		}
		
		/// <summary>
		/// Set user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding name</param>
		/// <param name="inputGestureCollection">Gesture assigned to this input binding</param>
		public static void SetInputBindingGestures(InputBindingInfo inputBindingInfo, InputGestureCollection inputGestureCollection) 
		{
			var identifier = GetInputBindingInfoIdentifier(inputBindingInfo);
			
			userDefinedGestures[identifier] = inputGestureCollection;
		}
		
		private static string GetInputBindingInfoIdentifier(InputBindingInfo inputBindingInfo) {
			if(inputBindingInfo.OwnerTypeName != null) {
				return string.Format("OwnerType={0};RoutedCommandName={1}", inputBindingInfo.OwnerTypeName, inputBindingInfo.RoutedCommandName);
			} else if(inputBindingInfo.OwnerInstanceName != null) {
				return string.Format("OwnerInstance={0};RoutedCommandName={1}", inputBindingInfo.OwnerInstanceName, inputBindingInfo.RoutedCommandName);
			} else {
				return null;
			}
		}
		
	}
}
