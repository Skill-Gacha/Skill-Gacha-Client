using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
	public bool IsConnected => _session.IsConnected;
	ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void Init()
	{
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}
	
	public void Init(string ipString, string portString)
	{
		IPAddress ipAddr = null;
		if (IPAddress.TryParse(ipString, out ipAddr) == false)
		{
			// DNS (Domain Name System)
			// string host = Dns.GetHostName();
			// IPHostEntry ipHost = Dns.GetHostEntry(host);
			// ipAddr = ipHost.AddressList[0];

			ipAddr = IPAddress.Parse("127.0.0.1");
		}
		
		int port; 
		if(int.TryParse(portString, out port) == false)
			port = 3000;
		
		IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}
}
