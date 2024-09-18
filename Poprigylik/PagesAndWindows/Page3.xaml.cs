using Poprigylik.BazaDan;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Poprigylik.MainWindow;
namespace Poprigylik.PagesAndWindows
{
    /// <summary>
    /// Логика взаимодействия для Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        Agent agent;
        private int curSelPr;
        private int curTypAg;
        public Page3(Agent ag)
        {
            InitializeComponent();
            try
            {
                Type.ItemsSource = helper.GetContext().AgentType.ToList();
                product.ItemsSource = helper.GetContext().Product.ToList();
            }
            catch { };
            if (ag != null)
            {
                agent = ag;
                Type.SelectedItem = ag.AgentType;
                this.Title.Text = ag.Title;
                this.Adress.Text = ag.Address;
                this.Inn.Text = ag.INN;
                this.Kpp.Text = ag.KPP;
                this.Director.Text = ag.DirectorName;
                this.Phone.Text = ag.Phone;
                this.Emale.Text = ag.Email;
                this.Logo.Text = ag.Logo;
                this.Prioritet.Text = ag.Priority.ToString();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == ag.ID).ToList();
            }
            else
            {
                agent = new Agent();
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
            }
            this.DataContext = agent;
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Type.SelectedItem != null)
            {
                var selectedType = Type.SelectedItem as AgentType;
                if (selectedType != null)
                {
                    curTypAg = selectedType.ID;
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (curTypAg == 0)
            {
                MessageBox.Show("Пожалуйста, выберите тип агента.");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.Title.Text))
            {
                MessageBox.Show("Пожалуйста, введите название агента.");
                return;
            }
            if (!(new Regex(@"\d{10}|\d{12}")).IsMatch(this.Inn.Text))
            {
                MessageBox.Show("ИНН должен состоять из 10 или 12 цифр.");
                return;
            }
            if (!(new Regex(@"\d{4}[\dA-Z][\dA-Z]\d{3}")).IsMatch(this.Kpp.Text))
            {
                MessageBox.Show("КПП должен соответствовать формату (4 цифры + 2 буквы + 2 цифры).");
                return;
            }
            if (!(new Regex(@"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}")).IsMatch(this.Phone.Text))
            {
                MessageBox.Show("Номер телефона должен соответствовать формату +X-XXX-XXX-XXXX.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(this.Emale.Text) &&
                !(new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)")).IsMatch(this.Emale.Text))
            {
                MessageBox.Show("Некорректный формат email.");
                return;
            }
            try
            {
                int priorityValue = Convert.ToInt32(this.Prioritet.Text);
                if (priorityValue < 0)
                {
                    MessageBox.Show("Приоритет не может быть отрицательным. Введите значение больше или равное 0.");
                    return;
                }
                agent.Priority = priorityValue;
            }
            catch
            {
                MessageBox.Show("Некорректное значение приоритета. Введите целое число.");
                return;
            }

            try
            {
                if (agent.ID > 0)
                {
                    helper.GetContext().Entry(agent).State = EntityState.Modified;
                }
                else
                {
                    helper.GetContext().Agent.Add(agent);
                }
                helper.GetContext().SaveChanges();
                MessageBox.Show("Операция успешно завершена.");
                this.NavigationService.Navigate(new Page2());
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        sb.AppendLine($"Ошибка свойства: {validationError.PropertyName}, ошибка: {validationError.ErrorMessage}");
                    }
                }
                MessageBox.Show($"Ошибка при сохранении данных: {sb.ToString()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (agent.ProductSale.Count > 0)
            {
                MessageBox.Show("Удаление не возможно!");
                return;
            }
            foreach (Shop shop in agent.Shop)
            {
                helper.GetContext().Shop.Remove(shop);
            }
            foreach (AgentPriorityHistory apr in agent.AgentPriorityHistory)
            {
                helper.GetContext().AgentPriorityHistory.Remove(apr);
            }
            helper.GetContext().Agent.Remove(agent);
            helper.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации об агенте завешено!");
            this.NavigationService.Navigate(new Page2());
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = Convert.ToInt32(count.Text);
            }
            catch
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }
            if (curSelPr > 0 && date.SelectedDate.HasValue && cnt > 0)
            {
                ProductSale pr = new ProductSale
                {
                    AgentID = agent.ID,
                    ProductID = curSelPr,
                    SaleDate = date.SelectedDate.Value,
                    ProductCount = cnt
                };
                try
                {
                    helper.GetContext().ProductSale.Add(pr);
                    helper.GetContext().SaveChanges();
                    historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ps => ps.AgentID == agent.ID).ToList();
                    UpdateAgentDiscount(agent);
                    MessageBox.Show("Операция успешно завершена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении реализации: {ex.Message}");
                }
            }
           }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (ProductSale prs in historyGrid.SelectedItems.OfType<ProductSale>())
            {
                helper.GetContext().ProductSale.Remove(prs);
            }
            try
            {
                helper.GetContext().SaveChanges();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ps => ps.AgentID == agent.ID).ToList();
                UpdateAgentDiscount(agent);
                MessageBox.Show("Операция успешно завершена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении реализации: {ex.Message}");
            }
        }
        private void UpdateAgentDiscount(Agent agent)
        {
            decimal totalSalesAmount = helper.GetContext().ProductSale.Where(ps => ps.AgentID == agent.ID).Sum(ps => ps.Product.MinCostForAgent * ps.ProductCount);
            if (totalSalesAmount < 10000)
            {
                agent.sale = 0;
            }
            else if (totalSalesAmount < 50000)
            {
                agent.sale = 5;
            }
            else if (totalSalesAmount < 150000)
            {
                agent.sale = 10;
            }
            else if (totalSalesAmount < 500000)
            {
                agent.sale = 20;
            }
            else
            {
                agent.sale = 25;
            }
            helper.GetContext().Entry(agent).State = EntityState.Modified;
            helper.GetContext().SaveChanges();
        }
        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curSelPr = ((Product)product.SelectedItem).ID;
        }
        private void frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                Page2 pg = (Page2)e.Content;
                pg.LoadAgents();
            }
            catch { };
        }
        private void historyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void mask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fnd = ((TextBox)sender).Text;
            try
            {
                product.ItemsSource = helper.GetContext().Product.Where(Product => Product.Title.Contains(fnd)).ToList();
            }
            catch {
            };
        }
    }
}
