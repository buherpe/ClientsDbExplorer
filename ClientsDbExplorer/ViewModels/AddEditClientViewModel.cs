using System;
using System.Windows.Forms;
using ClientsDbExplorer.Models;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ClientsDbExplorer.ViewModels
{
    public class AddEditClientViewModel : ReactiveObject
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [Reactive] public string Name { get; set; }
        [Reactive] public DateTime Birthday { get; set; } = DateTime.Now;
        [Reactive] public string Phone { get; set; }

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