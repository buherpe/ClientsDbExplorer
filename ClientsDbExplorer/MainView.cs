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
using ClientsDbExplorer.Models;
using ClientsDbExplorer.ViewModels;
using DynamicData;
using DynamicData.Binding;
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

            this.WhenActivated(d =>
            {
                d(this.Bind(VM, vm => vm.EditText, v => v.buttonEditClient.Text));
                d(this.Bind(VM, vm => vm.DeleteText, v => v.buttonDeleteClient.Text));
                d(this.Bind(VM, vm => vm.SearchName, v => v.textBoxSearch.Text));
                d(this.Bind(VM, vm => vm.IsLimited, v => v.checkBoxLimit.Checked));

                d(this.Bind(VM, vm => vm.Limit, v => v.numericUpDownLimit.Value));
                d(this.Bind(VM, vm => vm.Page, v => v.numericUpDownPage.Value));

                d(this.Bind(VM, vm => vm.IsLimited, v => v.numericUpDownLimit.Enabled));
                d(this.Bind(VM, vm => vm.IsLimited, v => v.numericUpDownPage.Enabled));

                d(this.Bind(VM, vm => vm.ColumnIdText, v => v.columnHeaderId.Text));
                d(this.Bind(VM, vm => vm.ColumnNameText, v => v.columnHeaderName.Text));
                d(this.Bind(VM, vm => vm.ColumnBirthdayText, v => v.columnHeaderBirthday.Text));
                d(this.Bind(VM, vm => vm.ColumnPhoneText, v => v.columnHeaderPhone.Text));

                d(this.BindCommand(VM, vm => vm.AddClientsCommand, v => v.buttonAddClient));
                d(this.BindCommand(VM, vm => vm.EditClientsCommand, v => v.buttonEditClient));
                d(this.BindCommand(VM, vm => vm.DeleteClientsCommand, v => v.buttonDeleteClient));

                d(listView1.Events().ItemSelectionChanged.InvokeCommand(VM, vm => vm.SelectionChangedCommand));
                d(listView1.Events().MouseDoubleClick.InvokeCommand(VM, vm => vm.EditClientsCommand));
                d(listView1.Events().ColumnClick.InvokeCommand(VM, vm => vm.ColumnClickCommand));

                var clientsService = VM.Clients.Connect()
                    //.Sort(SortExpressionComparer<Client>.Ascending(t => t.Id),
                    //    SortOptimisations.ComparesImmutableValuesOnly, 25)
                    .ObserveOn(listView1)
                    .Bind(out ClientData)
                    .DisposeMany()
                    .Subscribe();

                d(clientsService);

                ClientData.ActOnEveryObject(AddClient, DeleteClient);
            });
        }

        private void AddClient(Client client)
        {
            string phone;
            switch (client.Phone.Length)
            {
                case 5:
                    phone = $"{int.Parse(client.Phone):#-##-##}";
                    break;
                case 6:
                    phone = $"{int.Parse(client.Phone):##-##-##}";
                    break;
                case 7:
                    phone = $"{int.Parse(client.Phone):###-##-##}";
                    break;
                case 11:
                    phone = $"{long.Parse(client.Phone):# (###) ###-####}";
                    break;
                default:
                    phone = client.Phone;
                    break;
            }

            string[] row =
                {$"{client.Id}", client.Name, client.Birthday.ToShortDateString(), phone};
            var item = new ListViewItem(row);
            item.Tag = client;
            item.Name = $"{client.Id}";
            listView1.Items.Add(item);
        }

        private void DeleteClient(Client client)
        {
            listView1.Items.RemoveByKey($"{client.Id}");
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