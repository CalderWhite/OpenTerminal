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
using System.Windows.Threading;
using ScottPlot.Plottable;

namespace OpenTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // constant strings for the application. Not enough to create our own resource dictionary.
        private const string PLACEHOLDER_TEXT = "Start typing to find a quote. Hit <ESC> if you want to clear the text.";
        private SolidColorBrush LIGHT_TEXT_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#adadad"));
        private SolidColorBrush TEXT_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDFDFD"));
        private System.Drawing.Color UP_COLOR = System.Drawing.ColorTranslator.FromHtml("#00FF00");
        private System.Drawing.Color DOWN_COLOR = System.Drawing.ColorTranslator.FromHtml("#FF0000");

        // how many ticks per second
        private const int TICK_INTERVAL = 15;
        // (the duration of 9:30am --> 4:00pm = 30 minutes * 13)
        // 13 * 30 minutes * 60 seconds / ticks per second = total number of ticks in a trading day.
        private const int TOTAL_TICKS = 11 * 30 * 60 / TICK_INTERVAL;
        private ScottPlot.OHLC[] data = new ScottPlot.OHLC[TOTAL_TICKS];
        private FinancePlot plot;
        private bool? autoTrackGraph = true;

        DateTime lastTime = new DateTime(2021, 5, 17, 1, 45, 0);
        private int updateCounter = 0;
        private int tickIndex = 0;

        // this is for our hack. since we have to graph all of our data at once, we wait until
        // the first data point is done, then set all remaining data point to the first one so they overlap 
        // and the graph looks normal
        private bool doneFirst = false;

        private BaseConnector exchangeConnector;

        public MainWindow()
        {
            InitializeComponent();

            TickerInput.Focus();
            TickerInput.OnTextUpdate = OnTickerChanged;

            ResetGraph();

            // plot the data array only once. ScottPlot requires at least one candlestick to be present when graphing.
            this.plot = wpfPlot1.Plot.AddCandlesticks(new ScottPlot.OHLC[1] { new ScottPlot.OHLC(0, 0, 0, 0, new DateTime(0), new TimeSpan(0,10,0)) });
            ApplyGraphStyle(plot);
            wpfPlot1.Plot.XAxis.DateTimeFormat(true);

            // create a timer to modify the data
            DispatcherTimer _updateDataTimer = new DispatcherTimer();
            _updateDataTimer.Interval = TimeSpan.FromMilliseconds(250);
            _updateDataTimer.Tick += UpdateData;
            _updateDataTimer.Start();

            // create a timer to update the GUI
            DispatcherTimer _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(200);
            _renderTimer.Tick += Render;
            _renderTimer.Start();

            Closed += (sender, args) =>
            {
                _updateDataTimer?.Stop();
                _renderTimer?.Stop();
            };
        }

        /**
         * Pulls down the latest data and then graphs it.
         */
        void UpdateData(object sender, EventArgs e)
        {
            if (exchangeConnector == null)
            {
                return;
            }
            List<Trade> newTrades = exchangeConnector.GetLatestTrades();

            // before deduping the trades we will update the contents of the tape
            TradeTape.UpdateTape(newTrades);
            // now dedupe the trades so we don't regraph old trades
            exchangeConnector.DedupeTrades(newTrades);

            if (newTrades.Count > 0)
            {
                double open = newTrades[newTrades.Count - 1].Price;
                double high = newTrades.Max(trade => trade.Price);
                double low = newTrades.Min(trade => trade.Price);
                double close = newTrades[0].Price;
                DateTime closeTime = newTrades[0].Time;

                GraphNextPoint(open, high, low, close, closeTime);
            }

            ++updateCounter;
            this.tickIndex = updateCounter / 4;
        }

        private void GraphNextPoint(double open, double high, double low, double close, DateTime closeTime)
        {
            if (updateCounter % 4 == 0)
            {
                if (!doneFirst)
                {
                    plot.Clear();
                    doneFirst = true;
                }
                
                this.plot.Add(open, high, low, close, lastTime, new TimeSpan(0, 10, 0));
                lastTime = lastTime.AddMinutes(10);
            }
            
            var last = plot.Last();
            if (low < last.Low)
            {
                last.Low = low;
            }
            if (high > last.High)
            {
                last.High = high;
            }

            last.Close = close;

            if (this.autoTrackGraph == true)
            {
                wpfPlot1.Plot.AxisAuto();
            }
        }

        /**
         * Since we delete and re-graph, we have to re apply the style many times. Thus this function exists.
         */
        private void ApplyGraphStyle(FinancePlot plot)
        {
            // NOTE: ScottPlot v4.0 has control over anti aliasing but v4.1-beta does not,
            //       so we are stuck with whatever the library wants to do for now.

            // this makes no sense but it works
            plot.ColorUp = UP_COLOR;
            plot.ColorDown = DOWN_COLOR;
            wpfPlot1.Plot.Layout(padding: 12);
            wpfPlot1.Plot.Style(figureBackground: System.Drawing.Color.Black, dataBackground: System.Drawing.Color.Black);
            wpfPlot1.Plot.Frame(false);
            wpfPlot1.Plot.XAxis.TickLabelStyle(color: System.Drawing.Color.White);
            wpfPlot1.Plot.XAxis.TickMarkColor(System.Drawing.ColorTranslator.FromHtml("#333333"));
            wpfPlot1.Plot.XAxis.MajorGrid(color: System.Drawing.ColorTranslator.FromHtml("#333333"));

            // hide the left axis and show a right axis
            //wpfPlot1.Plot.YAxis.Ticks(false);
            //wpfPlot1.Plot.YAxis.Grid(false);
            //wpfPlot1.Plot.YAxis2.Ticks(true);
            //wpfPlot1.Plot.YAxis2.Grid(true);
            wpfPlot1.Plot.YAxis.TickLabelStyle(color: UP_COLOR);
            wpfPlot1.Plot.YAxis.TickMarkColor(System.Drawing.ColorTranslator.FromHtml("#333333"));
            wpfPlot1.Plot.YAxis.MajorGrid(color: System.Drawing.ColorTranslator.FromHtml("#333333"));
            //wpfPlot1.Plot.YAxis2.TickLabelStyle(color: UP_COLOR);
            //wpfPlot1.Plot.YAxis2.TickMarkColor(System.Drawing.ColorTranslator.FromHtml("#333333"));
            //wpfPlot1.Plot.YAxis2.MajorGrid(color: System.Drawing.ColorTranslator.FromHtml("#333333"));

            // customize the legend style
            var legend = wpfPlot1.Plot.Legend();
            legend.FillColor = System.Drawing.Color.Transparent;
            legend.OutlineColor = System.Drawing.Color.Transparent;
            legend.Font.Color = System.Drawing.Color.White;
            legend.Font.Bold = true;
        }

        /**
         * This runs on a timer set in the constructor so that 
         * the mutated graph data is updated.
         */
        private void Render(object sender, EventArgs e)
        {
            wpfPlot1.Render();
        }

        private void OnTickerChanged()
        {
            exchangeConnector = new DummyConnector(TickerInput.CustomTextBox.Text);
            ResetGraph();
        }

        private void ResetGraph()
        {
            doneFirst = false;
            if (!(plot is null))
            {
                plot.Clear();
            }
            updateCounter = 0;
            tickIndex = 0;
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            this.autoTrackGraph = AutoTrackCheckbox.IsChecked;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            w.Show();
        }

        private void TradeTapeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (TradeTape == null)
            {
                return;
            }

            if (TradeTape.Visibility == Visibility.Visible)
            {
                TradeTape.Visibility = Visibility.Collapsed;
            } else
            {
                TradeTape.Visibility = Visibility.Visible;
            }
        }

        private void GlobalKeydown(object sender, RoutedEventArgs e)
        {
            TickerInput.Focus();
        }
    }
}
