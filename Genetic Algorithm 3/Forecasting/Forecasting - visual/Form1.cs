using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Forecasting___visual
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void LoadData(DataTable dataSet, int totalPeriods)
        {
            Debug.WriteLine(Chart_Line);
            for (int i = 1; i <= totalPeriods; i++)
            {
                var row = dataSet.Rows[i];
                Chart_Line.Series["test1"].Points.AddXY
                    ((double)row["t"], (double)row["Demand"]);
            }
            for (int i = totalPeriods+1; i < totalPeriods+12; i++)
            {
                var row = dataSet.Rows[i];
                Chart_Line.Series["test2"].Points.AddXY
                    ((double)row["t"], (double)row["Forecast"]);
            }
            Chart_Line.Series["test1"].ChartType =
                SeriesChartType.FastLine;
            Chart_Line.Series["test1"].Color = Color.Red;

            Chart_Line.Series["test2"].ChartType =
                SeriesChartType.FastLine;
            Chart_Line.Series["test2"].Color = Color.Red;
        }
    }
}