using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project
{
    public partial class Form1 : Form
    {
        public List<string> finalMachines = new List<string>();
        public List<string> finalJobs = new List<string>();
        public List<int> finalFlexes = new List<int>();

        public List<string> inProgressMachine = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < inProgressMachine.Count; i++)
            {
                var machinesIndex = Enumerable.Range(0, this.finalMachines.Count).Where(j => this.finalMachines[j] == inProgressMachine[i]).ToList();
                for (int k = 0; k < machinesIndex.Count; k++)
                {
                    if (chart1.Series.IsUniqueName(k.ToString()))
                    {
                        chart1.Series.Add(k.ToString());
                        chart1.Series[k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
                        chart1.Series[k].IsVisibleInLegend = false;
                    }
                    
                    chart1.Series[k].Points.AddXY(finalMachines[machinesIndex[k]], finalFlexes[machinesIndex[k]]);
                    int lastIndex = chart1.Series[k].Points.Count - 1;
                    chart1.Series[k].Points[lastIndex].Label = finalJobs[machinesIndex[k]];
                    chart1.Series[k].Points[lastIndex].Font = new Font(new FontFamily("Microsoft Sans Serif"), 14);
                }
            }
        }
    }
}
