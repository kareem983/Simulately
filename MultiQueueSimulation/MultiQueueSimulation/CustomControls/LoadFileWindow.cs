﻿using MultiQueueModels;
using MultiQueueSimulation.Forms;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MultiQueueSimulation
{
    public partial class LoadFileWindow : UserControl
    {
        // ========================== Rounding Edges ==========================//
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // width of ellipse
           int nHeightEllipse // height of ellipse
        );
        // =====================================================================//

        private WelcomeForm welcomeForm;
        private int FileLineIndex;
        private Thread thread;

        public LoadFileWindow()
        {
            InitializeComponent();

            containerPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            FileLineIndex = 0;
        }

        private void loadFromFileBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog() { Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*", Multiselect = false })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string FilePath = dialog.FileName;
                    string[] lines = System.IO.File.ReadAllLines(FilePath);
                    setConfigurations(lines);
                    SetInterarrivalDist(lines);

                    for (int i = 0; i < Program.system.NumberOfServers; i++)
                        SetServerServiceTime(i + 1, lines);

                    openSumulationTableForm();

                    // Testing Data File which you get from the file
                    // testFileData();
                }
            }
        }

        private void setConfigurations(string[] lines)
        {
            Program.system.NumberOfServers = int.Parse(lines[1]);
            Program.system.StoppingNumber = int.Parse(lines[4]);
            int stoppintCriteriaID = int.Parse(lines[7]);
            int ServerSelectionID = int.Parse(lines[10]);

            if (stoppintCriteriaID == 1)
                Program.system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            else
                Program.system.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;

            if (ServerSelectionID == 1)
                Program.system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            else if (ServerSelectionID == 2)
                Program.system.SelectionMethod = Enums.SelectionMethod.Random;
            else
                Program.system.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
        }

        private void SetInterarrivalDist(string[] lines)
        {
            for (int i = 13; i < lines.Length; i++)
            {
                if (lines[i] == "") { FileLineIndex = i + 2; break; }

                char[] separator = { ',', ' ' };
                string[] timesAndProbs = lines[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                int time = int.Parse(timesAndProbs[0]);
                decimal prob = (decimal)float.Parse(timesAndProbs[1]);

                Program.system.InterarrivalDistribution.Add(new TimeDistribution(time, prob));
            }

        }

        private void SetServerServiceTime(int serverID, string[] lines)
        {
            for (int i = FileLineIndex; i < lines.Length; i++)
            {
                if (lines[i] == "") { FileLineIndex = i + 2; break; }

                char[] separator = { ',', ' ' };
                string[] timesAndProbs = lines[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                int time = int.Parse(timesAndProbs[0]);
                decimal prob = (decimal)float.Parse(timesAndProbs[1]);
                Server server = new Server(serverID, time, prob);
                Program.system.Servers.Add(server);

            }
        }

        public void setWelcomeForm(WelcomeForm welcomeForm)
        {
            this.welcomeForm = welcomeForm;
        }

        private void openSumulationTableForm()
        {
            SimulationTableForm simulationTableForm = new SimulationTableForm();
            thread = new Thread(openSimulationForm);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            welcomeForm.Close();
        }

        private void openSimulationForm(object obj)
        {
            Application.Run(new SimulationTableForm());
        }
        
        private void testFileData()
        {
            string showConfigurations = Program.system.NumberOfServers + " "
                 + Program.system.StoppingNumber + " "
                 + Program.system.StoppingCriteria + " "
                 + Program.system.SelectionMethod;

            string showInterarrivalTime = "";
            for (int i = 0; i < Program.system.InterarrivalDistribution.Count; ++i)
            {
                showInterarrivalTime += " [Time] ";
                showInterarrivalTime += Program.system.InterarrivalDistribution[i].Time.ToString();
                showInterarrivalTime += " [Prob] ";
                showInterarrivalTime += Program.system.InterarrivalDistribution[i].Probability.ToString();
            }

            string showServerServiceTime = "";
            for (int i = 0; i < Program.system.Servers.Count; ++i)
            {
                showServerServiceTime += " Server -> " + Program.system.Servers[i].ID + "\n";
                for (int j = 0; j < Program.system.Servers[i].TimeDistribution.Count; ++j)
                {
                    showServerServiceTime += " [Time] ";
                    showServerServiceTime += Program.system.Servers[i].TimeDistribution[j].Time.ToString();
                    showServerServiceTime += " [Prob] ";
                    showServerServiceTime += Program.system.Servers[i].TimeDistribution[j].Probability.ToString();
                }
            }

            MessageBox.Show(showConfigurations, "DATA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(showInterarrivalTime, "DATA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(showServerServiceTime, "DATA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
