using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SS_OpenCV
{
    public partial class Histogram : Form
    {
        public Histogram(ImageClass.Histogram h)
        {
            InitializeComponent();

            //chart.Series.Clear();
            chart.Series.Add("Red");
            DataPointCollection red = chart.Series["Red"].Points;
            chart.Series["Red"].Color = Color.Red;
            chart.Series["Red"].ChartType = SeriesChartType.Line;

            chart.Series.Add("Green");
            DataPointCollection green = chart.Series["Green"].Points;
            chart.Series["Green"].Color = Color.Green;
            chart.Series["Green"].ChartType = SeriesChartType.Line;

            chart.Series.Add("Blue");
            DataPointCollection blue = chart.Series["Blue"].Points;
            chart.Series["Blue"].Color = Color.Blue;
            chart.Series["Blue"].ChartType = SeriesChartType.Line;

            chart.Series.Add("BW");
            DataPointCollection bw = chart.Series["BW"].Points;
            chart.Series["BW"].Color = Color.Black;
            chart.Series["BW"].ChartType = SeriesChartType.Line;

            for (int i = 0; i<h.levels; i++) {
                red.AddXY(i, h.Red[i]);
                green.AddXY(i, h.Green[i]);
                blue.AddXY(i, h.Blue[i]);
                bw.AddXY(i, h.BW[i]);
            }

            chart.ChartAreas[0].AxisX.Maximum = h.levels - 1;
            chart.ChartAreas[0].AxisX.Minimum = 0;

            chart.ChartAreas[0].AxisX.Title = "Intensidade";
            chart.ChartAreas[0].AxisY.Title = "Numero de pixeis";
            chart.ResumeLayout();
            this.Show();
        }

        private void Histogram_Load(object sender, EventArgs e)
        {
            
        }
    }
}
