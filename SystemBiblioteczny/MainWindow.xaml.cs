using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SystemBiblioteczny.Methods;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny
{

    public partial class MainWindow : Window
    {
        LoginMethod loginMethod = new();
        public MainWindow()
        {
            InitializeComponent();
            base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            clientOption.IsChecked = true;
        }

        private void Register_Client(object sender, RoutedEventArgs e)
        {
            var username = RegisterUsername.Text;
            var password = RegisterPassword.Password;
            var confirmPassword = RegisterPasswordConfirmation.Password;
            var name = RegisterName.Text;
            var lastname = RegisterLastname.Text;
            var email = RegisterEmailAddress.Text;
            var phoneNumber = RegisterPhoneNumber.Text;
            AccountBase.RoleTypeEnum role = AccountBase.RoleTypeEnum.Client;
            LoginMethod l = new();
            AccountBase a = new();
            bool canProceed = l.CheckIfAllDataIsCorrectAndCanCreateAccount(username, password, confirmPassword, name, lastname, email);
            if (canProceed == false) return;
            Client newClient = new(username, password, name, lastname, email, phoneNumber);
            a.AddClientToList(newClient);
            ClientWindow clientwindow = new(newClient);
            clientwindow.Show();
            this.Close();
        }

        private void Sign_Client(object sender, RoutedEventArgs e)
        {
            var username = LoginEmail.Text;
            var password = LoginPassword.Password;

            AccountBase.RoleTypeEnum role = AccountBase.RoleTypeEnum.Client;

            if (clientOption.IsChecked == true) role = AccountBase.RoleTypeEnum.Client;
            if (admin_localOption.IsChecked == true) role = AccountBase.RoleTypeEnum.LocalAdmin;
            if (admin_networkOption.IsChecked == true) role = AccountBase.RoleTypeEnum.NetworkAdmin;
            if (librarianOption.IsChecked == true) role = AccountBase.RoleTypeEnum.Librarian;

            LoginMethod loginMethod = new LoginMethod();
            bool logged = loginMethod.CheckLogin(username, password, role);

            if (logged == true) { this.Close(); }

        }

        private void Hint(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opcjonalnie");
        }

        protected override void OnClosing(CancelEventArgs e) //nie wiem czemu ale nie dziala
        {
            base.OnClosing(e);
        }

        private void RegisterUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegisterUsername.Text = loginMethod.EraseWhiteSpace(RegisterUsername.Text);
        }

        private void RegisterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegisterName.Text = loginMethod.EraseWhiteSpace(RegisterName.Text);
        }

        private void RegisterLastname_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegisterLastname.Text = loginMethod.EraseWhiteSpace(RegisterLastname.Text);
        }

        private void RegisterEmailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegisterEmailAddress.Text = loginMethod.EraseWhiteSpace(RegisterEmailAddress.Text);
        }

        private void RegisterPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegisterPhoneNumber.Text = loginMethod.EraseWhiteSpace(RegisterPhoneNumber.Text);
        }
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            string a = RegisterPassword.Password;
            string b = loginMethod.EraseWhiteSpace(RegisterPassword.Password);
            if (a != b) RegisterPassword.Password = b;
        }
        private void ConfirmationPasswordChanged(object sender, RoutedEventArgs e)
        {
            string a = RegisterPasswordConfirmation.Password;
            string b = loginMethod.EraseWhiteSpace(RegisterPasswordConfirmation.Password);
            if (a != b) RegisterPasswordConfirmation.Password = b;
        }

    }
}