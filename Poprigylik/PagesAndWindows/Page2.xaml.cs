using Poprigylik.BazaDan;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Poprigylik.MainWindow;
namespace Poprigylik.PagesAndWindows
{
    public partial class Page2 : Page
    {
        private int order = 0;
        private int currentPage = 1;  
        private int itemsPerPage = 10;
        private int totalItems = 0;
        private string fnd = "";
        private int iag = 0;
        public Page2()
        {
            InitializeComponent();
            LoadAgents();
            LoadAgentTypes();
            UpdatePagination();
        }
        public void LoadAgentTypes()
        {
            List<AgentType> agents = helper.GetContext().AgentType.ToList();
            agents.Insert(0, new AgentType { Title = "Все типы", ID = 0 });
            Type.ItemsSource = agents.OrderBy(AgentType => AgentType.ID);
        }
        public void LoadAgents()
        {
            try
            {
                IQueryable<Agent> ag = helper.GetContext().Agent
                    .Where(Agent => Agent.Title.Contains(fnd) || Agent.Phone.Contains(fnd) || Agent.Email.Contains(fnd));
                if (iag > 0)
                    ag = ag.Where(Agent => Agent.AgentTypeID == iag);
                ag = SortAgents(ag);
                totalItems = ag.Count();
                var agents = ag.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                foreach (Agent agent in agents)
                {
                    double totalSum = agent.ProductSale.Sum(sale => (double)(sale.ProductCount));
                    agent.sale = (int)totalSum;
                    agent.percent = CalculateDiscount(totalSum);
                    if (string.IsNullOrEmpty(agent.Logo) || agent.Logo == "нет")
                    {
                        agent.Logo = "/images/picture.png";
                    }
                }
                agentGrid.ItemsSource = agents;
                full.Text = totalItems.ToString();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
        private IQueryable<Agent> SortAgents(IQueryable<Agent> agents)
        {
            switch (order)
            {
                case 1:  
                    return agents.OrderBy(agent => agent.Title);
                case 2:  
                    return agents.OrderByDescending(agent => agent.Title);
                case 3:  
                    return agents.OrderBy(agent => agent.Priority);
                case 4:  
                    return agents.OrderByDescending(agent => agent.Priority);
                default:  
                    return agents.OrderBy(agent => agent.ID);  
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            if (selectedItem != null)
            {
                order = Convert.ToInt32(selectedItem.Tag.ToString());
                LoadAgents();
            }
        }
        private void UpdatePagination()
        {
            PaginationPanel.Children.Clear();

            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            Button prevButton = new Button
            {
                Content = "<",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                IsEnabled = currentPage > 1 
            };
            prevButton.Click += PreviousPage_Click;
            PaginationPanel.Children.Add(prevButton);

            
            for (int i = 1; i <= totalPages; i++)
            {
                Button pageButton = new Button
                {
                    Content = i.ToString(),
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = i == currentPage ? Brushes.LightBlue : Brushes.White,
                    Tag = i 
                };

                
                pageButton.Click += PageButton_Click;

                PaginationPanel.Children.Add(pageButton);
            }

            
            Button nextButton = new Button
            {
                Content = ">",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                IsEnabled = currentPage < totalPages 
            };
            nextButton.Click += NextPage_Click;
            PaginationPanel.Children.Add(nextButton);
        }
        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
           
            int selectedPage = (int)((Button)sender).Tag;

           
            currentPage = selectedPage;

            
            LoadAgents();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            
            if (currentPage > 1)
            {
                currentPage--;
                LoadAgents();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

           
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadAgents();
            }
        }
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            currentPage++;
            if (currentPage > (int)Math.Ceiling((double)totalItems / itemsPerPage))
                currentPage = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            LoadAgents();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            currentPage--;
            if (currentPage < 1) currentPage = 1;
            LoadAgents();
        }
        private int CalculateDiscount(double fsum)
        {
            if (fsum >= 10000 && fsum < 50000) return 5;
            if (fsum >= 50000 && fsum < 150000) return 10;
            if (fsum >= 150000 && fsum < 500000) return 20;
            if (fsum >= 500000) return 25;
            return 0;
        }
        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iag = ((AgentType)Type.SelectedItem).ID;
            LoadAgents();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fnd = ((TextBox)sender).Text;
            LoadAgents();
        }
        private void agentGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                Agent agent = agentGrid.SelectedItems[0] as Agent;

                if (agent != null)
                {
                    NavigationService.Navigate(new Page3(agent));
                }
            }
        }
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                int prt = 0;
                foreach (Agent agent in agentGrid.SelectedItems)
                {
                    if (agent.Priority > prt) prt = agent.Priority;
                }
                Window1 dlg = new Window1(prt);
                helper.prioritet = prt;
                helper.flag = false;
                dlg.ShowDialog();
                if (helper.flag)
                {
                    foreach (Agent agent in agentGrid.SelectedItems)
                    {
                        agent.Priority = helper.prioritet;
                        helper.GetContext().Entry(agent).State = EntityState.Modified;
                    }
                    helper.GetContext().SaveChanges();
                    LoadAgents();
                }
            }
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page3(null));
        }
    }
}
