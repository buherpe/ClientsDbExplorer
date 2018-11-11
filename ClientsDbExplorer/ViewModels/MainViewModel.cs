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
using ReactiveUI.Fody.Helpers;

namespace ClientsDbExplorer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [Reactive]
        public string ConnectionString { get; set; } =
            @"Data Source=""(local)"";Initial Catalog=clientsdb;Integrated Security=True";

        private DataContext _db;

        public SourceCache<Client, int> Clients = new SourceCache<Client, int>(x => x.Id);
        public SourceCache<Client, int> SelectedClients = new SourceCache<Client, int>(x => x.Id);

        [Reactive] public int SelectedCount { get; set; }
        [Reactive] public string EditText { get; set; } = "Редактировать 0 клиентов";
        [Reactive] public string DeleteText { get; set; } = "Удалить 0 клиентов";
        [Reactive] public string SearchName { get; set; }
        [Reactive] public bool IsLimited { get; set; }
        [Reactive] public decimal Limit { get; set; } = 10;
        [Reactive] public decimal Page { get; set; } = 1;


        public ReactiveCommand AddClientsCommand { get; private set; }
        public ReactiveCommand EditClientsCommand { get; private set; }
        public ReactiveCommand DeleteClientsCommand { get; private set; }

        public ReactiveCommand SelectCommand { get; private set; }

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

            EditClientsCommand = ReactiveCommand.Create<object>(obj =>
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

            SelectCommand = ReactiveCommand.Create<object>(o =>
            {
                IQueryable<Client> clients;
                clients = _db.GetTable<Client>()
                    .Where(x => x.Name.Contains(SearchName))
                    .OrderBy(x => x.Id);

                if (IsLimited)
                {
                    clients = clients
                        .Skip((int) (Limit * (Page - 1)))
                        .Take((int) Limit);
                }

                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.AddOrUpdate(client);
                }
            });


            this.WhenAnyValue(x => x.Limit)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.Page)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.SearchName)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.IsLimited)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .DistinctUntilChanged()
                //.Skip(1)
                .InvokeCommand(SelectCommand);
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
                clientDb = new Client {Name = client.Name, Birthday = client.Birthday, Phone = client.Phone};
                _db.GetTable<Client>().InsertOnSubmit(clientDb);
            }

            _db.SubmitChanges();
            Clients.AddOrUpdate(clientDb);
        }
    }
}