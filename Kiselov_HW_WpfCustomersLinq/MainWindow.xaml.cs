using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Kiselov_HW_WpfCustomersLinq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClassesCustomersDataContext _context = new DataClassesCustomersDataContext();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method starts searching of elements with equal first and last letters in client's name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLetterSearch_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxFirstLetter.Text) || string.IsNullOrEmpty(TextBoxFirstLetter.Text))
                {
                    throw new Exception("Enter the correct first letter");
                }
                if (string.IsNullOrEmpty(TextBoxLastLetter.Text) || string.IsNullOrEmpty(TextBoxLastLetter.Text))
                {
                    throw new Exception("Enter the correct last letter");
                }
                List<Customer> context = (from X in _context.Customers
                    where X.name.ToLower()[0].Equals(TextBoxFirstLetter.Text.ToLower()[0])
                          && X.name.ToLower()[X.name.Length - 1].Equals(TextBoxLastLetter.Text.ToLower()[0])
                    select X).ToList();

                DataGridLetters.DataContext = context;

                DataSourceToXElement(context);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method returns all clients, who have salary which is smaller than average 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAverage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridAverage.DataContext = from X in _context.Customers
                    where X.account < (from Y in _context.Customers select Y.account).Average()
                    select X;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method updates client's accounts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAccountSearch_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                (from X in _context.Customers
                    where X.account == decimal.Parse(TextBoxComparable.Text)
                    select X).ToList().ForEach(X => X.account = decimal.Parse(TextBoxNewValue.Text));

                _context.SubmitChanges();
                DataGridAccount.DataContext = _context.Customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method deletes clients with direct accounts 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDigitSearch_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] separator = TextBoxDigit.Text.Split(',');
                var accountToDelete = new List<Customer>();

                foreach (var digit in separator)
                {
                    accountToDelete.AddRange(
                        from X in _context.Customers
                        where X.account.ToString().Contains(digit)
                        select X
                        );
                }

                _context.Customers.DeleteAllOnSubmit(accountToDelete);
                _context.SubmitChanges();
                DataGridDigit.DataContext = _context.Customers;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method converts existing database into XML document
        /// </summary>
        /// <param name="listOfCustomers"></param>
        private void DataSourceToXElement(List<Customer> listOfCustomers)
        {
            XElement root = new XElement("Customers");
            try
            {
                for (int i = 0; i < listOfCustomers.Count; i++)
                {
                    root.Add(new XElement(string.Format("Customer" + listOfCustomers[i].Id.ToString()),
                        new XElement("Name", listOfCustomers[i].name)));
                    root.Element(string.Format("Customer" + listOfCustomers[i].Id.ToString()))
                        .Add(new XElement("Account", listOfCustomers[i].account));
                }
                root.Save("1.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}