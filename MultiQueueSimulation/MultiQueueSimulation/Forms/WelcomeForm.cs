﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiQueueSimulation
{
    public partial class WelcomeForm : Form
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

        // determines which window is active from 0 to 3
        private int activeWindow;

        public WelcomeForm()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            activeWindow = 0;
            loadFileWindow.setWelcomeForm(this);
            firstCustomInputWindow.setWelcomeForm(this);
            secondCustomInputWindow.setWelcomeForm(this);
        }

        #region NAVIGATION_PICTURE_BUTTONS
        private void closePic_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Close();
        }

        private void nextPic_Click(object sender, EventArgs e)
        {
            if (activeWindow < 4)
                activeWindow++;

            activateWindow();
        }

        private void backPic_Click(object sender, EventArgs e)
        {
            if (activeWindow > 0)
                activeWindow--;

            activateWindow();
        }
        #endregion

        #region TIMER_AND_ACTIVATING_WINDOW
        private void timer_Tick(object sender, EventArgs e)
        {
            int animation_speed = 20;
            
            if (activeWindow == 0 && loadFileWindow.Left < Width)
            {
                loadFileWindow.Left += animation_speed;
            }

            else if (activeWindow == 1 && (loadFileWindow.Left > 0 || firstCustomInputWindow.Left < Width))
            {
                if (loadFileWindow.Left > 0)
                    loadFileWindow.Left -= animation_speed;

                if (firstCustomInputWindow.Left < Width)
                    firstCustomInputWindow.Left += animation_speed;
            }

            else if (activeWindow == 2 && (firstCustomInputWindow.Left > 0 || secondCustomInputWindow.Left < Width))
            {
                if (firstCustomInputWindow.Left > 0)
                    firstCustomInputWindow.Left -= animation_speed;

                if (secondCustomInputWindow.Left < Width)
                    secondCustomInputWindow.Left += animation_speed;
            }

            else if (activeWindow == 3 && secondCustomInputWindow.Left > 0)
            {
                secondCustomInputWindow.Left -= animation_speed;
            }
        }

        private void activateWindow()
        {
            if (activeWindow == 0) // Welcome Window
            {
                nextPic.Parent = welcomePanel;
                backPic.Parent = welcomePanel;
                closePic.Parent = welcomePanel;
            }

            else if (activeWindow == 1) // Load From File Window
            {
                nextPic.Parent = loadFileWindow;
                backPic.Parent = loadFileWindow;
                closePic.Parent = loadFileWindow;
            }

            else if (activeWindow == 2) // First Custom Input Window
            {
                nextPic.Parent = firstCustomInputWindow;
                backPic.Parent = firstCustomInputWindow;
                closePic.Parent = firstCustomInputWindow;
            }

            else if (activeWindow == 3) // Second Custom Input Window
            {
                nextPic.Parent = secondCustomInputWindow;
                backPic.Parent = secondCustomInputWindow;
                closePic.Parent = secondCustomInputWindow;
            }

            else if (activeWindow == 4) // Simulation Table Form
            {
                secondCustomInputWindow.simulateData();
            }

            if (activeWindow == 1 && (loadFileWindow.Left > 0 || firstCustomInputWindow.Left < Width))
            {
                loadFileWindow.BringToFront();
                firstCustomInputWindow.BringToFront();
            }

            else if (activeWindow == 2 && (firstCustomInputWindow.Left > 0 || secondCustomInputWindow.Left < Width))
            {
                firstCustomInputWindow.BringToFront();
                secondCustomInputWindow.BringToFront();
                secondCustomInputWindow.initializeServersColumns();
            }

            else if (activeWindow == 3 && secondCustomInputWindow.Left > 0)
            {
                secondCustomInputWindow.BringToFront();
            }

            nextPic.BringToFront();
            backPic.BringToFront();
            closePic.BringToFront();
        }
        #endregion

        public SecondCustomInputWindow getSecondCustomInput()
        {
            return secondCustomInputWindow;
        }

    }
}
