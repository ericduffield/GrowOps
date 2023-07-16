using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;

namespace GrowOps.ViewModels
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    [ObservableObject]
    public partial class ChartViewModel
    {
        static private ObservableCollection<ObservablePoint> ChartValues
        {
            get
            {
                ObservableCollection<ObservablePoint> collection = new ObservableCollection<ObservablePoint>();
                try
                {
                    var temperatures = App.Repo.ReadingDB.Items.Where(reading => reading.Type == Enums.Type.TEMPERATURE).ToList();
                    var humidities = App.Repo.ReadingDB.Items.Where(reading => reading.Type == Enums.Type.HUMIDITY).ToList();
                    int lowest = temperatures.Count > humidities.Count ? humidities.Count : temperatures.Count;

                    for (int i = 0; i < lowest; i++)
                    {
                        if (!(temperatures[i].Value <= 0 || humidities[i].Value <= 0))
                            collection.Add(new ObservablePoint(temperatures[i].Value, humidities[i].Value));
                    }


                }
                catch
                {
                }

                return collection;


            }
        }

        public ISeries[] Series { get; set; } =
        {
            new ScatterSeries<ObservablePoint>
            {
                TooltipLabelFormatter = value => $"Temperature: {value.PrimaryValue}\nHumidity: {value.SecondaryValue}",
                Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 5 },
                Fill = null,
                Values = ChartValues
            }
        };

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Temperature Humidity Comparison",
                TextSize = 60,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public Axis[] XAxes { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "Temperature °C",
                    NamePaint = new SolidColorPaint(SKColors.Black),

                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 45,
                    NameTextSize = 50,

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
                }
            };

        public Axis[] YAxes { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "Humidity",
                    NameTextSize = 50,
                    NamePaint = new SolidColorPaint(SKColors.Black),

                    LabelsPaint = new SolidColorPaint(SKColors.Green),
                    TextSize = 45,

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
                    {
                        StrokeThickness = 2,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    }
                }
            };
    }
}
