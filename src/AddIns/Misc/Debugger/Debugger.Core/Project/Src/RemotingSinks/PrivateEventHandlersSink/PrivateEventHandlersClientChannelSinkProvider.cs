// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace CustomSinks
{
	class PrivateEventHandlersClientChannelSinkProvider: IClientChannelSinkProvider 
	{
		private IClientChannelSinkProvider nextProvider;

		public PrivateEventHandlersClientChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink nextSink = nextProvider.CreateSink(channel, url, remoteChannelData);
			IClientChannelSink thisSink = new PrivateEventHandlersClientChannelSink(nextSink as IClientFormatterSink);
			return thisSink;
		}

		IClientChannelSinkProvider IClientChannelSinkProvider.Next {
			get	{
				return nextProvider;
			}
			set	{
				nextProvider = value;
			}
		}
	}
}
