namespace adventureGame
{
	partial class adventureGame
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
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblHP = new System.Windows.Forms.Label();
			this.lblGold = new System.Windows.Forms.Label();
			this.lblExp = new System.Windows.Forms.Label();
			this.lblLevel = new System.Windows.Forms.Label();
			this.btnTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "HP:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 17);
			this.label2.TabIndex = 1;
			this.label2.Text = "Gold:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "EXP:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 17);
			this.label4.TabIndex = 3;
			this.label4.Text = "Level:";
			// 
			// lblHP
			// 
			this.lblHP.AutoSize = true;
			this.lblHP.Location = new System.Drawing.Point(80, 19);
			this.lblHP.Name = "lblHP";
			this.lblHP.Size = new System.Drawing.Size(0, 17);
			this.lblHP.TabIndex = 4;
			// 
			// lblGold
			// 
			this.lblGold.AutoSize = true;
			this.lblGold.Location = new System.Drawing.Point(80, 45);
			this.lblGold.Name = "lblGold";
			this.lblGold.Size = new System.Drawing.Size(0, 17);
			this.lblGold.TabIndex = 5;
			// 
			// lblExp
			// 
			this.lblExp.AutoSize = true;
			this.lblExp.Location = new System.Drawing.Point(80, 73);
			this.lblExp.Name = "lblExp";
			this.lblExp.Size = new System.Drawing.Size(0, 17);
			this.lblExp.TabIndex = 6;
			// 
			// lblLevel
			// 
			this.lblLevel.AutoSize = true;
			this.lblLevel.Location = new System.Drawing.Point(80, 99);
			this.lblLevel.Name = "lblLevel";
			this.lblLevel.Size = new System.Drawing.Size(0, 17);
			this.lblLevel.TabIndex = 7;
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(342, 226);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(75, 23);
			this.btnTest.TabIndex = 8;
			this.btnTest.Text = "Press Me";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// adventureGame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(717, 643);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.lblLevel);
			this.Controls.Add(this.lblExp);
			this.Controls.Add(this.lblGold);
			this.Controls.Add(this.lblHP);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "adventureGame";
			this.Text = "Adventure Cave";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblHP;
		private System.Windows.Forms.Label lblGold;
		private System.Windows.Forms.Label lblExp;
		private System.Windows.Forms.Label lblLevel;
		private System.Windows.Forms.Button btnTest;
	}
}

