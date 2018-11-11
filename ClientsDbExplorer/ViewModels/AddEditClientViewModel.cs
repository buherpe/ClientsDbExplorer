using System;
using System.Reactive.Disposables;
using System.Windows.Forms;
using ClientsDbExplorer.Models;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ClientsDbExplorer.ViewModels
{
    public class AddEditClientViewModel : ReactiveObject, IDisposable
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDisposable _cleanup;

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
            
            var nameSub = this.WhenAnyValue(x => x.Name).Subscribe(c => Client.Name = c.Trim());
            var bdSub = this.WhenAnyValue(x => x.Birthday).Subscribe(c => Client.Birthday = c);
            var phoneSub = this.WhenAnyValue(x => x.Phone).Subscribe(c => Client.Phone = c.Trim());
            
            _cleanup = new CompositeDisposable(nameSub, bdSub, phoneSub);
        }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}