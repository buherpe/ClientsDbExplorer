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
        //private IDb _db;

        // These strings can be localized (need a little more time for it)
        //private const string TextBoxNameText = "Ivan Ivanovich Ivanov";
        //private const string TextBoxPhoneText = "79876543210";

        //public List<Client> SelectedClients { get; set; }

        //public SourceCache<Client, int> Clients;
        public ReadOnlyObservableCollection<Client> ClientData;

        //private SourceCache<Client, int> Clients; // = new SourceCache<Client, int>(x => x.Id);

        public MainView()
        {
            InitializeComponent();
            VM = new MainViewModel();
            //textBoxName.Text = TextBoxNameText;
            //textBoxPhone.Text = TextBoxPhoneText;


            this.Bind(VM, vm => vm.EditText, v => v.buttonEditClient.Text);
            this.Bind(VM, vm => vm.DeleteText, v => v.buttonDeleteClient.Text);
            //this.Bind(VM, vm => vm.Clients, v => v.Clients);
            //this.Bind(VM, x => x.Status, x => x.toolStripStatusLabel1.Text);
            //this.Bind(VM, vm => vm.SelectedClient, v => (Client) v.listView1.SelectedItems[0].Tag);
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



            //_db = new Db(@"Data Source=""(local)"";Initial Catalog=clientsdb;Integrated Security=True");


            //var clients = _db.Context().GetTable<Client>();


            //foreach (var client in clients)
            //{
            //    //_logger.Debug($"{client.Id} {client.Name} {client.Birthday} {client.Phone}");

            //    AddClient(client);
            //}
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var listView = (ListView) sender;
            //var selectedCount = listView.SelectedItems.Count;
            //toolStripStatusLabel1.Text = $"{selectedCount} item{(selectedCount > 1 ? "s" : "")} selected";

            //if (selectedCount == 0)
            //{
            //    SelectedClients = null;
            //}
            //else if (selectedCount == 1)
            //{
            //    var client = (Client) listView.SelectedItems[0].Tag;
            //    var clients = new List<Client> {client};
            //    SelectedClients = clients;
            //}
            //else
            //{
            //    List<Client> clients = new List<Client>();
            //    foreach (ListViewItem listViewSelectedItem in listView.SelectedItems)
            //    {
            //        clients.Add((Client) listViewSelectedItem.Tag);
            //    }

            //    SelectedClients = clients;
            //}

        }

        private void deleteItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (SelectedClients == null) return;

            //_db.Context().GetTable<Client>().DeleteAllOnSubmit(SelectedClients);
            //_db.Context().SubmitChanges();


            //DeleteClients(SelectedClients);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //deleteItemsToolStripMenuItem.Enabled = SelectedClients != null;
        }

        //private void buttonAddClient_Click(object sender, EventArgs e)
        //{
        //    var client = new Client { Name = textBoxName.Text, Birthday = dateTimePicker1.Value, Phone = textBoxPhone.Text};

        //    _db.Context().GetTable<Client>().InsertOnSubmit(client);
        //    _db.Context().SubmitChanges();

        //    AddClient(client);

        //    textBoxName.Clear();
        //    textBoxPhone.Clear();
        //    //ShowPlaceholder(textBoxName, TextBoxNameText);
        //    //ShowPlaceholder(textBoxPhone, TextBoxPhoneText);
        //}

        //private void ValidateClientVars(string name, DateTime birthday, string phone)
        //{
        //    buttonAddClient.Enabled = false;

        //    if (string.IsNullOrWhiteSpace(name)) return;
        //    if (string.IsNullOrWhiteSpace(phone)) return;

        //    // Переделать костыли
        //    if (/*name == TextBoxNameText &&*/
        //        textBoxName.ForeColor == Color.Gray)
        //    {
        //        return;
        //    }

        //    if (phone == TextBoxPhoneText &&
        //        textBoxPhone.ForeColor == Color.Gray)
        //    {
        //        return;
        //    }

        //    // Тут можно проверять копипастнутый номер телефона,
        //    // если он, например, со скобками и тире
        //    // var m = new Regex("<some_number_regex_pattern>").Match(phone);
        //    // if (m.Success)

        //    buttonAddClient.Enabled = true;
        //}

        //private void textBoxName_TextChanged(object sender, EventArgs e)
        //{
        //    ValidateClientVars(textBoxName.Text, dateTimePicker1.Value, textBoxPhone.Text);
        //}

        //private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        //{
        //    ValidateClientVars(textBoxName.Text, dateTimePicker1.Value, textBoxPhone.Text);
        //}

        //private void textBoxPhone_TextChanged(object sender, EventArgs e)
        //{
        //    ValidateClientVars(textBoxName.Text, dateTimePicker1.Value, textBoxPhone.Text);
        //}

        #region TextBox's placeholder

        //private void ShowPlaceholder(TextBox textBox, string text)
        //{
        //    if (string.IsNullOrWhiteSpace(textBox.Text))
        //    {
        //        textBox.Text = text;
        //        textBox.ForeColor = Color.Gray;
        //    }
        //}

        //private void HidePlaceholder(TextBox textBox, string text)
        //{
        //    if (textBox.Text == text && textBox.ForeColor == Color.Gray)
        //    {
        //        textBox.Clear();
        //        textBox.ForeColor = DefaultForeColor;
        //    }
        //}

        //private void textBoxName_Enter(object sender, EventArgs e)
        //{
        //    HidePlaceholder(textBoxName, TextBoxNameText);
        //}

        //private void textBoxName_Leave(object sender, EventArgs e)
        //{
        //    ShowPlaceholder(textBoxName, TextBoxNameText);
        //}

        //private void textBoxPhone_Enter(object sender, EventArgs e)
        //{
        //    HidePlaceholder(textBoxPhone, TextBoxPhoneText);
        //}

        //private void textBoxPhone_Leave(object sender, EventArgs e)
        //{
        //    ShowPlaceholder(textBoxPhone, TextBoxPhoneText);
        //}

        #endregion

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