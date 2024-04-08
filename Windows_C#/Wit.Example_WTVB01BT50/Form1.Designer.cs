namespace Wit.Example_WTVB01BT50
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataRichTextBox = new System.Windows.Forms.RichTextBox();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cycle = new System.Windows.Forms.Button();
            this.cufoff = new System.Windows.Forms.Button();
            this.returnRate50 = new System.Windows.Forms.Button();
            this.returnRate10 = new System.Windows.Forms.Button();
            this.readReg03Button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stopScanButton = new System.Windows.Forms.Button();
            this.startScanButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.dataRichTextBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // dataRichTextBox
            // 
            resources.ApplyResources(this.dataRichTextBox, "dataRichTextBox");
            this.dataRichTextBox.Name = "dataRichTextBox";
            // 
            // leftPanel
            // 
            resources.ApplyResources(this.leftPanel, "leftPanel");
            this.leftPanel.Controls.Add(this.groupBox3);
            this.leftPanel.Controls.Add(this.groupBox2);
            this.leftPanel.Name = "leftPanel";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.cycle);
            this.groupBox3.Controls.Add(this.cufoff);
            this.groupBox3.Controls.Add(this.returnRate50);
            this.groupBox3.Controls.Add(this.returnRate10);
            this.groupBox3.Controls.Add(this.readReg03Button);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cycle
            // 
            resources.ApplyResources(this.cycle, "cycle");
            this.cycle.Name = "cycle";
            this.cycle.UseVisualStyleBackColor = true;
            this.cycle.Click += new System.EventHandler(this.cycle_Click);
            // 
            // cufoff
            // 
            resources.ApplyResources(this.cufoff, "cufoff");
            this.cufoff.Name = "cufoff";
            this.cufoff.UseVisualStyleBackColor = true;
            this.cufoff.Click += new System.EventHandler(this.cufoff_Click);
            // 
            // returnRate50
            // 
            resources.ApplyResources(this.returnRate50, "returnRate50");
            this.returnRate50.Name = "returnRate50";
            this.returnRate50.UseVisualStyleBackColor = true;
            this.returnRate50.Click += new System.EventHandler(this.returnRate50_Click);
            // 
            // returnRate10
            // 
            resources.ApplyResources(this.returnRate10, "returnRate10");
            this.returnRate10.Name = "returnRate10";
            this.returnRate10.UseVisualStyleBackColor = true;
            this.returnRate10.Click += new System.EventHandler(this.returnRate10_Click);
            // 
            // readReg03Button
            // 
            resources.ApplyResources(this.readReg03Button, "readReg03Button");
            this.readReg03Button.Name = "readReg03Button";
            this.readReg03Button.UseVisualStyleBackColor = true;
            this.readReg03Button.Click += new System.EventHandler(this.readReg03Button_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.stopScanButton);
            this.groupBox2.Controls.Add(this.startScanButton);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // stopScanButton
            // 
            resources.ApplyResources(this.stopScanButton, "stopScanButton");
            this.stopScanButton.Name = "stopScanButton";
            this.stopScanButton.UseVisualStyleBackColor = true;
            this.stopScanButton.Click += new System.EventHandler(this.stopScanButton_Click);
            // 
            // startScanButton
            // 
            resources.ApplyResources(this.startScanButton, "startScanButton");
            this.startScanButton.Name = "startScanButton";
            this.startScanButton.UseVisualStyleBackColor = true;
            this.startScanButton.Click += new System.EventHandler(this.startScanButton_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.leftPanel);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox dataRichTextBox;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button returnRate50;
        private System.Windows.Forms.Button returnRate10;
        private System.Windows.Forms.Button readReg03Button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button stopScanButton;
        private System.Windows.Forms.Button startScanButton;
        private System.Windows.Forms.Button cycle;
        private System.Windows.Forms.Button cufoff;
    }
}

