namespace GuidanceSimulator
{
    partial class GuidanceMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.synthesisBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.detailTextbox = new System.Windows.Forms.TextBox();
            this.txtAssemblyName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.problemTree = new System.Windows.Forms.TreeView();
            this.putTree = new System.Windows.Forms.TreeView();
            this.synthesistxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Problems:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 346);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "PUT:";
            // 
            // synthesisBtn
            // 
            this.synthesisBtn.Location = new System.Drawing.Point(113, 51);
            this.synthesisBtn.Name = "synthesisBtn";
            this.synthesisBtn.Size = new System.Drawing.Size(164, 23);
            this.synthesisBtn.TabIndex = 4;
            this.synthesisBtn.Text = "SynthesizeFixSimulation";
            this.synthesisBtn.UseVisualStyleBackColor = true;
            this.synthesisBtn.Click += new System.EventHandler(this.synthesisBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Problem Details:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // detailTextbox
            // 
            this.detailTextbox.Location = new System.Drawing.Point(423, 104);
            this.detailTextbox.Multiline = true;
            this.detailTextbox.Name = "detailTextbox";
            this.detailTextbox.ReadOnly = true;
            this.detailTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.detailTextbox.Size = new System.Drawing.Size(455, 239);
            this.detailTextbox.TabIndex = 7;
            this.detailTextbox.TextChanged += new System.EventHandler(this.detailTextbox_TextChanged);
            // 
            // txtAssemblyName
            // 
            this.txtAssemblyName.Location = new System.Drawing.Point(24, 25);
            this.txtAssemblyName.Name = "txtAssemblyName";
            this.txtAssemblyName.Size = new System.Drawing.Size(149, 20);
            this.txtAssemblyName.TabIndex = 9;
            this.txtAssemblyName.Text = "Benchmarks";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "AssemblyName";
            // 
            // refreshBtn
            // 
            this.refreshBtn.Location = new System.Drawing.Point(23, 52);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(84, 23);
            this.refreshBtn.TabIndex = 10;
            this.refreshBtn.Text = "Refresh";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // problemTree
            // 
            this.problemTree.Location = new System.Drawing.Point(24, 104);
            this.problemTree.Name = "problemTree";
            this.problemTree.Size = new System.Drawing.Size(381, 239);
            this.problemTree.TabIndex = 11;
            this.problemTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.problemTree_NodeMouseClick);
            // 
            // putTree
            // 
            this.putTree.Location = new System.Drawing.Point(24, 362);
            this.putTree.Name = "putTree";
            this.putTree.Size = new System.Drawing.Size(381, 278);
            this.putTree.TabIndex = 12;
            this.putTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.putTree_AfterSelect);
            this.putTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.putTree_NodeMouseClick);
            // 
            // synthesistxtBox
            // 
            this.synthesistxtBox.Location = new System.Drawing.Point(423, 362);
            this.synthesistxtBox.Multiline = true;
            this.synthesistxtBox.Name = "synthesistxtBox";
            this.synthesistxtBox.ReadOnly = true;
            this.synthesistxtBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.synthesistxtBox.Size = new System.Drawing.Size(455, 278);
            this.synthesistxtBox.TabIndex = 13;
            this.synthesistxtBox.TextChanged += new System.EventHandler(this.synthesistxtBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(420, 346);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Synthesized Details:";
            // 
            // GuidanceMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 652);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.synthesistxtBox);
            this.Controls.Add(this.putTree);
            this.Controls.Add(this.problemTree);
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.txtAssemblyName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.detailTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.synthesisBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GuidanceMainWindow";
            this.Text = "GuidanceSimulator";
            this.Load += new System.EventHandler(this.GuidanceMainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button synthesisBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox detailTextbox;
        private System.Windows.Forms.TextBox txtAssemblyName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.TreeView problemTree;
        private System.Windows.Forms.TreeView putTree;
        private System.Windows.Forms.TextBox synthesistxtBox;
        private System.Windows.Forms.Label label5;
    }
}

