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
        [Reactive] public string SearchName { get; set; } = "";
        [Reactive] public bool IsLimited { get; set; }
        [Reactive] public decimal Limit { get; set; } = 10;
        [Reactive] public decimal Page { get; set; } = 1;

        private string _columnIdDefaultText = "Id";
        private string _columnNameDefaultText = "Имя";
        private string _columnBirthdayDefaultText = "День рождения";
        private string _columnPhoneDefaultText = "Телефон";
        [Reactive] public string ColumnIdText { get; set; } //= "Id";
        [Reactive] public string ColumnNameText { get; set; } //= "Имя";
        [Reactive] public string ColumnBirthdayText { get; set; } //= "День рождения";
        [Reactive] public string ColumnPhoneText { get; set; } //= "Телефон";
        [Reactive] public int SortColumnId { get; set; } = 0;
        [Reactive] public bool IsSortAscending { get; set; } = true;


        public ReactiveCommand AddClientsCommand { get; private set; }
        public ReactiveCommand EditClientsCommand { get; private set; }
        public ReactiveCommand DeleteClientsCommand { get; private set; }
        public ReactiveCommand SelectCommand { get; private set; }
        public ReactiveCommand SelectionChangedCommand { get; private set; }
        public ReactiveCommand ColumnClickCommand { get; private set; }

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
                Select();
            });

            ColumnClickCommand = ReactiveCommand.Create<object>(o =>
            {
                var columnId = ((ColumnClickEventArgs) o).Column;

                if (SortColumnId == columnId)
                    IsSortAscending = !IsSortAscending;
                else
                    IsSortAscending = true;

                SortColumnId = columnId;

                // lol, unreadable
                ColumnIdText = SortColumnId == 0
                    ? (IsSortAscending ? $"{_columnIdDefaultText} ↑" : $"{_columnIdDefaultText} ↓")
                    : _columnIdDefaultText;
                ColumnNameText = SortColumnId == 1
                    ? (IsSortAscending ? $"{_columnNameDefaultText} ↑" : $"{_columnNameDefaultText} ↓")
                    : _columnNameDefaultText;
                ColumnBirthdayText = SortColumnId == 2
                    ? (IsSortAscending ? $"{_columnBirthdayDefaultText} ↑" : $"{_columnBirthdayDefaultText} ↓")
                    : _columnBirthdayDefaultText;
                ColumnPhoneText = SortColumnId == 3
                    ? (IsSortAscending ? $"{_columnPhoneDefaultText} ↑" : $"{_columnPhoneDefaultText} ↓")
                    : _columnPhoneDefaultText;

                Select();
            });


            this.WhenAnyValue(x => x.SortColumnId)
                .Subscribe(x =>
                {
                    //_logger.Debug($"{x}");
                });

            this.WhenAnyValue(x => x.IsSortAscending)
                .Subscribe(x =>
                {
                    
                });

            this.WhenAnyValue(x => x.Limit)
                //.Throttle(TimeSpan.FromMilliseconds(250))
                //.DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.Page)
                //.Throttle(TimeSpan.FromMilliseconds(250))
                //.DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.SearchName)
                .Throttle(TimeSpan.FromMilliseconds(150))
                .DistinctUntilChanged()
                .Skip(1)
                .InvokeCommand(SelectCommand);

            this.WhenAnyValue(x => x.IsLimited)
                //.Throttle(TimeSpan.FromMilliseconds(250))
                //.DistinctUntilChanged()
                //.Skip(1)
                .InvokeCommand(SelectCommand);
        }

        private void Select()
        {
            IQueryable<Client> clients = _db.GetTable<Client>()
                .Where(x => x.Name.Contains(SearchName));

            switch (SortColumnId)
            {
                case 0:
                    clients = IsSortAscending
                        ? clients.OrderBy(x => x.Id)
                        : clients.OrderByDescending(x => x.Id);
                    break;
                case 1:
                    clients = IsSortAscending
                        ? clients.OrderBy(x => x.Name)
                        : clients.OrderByDescending(x => x.Name);
                    break;
                case 2:
                    clients = IsSortAscending
                        ? clients.OrderBy(x => x.Birthday)
                        : clients.OrderByDescending(x => x.Birthday);
                    break;
                case 3:
                    clients = IsSortAscending
                        ? clients.OrderBy(x => x.Phone)
                        : clients.OrderByDescending(x => x.Phone);
                    break;
            }

            if (IsLimited)
            {
                clients = clients
                    .Skip((int)(Limit * (Page - 1)))
                    .Take((int)Limit);
            }


            Clients.Clear();
            foreach (var client in clients)
            {
                Clients.AddOrUpdate(client);
            }
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