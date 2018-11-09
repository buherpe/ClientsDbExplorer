using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ClientsDbExplorer.Models;
using DynamicData;
using NLog;
using ReactiveUI;

namespace ClientsDbExplorer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        //private string _modelString = "";
        //public string EnteredText
        //{
        //    get => _modelString;
        //    set => this.RaiseAndSetIfChanged(ref _modelString, value);
        //}

        //private string _statusString = "";
        //public string Status
        //{
        //    get => _statusString;
        //    set => this.RaiseAndSetIfChanged(ref _statusString, value);
        //}

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private string _connectionString =
            @"Data Source=""(local)"";Initial Catalog=clientsdb;Integrated Security=True";

        public string ConnectionString
        {
            get => _connectionString;
            set => this.RaiseAndSetIfChanged(ref _connectionString, value);
        }

        private DataContext _db;

        public SourceCache<Client, int> Clients = new SourceCache<Client, int>(x => x.Id);
        public SourceCache<Client, int> SelectedClients = new SourceCache<Client, int>(x => x.Id);
        
        private int _selectedCount;

        public int SelectedCount
        {
            get => _selectedCount;
            set => this.RaiseAndSetIfChanged(ref _selectedCount, value);
        }

        private string _editText = "Редактировать 0 клиента(ов)";

        public string EditText
        {
            get => _editText;
            set => this.RaiseAndSetIfChanged(ref _editText, value);
        }

        private string _deleteText = "Удалить 0 клиента(ов)";

        public string DeleteText
        {
            get => _deleteText;
            set => this.RaiseAndSetIfChanged(ref _deleteText, value);
        }

        public ReactiveCommand AddClientsCommand { get; private set; }
        public ReactiveCommand EditClientsCommand { get; private set; }
        public ReactiveCommand DeleteClientsCommand { get; private set; }

        public ReactiveCommand<ListViewItemSelectionChangedEventArgs, Unit> SelectionChangedCommand
        {
            get;
            private set;
        }

        public MainViewModel()
        {
            _db = new DataContext(ConnectionString);
            AddClientsCommand = ReactiveCommand.Create(() =>
            {
                var form = new AddEditClientView(null);
                DialogResult dr = form.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    var clientForm = form.VM.Client;
                    AddOrUpdate(clientForm);
                }
            });

            EditClientsCommand = ReactiveCommand.Create(() =>
            {
                foreach (var client in SelectedClients.Items)
                {
                    var form = new AddEditClientView(client);
                    DialogResult dr = form.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        var clientForm = form.VM.Client;
                        AddOrUpdate(clientForm);
                    }
                }
            }, this.WhenAny(vm => vm.SelectedCount, s => s.Value != 0));

            DeleteClientsCommand = ReactiveCommand.Create(() =>
            {
                _db.GetTable<Client>().DeleteAllOnSubmit(SelectedClients.Items);
                _db.SubmitChanges();
                Clients.Remove(SelectedClients.Items);
            }, this.WhenAny(vm => vm.SelectedCount, s => s.Value != 0));

            SelectionChangedCommand = ReactiveCommand.Create<ListViewItemSelectionChangedEventArgs>(x =>
            {
                var client = (Client) x.Item.Tag;
                if (x.IsSelected)
                    SelectedClients.AddOrUpdate(client);
                else
                    SelectedClients.Remove(client);
                SelectedCount = SelectedClients.Count;

                var decl = Helpers.Declination.GetClientDeclension(SelectedCount);
                EditText = $"Редактировать {decl}";
                DeleteText = $"Удалить {decl}";
            });
        }

        private void AddOrUpdate(Client client)
        {
            var clientDb = _db.GetTable<Client>().FirstOrDefault(x => x.Id == client.Id);
            if (clientDb != null)
            {
                clientDb.Name = client.Name;
                clientDb.Birthday = client.Birthday;
                clientDb.Phone = client.Phone;
            }
            else
            {
                clientDb = new Client { Name = client.Name, Birthday = client.Birthday, Phone = client.Phone};
                _db.GetTable<Client>().InsertOnSubmit(clientDb);
            }

            _db.SubmitChanges();
            Clients.AddOrUpdate(clientDb);
        }

        private IEnumerable<Client> GetAllClients()
        {
            return _db.GetTable<Client>();
        }

        public Task GetAllClientTask()
        {
            //await Task.Delay(1000);
            foreach (var client in GetAllClients())
            {
                Clients.AddOrUpdate(client);
            }

            return Task.CompletedTask;
        }
    }
}