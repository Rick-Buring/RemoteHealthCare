using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CommunicationObjects.DataObjects;
using DoktersApplicatie.ViewModels;

namespace DoktersApplicatie.Commands
{
    public class AsyncLoginCommand<T> : AsyncCommandBase where T : ViewModels.ViewModelBase
    {
        private MainViewModel.NavigateDelegate navigate;
        private ClientHandler clientHandler;
        private LoginVM viewModel;

        public AsyncLoginCommand(MainViewModel.NavigateDelegate navigate, LoginVM viewModel, Action<Exception> onException) : base(onException)
        {
            this.navigate = navigate;
            this.viewModel = viewModel;
            this.clientHandler= new ClientHandler();

        }

        protected async override Task ExecuteAsync(object parameter)
        {
            await Login();
        }

        private async Task Login()
        {
            viewModel.Message = "";

            if (viewModel.Name == null || viewModel.Name.Length < 1 || viewModel.Password == null || viewModel.Password.Length < 1)
            {
                viewModel.Message = "Login or password have to be filled in";
                viewModel.MessageIsError = true;
                return;
            }

            viewModel.MessageIsError = false;
            viewModel.Message = "Login succesfull!";

            string[] serverAddres = viewModel.ServerAddres.Split(":");

            try
            {
                await clientHandler.StartConnection(serverAddres[0], Int32.Parse(serverAddres[1]));

            }catch(Exception ex)
            {
                Debug.WriteLine(ex);
                viewModel.Message = "Server not found";
                viewModel.MessageIsError = true;
                return;
            }


            if (await clientHandler.Login(viewModel.Name, viewModel.Password))
            {
                navigate(new HomeVM(navigate, clientHandler));
            }
            else
            {
                viewModel.Message = "Not authorized";
                viewModel.MessageIsError = true;
            }

        }
    }
}

