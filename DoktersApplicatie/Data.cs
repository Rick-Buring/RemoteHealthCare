using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DoktersApplicatie
{
    class Data
    {

        public ObservableCollection<Client> clients { get; set; }
        public ObservableCollection<Message> messages { get; set; }

        public Data()
        {
            clients = new ObservableCollection<Client>();
            messages = new ObservableCollection<Message>();

            clients.Add(new Client { Name = "Kapil Malhotra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 50 });
            clients.Add(new Client { Name = "Raj Kundra", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 20 });
            clients.Add(new Client { Name = "Amitabh Bachan", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 30 });
            clients.Add(new Client { Name = "Deepak Khanna", BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
        }

    }

    public class Client
    {
        public string Name { get; set; }
        public int BPM { get; set; }
        public int RPM { get; set; }
        public double KMH { get; set; }
        public int CurrWatt { get; set; }
        public double Distance { get; set; }
        public int AccWatt { get; set; }
        public int SessionTime { get; set; }
        public int Resistance { get; set; }
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
    }

}
