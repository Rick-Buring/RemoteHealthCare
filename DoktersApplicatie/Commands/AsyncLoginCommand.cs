using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunicationObjects.DataObjects;
using DoktersApplicatie.ViewModels;

namespace DoktersApplicatie.Commands
{
    public class AsyncLoginCommand<T> : AsyncCommandBase where T : ViewModels.ViewModelBase
    {
        private MainViewModel.NavigateDelegate navigate;
        private LoginVM viewModel;

        public AsyncLoginCommand(MainViewModel.NavigateDelegate navigate, LoginVM viewModel, Action<Exception> onException) : base(onException)
        {
            this.navigate = navigate;
            this.viewModel = viewModel;
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

            HomeVM homeVM = new HomeVM(navigate);
            ClientHandler clientHandler = new ClientHandler(homeVM.clientReceived, homeVM.updateClient, homeVM.updateHistory, viewModel.Name, viewModel.Password);
            homeVM.setClientHandler(clientHandler);

            await clientHandler.StartConnection("LocalHost", 6006);

            Acknowledge a = await clientHandler.Login();
            if (a.status == 200)
            {
                homeVM.StartClientHandler();
                navigate(homeVM);
            }
            else
            {
                viewModel.Message = "Not authorized";
                viewModel.MessageIsError = true;
            }

        }
    }
}

