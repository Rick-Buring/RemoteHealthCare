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

            clients.Add(new Client { Name = "Kapil Malhotra", Age = 30 });
            clients.Add(new Client { Name = "Raj Kundra", Age = 34 });
            clients.Add(new Client { Name = "Amitabh Bachan", Age = 80 });
            clients.Add(new Client { Name = "Deepak Khanna", Age = 72 });
        }

    }

    public class Client
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Text { get; set; }
    }

}
