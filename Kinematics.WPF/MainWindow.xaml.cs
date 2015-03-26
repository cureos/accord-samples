// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Cureos AB">
//   Copyriht (c) 2015 Anders Gustafsson, Cureos AB
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Kinematics.WPF
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Accord.Math.Kinematics;

    using AForge.Math;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region FIELDS

        /// <summary>
        /// The arm base model
        /// </summary>
        private DenavitHartenbergModel model;

        /// <summary>
        /// The model left gripper
        /// </summary>
        private DenavitHartenbergModel model_tgripper;

        /// <summary>
        /// The model right gripper
        /// </summary>
        private DenavitHartenbergModel model_bgripper;

        // The whole arm made of a combination of 
        // the three previously declared models:
        /// <summary>
        /// </summary>
        private DenavitHartenbergNode arm;

        /// <summary>
        /// The visualization model
        /// </summary>
        private DenavitHartenbergViewer viewer;

        /// <summary>
        /// Angle variable for animation
        /// </summary>
        private double angle = 0;

        private DispatcherTimer timer1;

        #endregion

        /// <summary>
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Ok, let's start to build our virtual robot arm !
            this.model = new DenavitHartenbergModel(new Vector3(0, 0, 0));

            // Add the first joint 
            this.model.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 35, offset: 0);

            // Add the second joint
            this.model.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 35, offset: 0);

            // Create the top finger
            this.model_tgripper = new DenavitHartenbergModel();
            this.model_tgripper.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 20, offset: 0);
            this.model_tgripper.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 20, offset: 0);

            // Create the bottom finger
            this.model_bgripper = new DenavitHartenbergModel();
            this.model_bgripper.Joints.Add(alpha: 0, theta: -Math.PI / 4, radius: 20, offset: 0);
            this.model_bgripper.Joints.Add(alpha: 0, theta: Math.PI / 3, radius: 20, offset: 0);

            // Create the model combinator from the parent model
            this.arm = new DenavitHartenbergNode(this.model);

            // Add the top finger
            this.arm.Children.Add(this.model_tgripper);

            // Add the bottom finger
            this.arm.Children.Add(this.model_bgripper);

            // Calculate the whole model (parent model + children models)
            this.arm.Compute();

            // Create the model visualizer
            this.viewer = new DenavitHartenbergViewer(300, 300) { JointRadius = 4f };

            // Assign each projection image of the model to a picture box
            this.pictureBox1.Source = this.viewer.PlaneXY;
            this.pictureBox2.Source = this.viewer.PlaneXZ;
            this.pictureBox3.Source = this.viewer.PlaneYZ;

            // Start the animation
            timer1 = new DispatcherTimer();
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 40);
            timer1.Tick += this.timer1_Tick;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Let's move some joints to make a "hello" or "help meeee !" gesture !
            model.Joints[0].Parameters.Theta = (float)(Math.Sin(angle) * Math.PI / 4 + Math.PI / 6);
            model.Joints[1].Parameters.Theta = (float)(Math.Sin(angle) * Math.PI / 4 + Math.PI / 6);

            // Increment the animation time
            angle += (float)Math.PI / 30;

            // Calculate the whole model
            arm.Compute();

            // Compute the images for displaying on the picture boxes
            viewer.ComputeImages(arm);

            // Refresh the pictures
            pictureBox1.UpdateLayout();
            pictureBox2.UpdateLayout();
            pictureBox3.UpdateLayout();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void PauseAndResumeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.timer1.IsEnabled)
            {
                this.timer1.Stop();
                this.PauseAndResumeButton.Content = "Resume";
            }
            else
            {
                this.timer1.Start();
                this.PauseAndResumeButton.Content = "Pause";
            }
        }
    }
}
