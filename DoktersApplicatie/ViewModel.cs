using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace DoktersApplicatie
{
    class ViewModel : BindableBase, INotifyPropertyChanged
    {

        public DelegateCommand<object> cMoreInfo { get; private set; }
        public DelegateCommand<object> cStartStopSession { get; private set; }
        public DelegateCommand<object> cEmergencyStop { get; private set; }
        public DelegateCommand<object> cSendMessage { get; private set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; }
        public int ActiveSecondaryTab { get; set; }
        public int ActiveMainTab { get; set; }

        public ViewModel()
        {
            Data data = new Data();

            this.Clients = data.clients;
            this.Messages = data.messages;

            cMoreInfo = new DelegateCommand<object>(MoreInfo, canSubmit);
            cStartStopSession = new DelegateCommand<object>(StartStopSession, canSubmit);
            cEmergencyStop = new DelegateCommand<object>(EmergencyStop, canSubmit);
            cSendMessage = new DelegateCommand<object>(SendMessage, canSubmit);
        }

        public bool canSubmit(object parameter)
        {
            return true;
        }

        public void MoreInfo(object name)
        {
            Debug.WriteLine(name);
            ActiveMainTab = 1;

            foreach (var client in Clients)
            {
                if (client.Name.Equals(name))
                {
                    ActiveSecondaryTab = Clients.IndexOf(client);
                }
            }

            //Debug.WriteLine(ActiveMainTab + " : " + ActiveSecondaryTab);
        }

        public void StartStopSession(object parameter)
        {
            Debug.WriteLine("Started/Stopped session");
        }

        public void EmergencyStop(object parameter)
        {
            Debug.WriteLine("Emergency Stop");
        }

        public void SendMessage(object messageBox)
        {
            TextBox textbox = (TextBox)messageBox;

            if(textbox.Text != "")
            {
                Messages.Add(new Message { Sender = "Doctor", Text = textbox.Text });
                textbox.Clear();
                Debug.WriteLine(textbox.Text);
            }
            
        }

    }
}
