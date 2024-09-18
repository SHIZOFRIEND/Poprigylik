using Poprigylik.BazaDan;
using Poprigylik.PagesAndWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
namespace Poprigylik
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new Page2();
        }
        public class helper
        {
            public static PoprizonokEntities ent;
            public static bool flag = false;
            public static int prioritet = 0;
            public static PoprizonokEntities GetContext()
            {
                if (ent == null)
                {
                    ent = new PoprizonokEntities();
                }
                return ent;
            }
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
    }
}
