namespace ClientsDbExplorer
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBirthday = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPhone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonAddClient = new System.Windows.Forms.Button();
            this.buttonEditClient = new System.Windows.Forms.Button();
            this.buttonDeleteClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderName,
            this.columnHeaderBirthday,
            this.columnHeaderPhone});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 41);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(497, 380);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderId
            // 
            this.columnHeaderId.Text = "Id";
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 150;
            // 
            // columnHeaderBirthday
            // 
            this.columnHeaderBirthday.Text = "Birthday";
            this.columnHeaderBirthday.Width = 80;
            // 
            // columnHeaderPhone
            // 
            this.columnHeaderPhone.Text = "Phone";
            this.columnHeaderPhone.Width = 100;
            // 
            // buttonAddClient
            // 
            this.buttonAddClient.Location = new System.Drawing.Point(12, 12);
            this.buttonAddClient.Name = "buttonAddClient";
            this.buttonAddClient.Size = new System.Drawing.Size(119, 23);
            this.buttonAddClient.TabIndex = 6;
            this.buttonAddClient.Text = "Добавить клиента";
            this.buttonAddClient.UseVisualStyleBackColor = true;
            // 
            // buttonEditClient
            // 
            this.buttonEditClient.Location = new System.Drawing.Point(137, 12);
            this.buttonEditClient.Name = "buttonEditClient";
            this.buttonEditClient.Size = new System.Drawing.Size(176, 23);
            this.buttonEditClient.TabIndex = 7;
            this.buttonEditClient.Text = "Редактировать 0 клиента(ов)";
            this.buttonEditClient.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteClient
            // 
            this.buttonDeleteClient.Location = new System.Drawing.Point(319, 12);
            this.buttonDeleteClient.Name = "buttonDeleteClient";
            this.buttonDeleteClient.Size = new System.Drawing.Size(150, 23);
            this.buttonDeleteClient.TabIndex = 8;
            this.buttonDeleteClient.Text = "Удалить 0 клиента(ов)";
            this.buttonDeleteClient.UseVisualStyleBackColor = true;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 421);
            this.Controls.Add(this.buttonDeleteClient);
            this.Controls.Add(this.buttonEditClient);
            this.Controls.Add(this.buttonAddClient);
            this.Controls.Add(this.listView1);
            this.Name = "MainView";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderBirthday;
        private System.Windows.Forms.ColumnHeader columnHeaderPhone;
        private System.Windows.Forms.ColumnHeader columnHeaderId;
        private System.Windows.Forms.Button buttonAddClient;
        private System.Windows.Forms.Button buttonEditClient;
        private System.Windows.Forms.Button buttonDeleteClient;
    }
}

