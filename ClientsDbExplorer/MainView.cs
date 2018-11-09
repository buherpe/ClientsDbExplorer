using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientsDbExplorer.Interfaces;
using ClientsDbExplorer.Models;
using ClientsDbExplorer.ViewModels;
using DynamicData;
using NLog;
using ReactiveUI;
using ReactiveUI.Events;

namespace ClientsDbExplorer
{
    public partial class MainView : Form, IViewFor<MainViewModel>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ReadOnlyObservableCollection<Client> ClientData;

        public MainView()
        {
            InitializeComponent();
            VM = new MainViewModel();

            this.Bind(VM, vm => vm.EditText, v => v.buttonEditClient.Text);
            this.Bind(VM, vm => vm.DeleteText, v => v.buttonDeleteClient.Text);
            this.BindCommand(VM, vm => vm.AddClientsCommand, v => v.buttonAddClient);
            this.BindCommand(VM, vm => vm.EditClientsCommand, v => v.buttonEditClient);
            this.BindCommand(VM, vm => vm.DeleteClientsCommand, v => v.buttonDeleteClient);
            //this.BindCommand(VM, vm => vm.SelectedCommand, v => v.listView1.Events().ItemSelectionChanged);
            listView1.Events().ItemSelectionChanged.InvokeCommand(VM, vm => vm.SelectionChangedCommand);
            
            //listView1.Events().MouseDoubleClick.Subscribe(_logger.Debug);
            //listView1.Events().Enter.Subscribe(_logger.Debug);

            var clientsService = VM.Clients.Connect()
                .Bind(out ClientData)
                .DisposeMany()
                .Subscribe();

            ClientData.ActOnEveryObject(AddClient, DeleteClient);


            VM.GetAllClientTask();
        }

        private void AddClient(Client client)
        {
            string[] row = {$"{client.Id}", client.Name, client.Birthday.ToShortDateString(), client.Phone};
            var item = new ListViewItem(row);
            item.Tag = client;
            item.Name = $"{client.Id}";
            listView1.Items.Add(item);
        }

        private void DeleteClient(Client client)
        {
            listView1.Items.RemoveByKey($"{client.Id}");
        }

        private void DeleteClients(IEnumerable<Client> clients)
        {
            foreach (var client in clients)
            {
                DeleteClient(client);
            }
        }

        public MainViewModel VM { get; set; }

        object IViewFor.ViewModel
        {
            get => VM;
            set => VM = (MainViewModel) value;
        }

        MainViewModel IViewFor<MainViewModel>.ViewModel
        {
            get => VM;
            set => VM = value;
        }
    }
}