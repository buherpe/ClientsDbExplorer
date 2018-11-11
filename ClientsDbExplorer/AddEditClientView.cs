using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientsDbExplorer.Models;
using ClientsDbExplorer.ViewModels;
using NLog;
using ReactiveUI;

namespace ClientsDbExplorer
{
    public partial class AddEditClientView : Form, IViewFor<AddEditClientViewModel>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public AddEditClientView(Client client)
        {
            InitializeComponent();
            VM = new AddEditClientViewModel(client);
            
            this.WhenActivated(d =>
            {
                d(this.Bind(VM, vm => vm.Name, v => v.textBoxName.Text));
                d(this.Bind(VM, vm => vm.Birthday, v => v.dateTimePickerBirthday.Value));
                d(this.Bind(VM, vm => vm.Phone, v => v.maskedTextBoxPhone.Text));
            });
        }

        public AddEditClientViewModel VM { get; set; }

        object IViewFor.ViewModel
        {
            get => VM;
            set => VM = (AddEditClientViewModel) value;
        }

        AddEditClientViewModel IViewFor<AddEditClientViewModel>.ViewModel
        {
            get => VM;
            set => VM = value;
        }
    }
}