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
    public partial class GraphXY : Form
    {
        public GraphXY(int[] h, int[] v)
        {
            InitializeComponent();

            //chart.Series.Clear();
            chart.Series.Add("Horizontal");
            DataPointCollection horizontal = chart.Series["Horizontal"].Points;
            chart.Series["Horizontal"].Color = Color.Red;
            chart.Series["Horizontal"].ChartType = SeriesChartType.Line;

            chart.Series.Add("Vertical");
            DataPointCollection vertical = chart.Series["Vertical"].Points;
            chart.Series["Vertical"].Color = Color.Green;
            chart.Series["Vertical"].ChartType = SeriesChartType.Line;

            for (int i =0; i<h.Length ; i++)
            {
                horizontal.AddXY(i, h[i]);
            }

            for (int i = 0; i < v.Length; i++)
            {
                vertical.AddXY(v[i], i);
            }
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            //chart.ChartAreas[0].AxisX.Maximum = h.levels - 1;
            //chart.ChartAreas["Horizontal"].AxisX.Minimum = 0;
            //chart.ChartAreas["Horizontal"].AxisX.Title = "Intensidade";
            //chart.ChartAreas["Horizontal"].AxisY.Title = "Numero de pixeis";

            //chart.ChartAreas["Vertical"].AxisX.Minimum = 0;
            //chart.ChartAreas["Vertical"].AxisX.Title = "Intensidade";
            //chart.ChartAreas["Vertical"].AxisY.Title = "Numero de pixeis";

            chart.ResumeLayout();
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
