using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

// TODO: handle overloading (links)

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public interface IHelpFileFormat
	{
		bool NumerateConstructor {
			get;
		}
		string NamespaceFormat {
			get;
		}
		string ClassTopic {
			get;
		}
		string MembersTopic {
			get;
		}
		string MethodsTopic {
			get;
		}
		string PropertiesTopic {
			get;
		}
		string FieldsTopic {
			get;
		}
		string EventsTopic {
			get;
		}
		
		string MemberFormat {
			get;
		}
	}
	
	public class SDKHelpFileFormat : IHelpFileFormat
	{
		string prefix = "ms-help://MS.NETFrameworkSDK";
		
		public bool NumerateConstructor {
			get {
				return false;
			}
		}
		
		string Prefix {
			get {
				return prefix;
			}
		}
		
		public string NamespaceFormat {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%NAMESPACEFLAT%.htm");
			}
		}
		
		public string ClassTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%ClassTopic.htm");
			}
		}
		
		public string MembersTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%MembersTopic.htm");
			}
		}
		
		public string MethodsTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%MethodsTopic.htm");
			}
		}
		
		public string PropertiesTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%PropertiesTopic.htm");
			}
		}
		public string FieldsTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%FieldsTopic.htm");
			}
		}
		public string EventsTopic {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%EventsTopic.htm");
			}
		}
		
		public string MemberFormat {
			get {
				return String.Concat(Prefix, "/cpref/html/frlrf%FULLFLATTYPENAME%class%MEMBERNAME%Topic%NUM%.htm");
			}
		}
		
		string ScanSubKeys(RegistryKey key)
		{
			string[] subKeys = key.GetSubKeyNames();
			foreach (string subKey in subKeys) {
				RegistryKey sub = key.OpenSubKey(subKey);
				if (sub.GetValue(null).ToString().StartsWith("Microsoft .NET Framework SDK")) {
					return sub.GetValue("Filename").ToString();
				}
			}
			return null;
		}
		
		public SDKHelpFileFormat()
		{
			string localHelp = "0x" + Thread.CurrentThread.CurrentCulture.LCID.ToString("X4");
			RegistryKey helpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSDN\7.0\Help");
			
			RegistryKey k = helpKey.OpenSubKey(localHelp);
			bool found = false;
			if (k != null) {
				string v = ScanSubKeys(k);
				if (v != null) {
					prefix = v;
					found = true;
				}
			}
			if (!found) {
				// use default english subkey
				k = helpKey.OpenSubKey("0x0409");
				string v = k != null ? ScanSubKeys(k) : null;
				if (v != null) {
					prefix = v;
				} else {
					string[] subKeys = helpKey.GetSubKeyNames();
					foreach (string subKey in subKeys) {
						if (subKey.StartsWith("0x")) {
							prefix = ScanSubKeys(helpKey.OpenSubKey(subKey));
							break;
						}
					}
				}
			}
		}
	}
	
	public class DirectX9HelpFileFormat : IHelpFileFormat
	{
		public bool NumerateConstructor {
			get {
				return true;
			}
		}
		
		public string NamespaceFormat {
			get {
				return "ms-help://MS.DirectX9.1033/DirectX9_m/directx/ref/ns/%NAMESPACE%.htm";
			}
		}
		
		public string ClassTopic {
			get {
				return "ms-help://MS.DirectX9.1033/DirectX9_m/directx/ref/ns/%NAMESPACE%/%TYPESHORT%/%TYPENAME%/%TYPENAME%.htm";
			}
		}
		
		public string MembersTopic {
			get {
				return "ms-help://MS.DirectX9.1033/DirectX9_m/directx/ref/ns/%NAMESPACE%/%TYPESHORT%/%TYPENAME%/%TYPENAME%.htm";
			}
		}
		public string MethodsTopic {
			get {
				return null;
			}
		}
		public string PropertiesTopic {
			get {
				return null;
			}
		}
		public string FieldsTopic {
			get {
				return null;
			}
		}
		public string EventsTopic {
			get {
				return null;
			}
		}
		
		public string MemberFormat {
			get {
				return "ms-help://MS.DirectX9.1033/DirectX9_m/directx/ref/ns/%NAMESPACE%/%TYPESHORT%/%TYPENAME%/%MEMBERSHORT%/%MEMBERNAME%%NUM%.htm";
			}
		}
	}
	
	
	public class classNodeBuilder : ITypeNodeBuilder
	{
		BindingFlags flags = BindingFlags.Instance  | 
		                     BindingFlags.DeclaredOnly |
		                     BindingFlags.Static    | 
		                     BindingFlags.Public;
		
		public static IHelpFileFormat helpFileFormat = new SDKHelpFileFormat();
		
		public virtual string Postfix {
			get {
				return " class";
			}
		}
		
		public virtual string ShortType {
			get {
				return "c";
			}
		}
		
		public void SetLink(XmlDocument doc, XmlNode node, string link)
		{
			if (link != null) {
				XmlAttribute attrib = doc.CreateAttribute("link");
				attrib.Value = link;
				node.Attributes.Append(attrib);
				
				attrib = doc.CreateAttribute("ismsdn");
				attrib.Value = "true";
				node.Attributes.Append(attrib);
			}
		}
		
		
		
		public XmlNode createLinkNode(XmlDocument doc, string name, string link)
		{
			XmlNode node = doc.CreateElement("HelpTopic");
			XmlAttribute attrib = doc.CreateAttribute("name");
			attrib.Value = name;
			node.Attributes.Append(attrib);
			SetLink(doc, node, link);
			return node;
		}
		
		public XmlNode createFolderNode(XmlDocument doc, string name)
		{
			XmlNode node = doc.CreateElement("HelpFolder");
			XmlAttribute attrib = doc.CreateAttribute("name");
			attrib.Value = name;
			node.Attributes.Append(attrib);
			
			return node;
		}
		
		public string ConvertLink(string format, Type type)
		{
			if (format == null) {
				return null;
			}
			string output = Regex.Replace(format, "%FULLFLATTYPENAME%", (type.Namespace + type.Name).Replace(".", "").ToLower(), RegexOptions.None);
			output = Regex.Replace(output, "%NAMESPACE%", type.Namespace, RegexOptions.None);
			output = Regex.Replace(output, "%NAMESPACEFLAT%", type.Namespace.Replace(".", ""), RegexOptions.None);
			output = Regex.Replace(output, "%TYPENAME%", type.Name, RegexOptions.None);
			output = Regex.Replace(output, "%TYPESHORT%", ShortType, RegexOptions.None);
			return output;
		}
		
		public string ConvertLink(string format, Type type, string memberName, string memberShort, string memberNum)
		{
			string output = ConvertLink(format, type);
			
			output = Regex.Replace(output, "%MEMBERNAME%", memberName, RegexOptions.None);
			output = Regex.Replace(output, "%MEMBERSHORT%", memberShort, RegexOptions.None);
			output = Regex.Replace(output, "%NUM%", memberNum, RegexOptions.None);
			
			return output;
		}
		
		public virtual XmlNode buildNode(XmlDocument doc, Type type)
		{
			XmlNode rootnode = createFolderNode(doc, type.Name + Postfix);
			SetLink(doc, rootnode, ConvertLink(helpFileFormat.ClassTopic, type));
			
			rootnode.AppendChild(createLinkNode(doc, type.Name + " members", ConvertLink(helpFileFormat.MembersTopic, type)));
			
			// search for constructors
			XmlNode constructorsNode = createFolderNode(doc, "Constructors");
			int constructorNum = 0;
			ConstructorInfo[] constructorInfo = type.GetConstructors(flags);
			if (constructorInfo.Length > 0) {
				SetLink(doc, constructorsNode, ConvertLink(helpFileFormat.MemberFormat, type, "ctor", "m", ""));
			}
			foreach(ConstructorInfo constructor in constructorInfo) {
				if(constructor.DeclaringType == type) {
					string memberFormat = null;
					if (constructorInfo.Length > 1) {
						memberFormat    = ConvertLink(helpFileFormat.MemberFormat, type, "ctor", "m", constructorNum == 0 && !helpFileFormat.NumerateConstructor ? (constructorNum + 1).ToString() : constructorNum.ToString());
						++constructorNum;
					} else {
						memberFormat    = ConvertLink(helpFileFormat.MemberFormat, type, "ctor", "m", "");
					}
					
					constructorsNode.AppendChild(createLinkNode(doc,
					                                            constructor.Name + "(" + getMethodSignature(constructor) + ")",
					                                            memberFormat));
				}
			}
			if(constructorsNode.ChildNodes.Count > 0) {
				rootnode.AppendChild(constructorsNode);
			}
			
			// search for fields
			XmlNode fieldsNode = createFolderNode(doc, "Fields");
			SetLink(doc, fieldsNode, ConvertLink(helpFileFormat.FieldsTopic, type));
			
			foreach(FieldInfo field in type.GetFields(flags)) {
				if(field.DeclaringType == type) {
					string memberFormat = ConvertLink(helpFileFormat.MemberFormat, type, field.Name, "f", "");
					fieldsNode.AppendChild(createLinkNode(doc, field.Name, memberFormat));
				}
			}
			if(fieldsNode.ChildNodes.Count > 0) {
				rootnode.AppendChild(fieldsNode);
			}
			
			
			// search for methods
			XmlNode methodsNode = createFolderNode(doc, "Methods");
			SetLink(doc, methodsNode, ConvertLink(helpFileFormat.MethodsTopic, type));
			MethodInfo[] methodInfos = type.GetMethods(flags);
			int methodNum = 0;
			for (int i = 0; i < methodInfos.Length; ++i) {
				MethodInfo method = methodInfos[i];
				if (!method.IsSpecialName && method.DeclaringType == type) {
					// HACK: BUGFIX FOR DirectX.Direct3D.Device and other directx classes ... 
					// for unknown reason add and remove methods are added to the non special name, public space
					// maybe because the directx assemblies are generated by a buggy internal MS compiler :)
					if (method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")) {
						continue;
					}
					string number = "";
					if (i > 0 && methodInfos[i - 1].Name != method.Name) {
						methodNum = helpFileFormat.NumerateConstructor ? 0 : 1;
					}
					if (i > 0 && methodInfos[i - 1].Name == method.Name) {
						number = methodNum++.ToString();
					} else if (i + 1 < methodInfos.Length && methodInfos[i + 1].Name == method.Name) {
						number = methodNum++.ToString();
					}
					
					string memberFormat = ConvertLink(helpFileFormat.MemberFormat, type, method.Name, "m", number);
					methodsNode.AppendChild(createLinkNode(doc,
					                                       method.Name + "(" + getMethodSignature(method) + ")",
					                                       memberFormat));
				}
			}
			
			if(methodsNode.ChildNodes.Count > 0) {
				rootnode.AppendChild(methodsNode);
			}
			
			// search for properties
			XmlNode propertiesNode = createFolderNode(doc, "Properties");
			SetLink(doc, propertiesNode, ConvertLink(helpFileFormat.PropertiesTopic, type));
			
			foreach(PropertyInfo property in type.GetProperties(flags))
			{
				if (property.DeclaringType == type) {
					string memberFormat = ConvertLink(helpFileFormat.MemberFormat, type, property.Name, "p", "");
					propertiesNode.AppendChild(createLinkNode(doc, property.Name, memberFormat));
				}
			}
			
			if(propertiesNode.ChildNodes.Count > 0) {
				rootnode.AppendChild(propertiesNode);
			}
			
			// search for events
			XmlNode eventsNode = createFolderNode(doc, "Events");
			SetLink(doc, eventsNode, ConvertLink(helpFileFormat.EventsTopic, type));
			
			foreach(EventInfo ev in type.GetEvents(flags))
			{
				if(ev.DeclaringType == type) {
					string memberFormat = ConvertLink(helpFileFormat.MemberFormat, type, ev.Name, "e", "");
					eventsNode.AppendChild(createLinkNode(doc, ev.Name, memberFormat));
				}
			}
			
			if(eventsNode.ChildNodes.Count > 0) {
				rootnode.AppendChild(eventsNode);
			}
			
			return rootnode;
		}
		
		string getMethodSignature(MethodBase method)
		{
			string signature = "";
			foreach(ParameterInfo param in method.GetParameters())
			{
				if(signature != "") signature += ", ";
				signature += param.ParameterType;
			}
			return signature;
		}
	}
}
