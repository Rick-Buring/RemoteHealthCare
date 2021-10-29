using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
	public class ConnectToServerVM : BaseViewModel
	{
		private Thread serverConnectionThread;
		private VrManager vr;

		public ClientHandler Client { get; private set; }
		private EquipmentMain eq;
		public DelegateCommand ConnectToServer { get; }
		

		public ConnectToServerVM(ClientHandler client, EquipmentMain equipment, VrManager vr)
		{
			this.vr = vr;
			this.Client = client;
			this.eq = equipment;
			this.ConnectToServer = new DelegateCommand(EngageConnection);
			//requestResistance += this.eq.ergometer.SendResistance;
		}

		public string PortNumber { get; set; } = "5005";
		public string IPAddress { get; set; } = "localHost";

		private void EngageConnection()
		{
			this.serverConnectionThread = new Thread(() => Client.StartConnection(this.IPAddress, Int32.Parse(this.PortNumber)));
			this.serverConnectionThread.Start();
			ViewModel.resistanceUpdater += this.eq.Ergometer.SendResistance;
			RaiseOnNavigate(new ConnectedVM(Client, vr, eq));
		}


		public override void Dispose()
		{
			if (serverConnectionThread != null && serverConnectionThread.IsAlive)
			{
				serverConnectionThread.Abort();
			}

		}
	}
}
