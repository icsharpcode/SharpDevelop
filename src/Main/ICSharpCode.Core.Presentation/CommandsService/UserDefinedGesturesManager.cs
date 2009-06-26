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
				
				var nameAttribute = xmlDocument.CreateAttribute("name");
				nameAttribute.Value = definedGestures.Key;
				bindingInfoNode.Attributes.Append(nameAttribute);
				
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
					var name = bindingInfoNode.Attributes["name"].Value;
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(bindingInfoNode.Attributes["gestures"].Value);
					
					userDefinedGestures[name] = gestures;
				}
			}
		}
		
		/// <summary>
		/// Get user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding name</param>
		/// <returns>Gestures assigned to this input binding</returns>
		public static InputGestureCollection GetInputBindingGesture(string inputBindingInfoName) 
		{
			InputGestureCollection gestures;
			userDefinedGestures.TryGetValue(inputBindingInfoName, out gestures);
			
			return gestures;
		}
		
		/// <summary>
		/// Set user defined input binding gestures
		/// </summary>
		/// <param name="inputBindingInfoName">Input binding name</param>
		/// <param name="inputGestureCollection">Gesture assigned to this input binding</param>
		public static void SetInputBindingGesture(string inputBindingInfoName, InputGestureCollection inputGestureCollection) 
		{
			userDefinedGestures[inputBindingInfoName] = inputGestureCollection;
		}
	}
}
