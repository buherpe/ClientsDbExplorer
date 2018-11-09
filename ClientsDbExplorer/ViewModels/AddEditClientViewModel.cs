using System;
using System.Windows.Forms;
using ClientsDbExplorer.Models;
using NLog;
using ReactiveUI;

namespace ClientsDbExplorer.ViewModels
{
    public class AddEditClientViewModel : ReactiveObject
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private DateTime _birthday = DateTime.Now;

        public DateTime Birthday
        {
            get => _birthday;
            set => this.RaiseAndSetIfChanged(ref _birthday, value);
        }

        private string _phone;

        public string Phone
        {
            get => _phone;
            set => this.RaiseAndSetIfChanged(ref _phone, value);
        }

        public Client Client { get; set; }

        public AddEditClientViewModel(Client client)
        {
            Client = client ?? new Client {Birthday = DateTime.Now};

            Name = Client.Name;
            Birthday = Client.Birthday;
            Phone = Client.Phone;

            this.WhenAnyValue(x => x.Name).Subscribe(c => Client.Name = c);
            this.WhenAnyValue(x => x.Birthday).Subscribe(c => Client.Birthday = c);
            this.WhenAnyValue(x => x.Phone).Subscribe(c => Client.Phone = c);
        }
    }
}