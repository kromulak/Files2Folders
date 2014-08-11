using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Files2Folders
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (button2.Text.Equals("Start"))
            {
                button2.Text = "Stop";
                textBox1.Enabled = false;
                button1.Enabled = false;

                groupFilesInFolders();
            }
            else
            {
                backgroundWorker1.CancelAsync();
                resetControls();
            }
        }

        private void resetControls()
        {
            button2.Text = "Start";
            textBox1.Enabled = true;
            button1.Enabled = true;
        }

        private void groupFilesInFolders()
        {
            backgroundWorker1.RunWorkerAsync();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs ework)
        {
            Console.WriteLine("BEGIN WORKER");

            string currentDir = textBox1.Text;


            if (!System.IO.Directory.Exists(currentDir))
            {
                MessageBox.Show("Invalid Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ework.Cancel = true;
            }
            else
            {

                DirectoryInfo di = new DirectoryInfo(currentDir);

                foreach (FileInfo file in di.EnumerateFiles())//string file in files)
                {
                    try
                    {
                        // Perform whatever action is required in your scenario.
                        //System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        moveFile(file, 1);
                        //Console.WriteLine("{0}: {1}, {2}", fi.Name, fi.Length, fi.CreationTime);
                        //backgroundWorker1.ReportProgress(1,fi.FullName + "\n");
                        if (backgroundWorker1.CancellationPending)
                        {
                            ework.Cancel = true;
                            return;
                        }
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // If file was deleted by a separate application 
                        //  or thread since the call to TraverseTree() 
                        // then just continue.
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }
            }
        }

        private bool moveFile(FileInfo file, int i)
        {
            string oldName = file.FullName;
            string newLoc = file.DirectoryName + "\\";

            if (file.Extension.Length > 2)
                newLoc = newLoc + file.Extension.Substring(1).ToUpper();
            else
                newLoc = newLoc + "BLANK";

            try
            {
                System.IO.FileInfo f = new System.IO.FileInfo(newLoc + "\\temp");
                f.Directory.Create();

                file.MoveTo(newLoc + "\\" + file.Name);

                // To move a file or folder to a new location:
                // System.IO.File.Move(sourceFile, destinationFile);
                backgroundWorker1.ReportProgress(i, "Moved [" + oldName + "] to \t [" + file.FullName + "]\n");
            }
            catch(Exception e)
            {
                backgroundWorker1.ReportProgress(i, "ERROR Moving [" + oldName + "] to [" + newLoc + "]\n" + e.Message + "\n");
                return false;
            }

            return true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        { 
            textBox2.AppendText(e.UserState.ToString());
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if((e.Cancelled))
            {
                textBox2.AppendText("\nSTOPPED - Did not complete move of all files.");
            }
            else
            {
                textBox2.AppendText("\nDONE - All files moved.");
            }

            resetControls();
        }
    }
}
