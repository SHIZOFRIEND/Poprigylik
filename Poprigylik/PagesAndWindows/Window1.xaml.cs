using System;
using System.Windows;
using static Poprigylik.MainWindow;

namespace Poprigylik.PagesAndWindows
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public static bool flag = false; 
        public static int prioritet = 0; 
        public Window1(int pr)
        {
            InitializeComponent();
            priorety.Text = pr.ToString(); 
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(priorety.Text, out int newPriority))
            {
                if (newPriority < 0) 
                {
                    MessageBox.Show("Приоритет не может быть отрицательным. Введите значение больше или равное 0.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }

                helper.flag = true;
                helper.prioritet = newPriority;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите корректное целое число для приоритета.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}
