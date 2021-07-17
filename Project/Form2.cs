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
    public partial class Form2 : Form
    {
        bool processed = false;
        List<string> allMachines = new List<string>();
        List<string> allJobs = new List<string>();
        List<int> allFlexes = new List<int>();

        List<string> finalMachines = new List<string>();
        List<string> finalJobs = new List<string>();
        List<int> finalFlexes = new List<int>();

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (processed == true && allMachines.Count > 0)
            {
                DialogResult dr = MessageBox.Show("شما در حال وارد کردن اطلاعات جدید هستید،. برای اینکه اطلاعات جدید شما قابل پردازش باشد باید اطلاعات قبلی از بین برود. آیا میخواهید اطلاعات قبلی را پاک کنید؟", "سوال", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                if (dr == DialogResult.Yes)
                {
                    allMachines.Clear();
                    allJobs.Clear();
                    allFlexes.Clear();
                    finalFlexes.Clear();
                    finalJobs.Clear();
                    finalMachines.Clear();
                    processed = false;
                }
            }
            var mj = textBox3.Text.Split(',');
            for (int i = 0; i < mj.Length; i++)
            {
                allJobs.Add(textBox1.Text);
                allFlexes.Add(int.Parse(textBox2.Text));
                allMachines.Add(mj[i]);
            }
            MessageBox.Show("کار " + textBox1.Text + " ثبت شد", "ثبت", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void run()
        {
            List<string> availableMachines = new List<string>();
            List<string> inProgressMachines = new List<string>();

            List<string> availableJobs = new List<string>();
            List<string> inProgressJobs = new List<string>();

            List<int> availableJFlexes = new List<int>();
            List<int> inProgressFlexes = new List<int>();

            /* Calculate Machines Flex */
            List<int> machinesFlexes = new List<int>();
            List<string> machinesJobs = new List<string>();
            List<int> machineJobsCount = new List<int>();

            int distinctMachinesCount = this.allMachines.Select(machine => machine).Distinct().Count();
            var distinctMachines = this.allMachines.Select(machine => machine).Distinct().ToList();

            availableMachines.AddRange(distinctMachines);

            availableJobs = this.allJobs.Select(job => job).Distinct().ToList();
            availableJFlexes = this.allFlexes.Select(flex => flex).Distinct().ToList();

            for (int i = 0; i < availableMachines.Count; i++)
            {
                machinesFlexes.Add(0);
                List<string> machinesJob = new List<string>();

                var machines = Enumerable.Range(0, this.allMachines.Count).Where(j => this.allMachines[j] == availableMachines[i]).ToList();
                machineJobsCount.Add(machines.Count);
                for (int k = 0; k < machines.Count; k++)
                {
                    machinesFlexes[i] = machinesFlexes[i] + allFlexes[machines[k]];
                    machinesJob.Add(this.allJobs[machines[k]]);
                }
                machinesJobs.AddRange(machinesJob);
            }


            while (machinesFlexes.Count != 0)
            {
                /* Choose a Machine with lowest flex */
                int lowestFlex = machinesFlexes.Min();
                int lowestFlexIndex = machinesFlexes.IndexOf(lowestFlex);

                string selectedMachine = availableMachines[lowestFlexIndex];
                inProgressMachines.Add(selectedMachine);
                finalMachines.Add(selectedMachine);
                availableMachines.Remove(selectedMachine);

                // Get Machines Jobs
                var machineJobsIndex = Enumerable.Range(0, this.allMachines.Count).Where(j => this.allMachines[j] == selectedMachine).ToList();
                List<int> machineJobsIndexTemp = new List<int>(machineJobsIndex);
                List<string> machineJobs = new List<string>();

                for (int i = 0; i < machineJobsIndexTemp.Count; i++)
                {
                    var exist = availableJobs.Any(job => job == allJobs[machineJobsIndexTemp[i]]);
                    if (exist)
                    {
                        machineJobs.Add(allJobs[machineJobsIndexTemp[i]]);
                    }
                    else
                    {
                        machineJobsIndex.Remove(machineJobsIndexTemp[i]);
                    }
                }

                int lowestJobMachinesCount = 0;
                string lowestJob = "";
                int lowestJobFlex = 0;

                for (int i = 0; i < machineJobsIndex.Count; i++)
                {
                    // Get Job Machines
                    var jobMachines = Enumerable.Range(0, this.allJobs.Count).Where(j => this.allJobs[j] == machineJobs[i]).ToList();
                    var jobFlex = allFlexes[machineJobsIndex[i]];

                    if (lowestJobMachinesCount == 0)
                    {
                        lowestJobMachinesCount = jobMachines.Count;
                        lowestJob = machineJobs[i];
                        lowestJobFlex = jobFlex;
                    }
                    else if (jobMachines.Count < lowestJobMachinesCount)
                    {
                        lowestJobMachinesCount = jobMachines.Count;
                        lowestJob = machineJobs[i];
                        lowestJobFlex = jobFlex;
                    }
                    else if (jobMachines.Count == lowestJobMachinesCount)
                    {
                        if (jobFlex > lowestJobFlex)
                        {
                            lowestJobMachinesCount = jobMachines.Count;
                            lowestJob = machineJobs[i];
                            lowestJobFlex = jobFlex;
                        }
                    }
                }

                inProgressJobs.Add(lowestJob);
                finalJobs.Add(lowestJob);
                availableJobs.Remove(lowestJob);

                machinesFlexes.Remove(lowestFlex);
            }

            List<int> availableJobsFlex = new List<int>();

            for (int i = 0; i < availableJobs.Count; i++)
            {
                // Get Available Jobs Flex
                var jobsFlex = Enumerable.Range(0, this.allJobs.Count).Where(j => this.allJobs[j] == availableJobs[i]).ToList();
                availableJobsFlex.Add(allFlexes[jobsFlex[0]]);
            }

            for (int i = 0; i < inProgressJobs.Count; i++)
            {
                // Get InProgress Jobs Flex
                var jobsFlex = Enumerable.Range(0, this.allJobs.Count).Where(j => this.allJobs[j] == inProgressJobs[i]).ToList();
                inProgressFlexes.Add(allFlexes[jobsFlex[0]]);
                finalFlexes.Add(allFlexes[jobsFlex[0]]);
            }

            int cmax = 0;
            int searched = 0;
            int availableJobsFlexCount = availableJobsFlex.Count;
            var distinctFlexes = this.allFlexes.Select(flex => flex).Distinct().ToList();
            var distinctJobs = this.allJobs.Select(job => job).Distinct().ToList();

            while (availableJobs.Count != 0)
            {
                var progressFlex = inProgressFlexes.Min();
                var progressFlexIndex = inProgressFlexes.IndexOf(progressFlex);
                var selectedMachine = inProgressMachines[progressFlexIndex];

                finalMachines.Add(selectedMachine);

                // Find Machine Jobs
                var machineJobsIndex = Enumerable.Range(0, this.allMachines.Count).Where(j => this.allMachines[j] == selectedMachine).ToList();
                List<int> machineJobsIndexTemp = new List<int>(machineJobsIndex);
                List<string> machineJobs = new List<string>();

                for (int i = 0; i < machineJobsIndexTemp.Count; i++)
                {
                    var exist = availableJobs.Any(job => job == allJobs[machineJobsIndexTemp[i]]);
                    if (exist)
                    {
                        machineJobs.Add(allJobs[machineJobsIndexTemp[i]]);
                    }
                    else
                    {
                        machineJobsIndex.Remove(machineJobsIndexTemp[i]);
                    }
                }

                // Select Job with Lowest Flex
                int lowestJobMachinesCount = 0;
                string lowestJob = "";
                int lowestJobFlex = 0;
                int jobFlex = 0;

                for (int i = 0; i < machineJobsIndex.Count; i++)
                {
                    // Get Job Machines
                    var jobMachines = Enumerable.Range(0, this.allJobs.Count).Where(j => this.allJobs[j] == machineJobs[i]).ToList();
                    jobFlex = allFlexes[machineJobsIndex[i]];

                    if (lowestJobMachinesCount == 0)
                    {
                        lowestJobMachinesCount = jobMachines.Count;
                        lowestJob = machineJobs[i];
                        lowestJobFlex = jobFlex;
                    }
                    else if (jobMachines.Count < lowestJobMachinesCount)
                    {
                        lowestJobMachinesCount = jobMachines.Count;
                        lowestJob = machineJobs[i];
                        lowestJobFlex = jobFlex;
                    }
                    else if (jobMachines.Count == lowestJobMachinesCount)
                    {
                        if (jobFlex > lowestJobFlex)
                        {
                            lowestJobMachinesCount = jobMachines.Count;
                            lowestJob = machineJobs[i];
                            lowestJobFlex = jobFlex;
                        }
                    }
                }

                inProgressJobs.Add(lowestJob);
                finalJobs.Add(lowestJob);
                availableJobs.Remove(lowestJob);

                finalFlexes.Add(lowestJobFlex);
                inProgressFlexes[progressFlexIndex] = inProgressFlexes[progressFlexIndex] + lowestJobFlex;
            }

            // Get CMAX count
            cmax = inProgressFlexes.Max();

            label5.Text = "مقدار cmax: " + cmax;
            processed = true;

            Form1 gantt = new Form1();
            gantt.finalFlexes = this.finalFlexes;
            gantt.finalJobs = this.finalJobs;
            gantt.finalMachines = this.finalMachines;
            gantt.inProgressMachine = inProgressMachines;
            gantt.Show();
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox3.Text != "")
                {
                    button1_Click(sender, new EventArgs());
                    textBox1.Focus();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run();
        }
    }
}
