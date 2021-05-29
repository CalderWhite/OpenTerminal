using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OpenTerminal
{
    /// <summary>
    /// Interaction logic for TradeTapeView.xaml
    /// </summary>
    public partial class TradeTapeView : UserControl
    {
        private readonly ObservableCollection<Trade> tradeTapeSource = new ObservableCollection<Trade>();
        public TradeTapeView()
        {
            InitializeComponent();
            TradeTapeDataGrid.ItemsSource = tradeTapeSource;
        }

        public void UpdateTape(List<Trade> trades)
        {
            tradeTapeSource.Clear();
            for(int i=0; i<trades.Count; ++i)
            {
                tradeTapeSource.Add(trades[i]);
            }

            TradeTapeDataGrid.Items.Refresh();
            TradeTapeDataGrid.ScrollIntoView(tradeTapeSource.Last<Trade>());
        }
    }
}
