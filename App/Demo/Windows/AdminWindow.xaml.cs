using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo
{
    public partial class AdminWindow : Window
    {
        Demo_Polzovateli selected;

        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            GridUsers.ItemsSource = user182_dbEntities.GetContext().Demo_Polzovateli.ToList();
            RoleCB.ItemsSource = user182_dbEntities.GetContext().Demo_Roli.ToList();
        }

        private void GridUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = GridUsers.SelectedItem as Demo_Polzovateli;
            if (selected == null) return;

            LogTB.Text = selected.login_polzovatelya;
            PassTB.Text = selected.parol;
            NameTB.Text = selected.imya;
            LastTB.Text = selected.familiya;
            PatrTB.Text = selected.otchestvo;
            RoleCB.SelectedValue = selected.id_roli;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LogTB.Text) || RoleCB.SelectedValue == null) return;

            user182_dbEntities.GetContext().Demo_Polzovateli.Add(new Demo_Polzovateli
            {
                login_polzovatelya = LogTB.Text,
                parol = PassTB.Text,
                imya = NameTB.Text,
                familiya = LastTB.Text,
                otchestvo = PatrTB.Text,
                id_roli = (int)RoleCB.SelectedValue,
                zablokirovan = false
            });
            SaveAndRefresh();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (selected == null) return;
            selected.login_polzovatelya = LogTB.Text; selected.parol = PassTB.Text;
            selected.imya = NameTB.Text; selected.familiya = LastTB.Text;
            selected.otchestvo = PatrTB.Text; selected.id_roli = (int)RoleCB.SelectedValue;
            SaveAndRefresh();
        }

        private void Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null) { 
                selected.zablokirovan = false; 
                SaveAndRefresh(); 
                LoadData();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LogTB.Text = PassTB.Text = NameTB.Text = LastTB.Text = PatrTB.Text = "";
            RoleCB.SelectedIndex = -1; 
            selected = null; 
            GridUsers.SelectedItem = null;
        }

        private void SaveAndRefresh()
        {
            user182_dbEntities.GetContext().SaveChanges();
            LoadData();
            Clear_Click(null, null);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show(); Close();
        }
    }
}