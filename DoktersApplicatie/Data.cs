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

            clients.Add(new Client { Name = "Kapil Malhotra", Age = 30, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Raj Kundra", Age = 34, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Amitabh Bachan", Age = 80, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
            clients.Add(new Client { Name = "Deepak Khanna", Age = 72, BPM = 52, RPM = 60, KMH = 35.0, CurrWatt = 200, Distance = 2.5, AccWatt = 400, SessionTime = 100, Resistance = 40 });
        }

    }

    public class Client
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int BPM { get; set; }
        public int RPM { get; set; }
        public double KMH { get; set; }
        public int CurrWatt { get; set; }
        public double Distance { get; set; }
        public int AccWatt { get; set; }
        public int SessionTime { get; set; }
        public int Resistance { get; set; }

        public string AccWattText
        {
            get
            {
                return "Accumulated Watt: " + AccWatt;
            }
        }
        public string DistanceText
        {
            get
            {
                return "Distance: " + Distance;
            }
        }
        public string SessionTimeText
        {
            get
            {
                return "Session Time: " + SessionTime;
            }
        }
        public string ResistanceText
        {
            get
            {
                return "Resistance: " + Resistance;
            }
        }

    }

    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
    }

}
