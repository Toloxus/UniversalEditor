using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace UniversalEditor.Plugins.Multimedia.Dialogs.Audio
{
	public class NoteExpressionProperty : Form
	{
		private IContainer components = null;
		private Label lblTemplate;
		private ComboBox cboTemplate;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private Label label2;
		private TextBox textBox1;
		private TrackBar trackBar2;
		private GroupBox fraBendDepth;
		private TextBox txtBendDepth;
		private Label label1;
		private TrackBar trackBar1;
		private GroupBox groupBox3;
		private CheckBox checkBox1;
		private CheckBox checkBox2;
		private GroupBox groupBox4;
		private GroupBox groupBox6;
		private Label label3;
		private TextBox textBox2;
		private TrackBar trackBar3;
		private GroupBox groupBox7;
		private TextBox textBox3;
		private Label label4;
		private TrackBar trackBar4;
		private Button cmdCancel;
		private Button cmdOK;
		public NoteExpressionProperty()
		{
			this.InitializeComponent();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.lblTemplate = new Label();
			this.cboTemplate = new ComboBox();
			this.groupBox1 = new GroupBox();
			this.trackBar1 = new TrackBar();
			this.fraBendDepth = new GroupBox();
			this.txtBendDepth = new TextBox();
			this.label1 = new Label();
			this.groupBox2 = new GroupBox();
			this.textBox1 = new TextBox();
			this.trackBar2 = new TrackBar();
			this.label2 = new Label();
			this.groupBox3 = new GroupBox();
			this.checkBox1 = new CheckBox();
			this.checkBox2 = new CheckBox();
			this.groupBox4 = new GroupBox();
			this.groupBox6 = new GroupBox();
			this.label3 = new Label();
			this.textBox2 = new TextBox();
			this.trackBar3 = new TrackBar();
			this.groupBox7 = new GroupBox();
			this.textBox3 = new TextBox();
			this.label4 = new Label();
			this.trackBar4 = new TrackBar();
			this.cmdCancel = new Button();
			this.cmdOK = new Button();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.trackBar1).BeginInit();
			this.fraBendDepth.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((ISupportInitialize)this.trackBar2).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((ISupportInitialize)this.trackBar3).BeginInit();
			this.groupBox7.SuspendLayout();
			((ISupportInitialize)this.trackBar4).BeginInit();
			base.SuspendLayout();
			this.lblTemplate.AutoSize = true;
			this.lblTemplate.FlatStyle = FlatStyle.System;
			this.lblTemplate.Location = new Point(12, 15);
			this.lblTemplate.Name = "lblTemplate";
			this.lblTemplate.Size = new Size(54, 13);
			this.lblTemplate.TabIndex = 0;
			this.lblTemplate.Text = "&Template:";
			this.cboTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.cboTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboTemplate.FlatStyle = FlatStyle.System;
			this.cboTemplate.FormattingEnabled = true;
			this.cboTemplate.Location = new Point(72, 12);
			this.cboTemplate.Name = "cboTemplate";
			this.cboTemplate.Size = new Size(330, 21);
			this.cboTemplate.TabIndex = 1;
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.fraBendDepth);
			this.groupBox1.FlatStyle = FlatStyle.System;
			this.groupBox1.Location = new Point(12, 39);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(192, 261);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pitch control";
			this.trackBar1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom);
			this.trackBar1.Location = new Point(21, 19);
			this.trackBar1.Maximum = 100;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Orientation = Orientation.Vertical;
			this.trackBar1.Size = new Size(45, 104);
			this.trackBar1.TabIndex = 0;
			this.trackBar1.TickFrequency = 10;
			this.trackBar1.TickStyle = TickStyle.Both;
			this.trackBar1.Value = 8;
			this.fraBendDepth.Controls.Add(this.txtBendDepth);
			this.fraBendDepth.Controls.Add(this.label1);
			this.fraBendDepth.Controls.Add(this.trackBar1);
			this.fraBendDepth.FlatStyle = FlatStyle.System;
			this.fraBendDepth.Location = new Point(6, 19);
			this.fraBendDepth.Name = "fraBendDepth";
			this.fraBendDepth.Size = new Size(87, 161);
			this.fraBendDepth.TabIndex = 0;
			this.fraBendDepth.TabStop = false;
			this.fraBendDepth.Text = "&Bend depth:";
			this.txtBendDepth.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.txtBendDepth.Location = new Point(14, 129);
			this.txtBendDepth.Name = "txtBendDepth";
			this.txtBendDepth.Size = new Size(56, 20);
			this.txtBendDepth.TabIndex = 1;
			this.txtBendDepth.Text = "8";
			this.txtBendDepth.TextAlign = HorizontalAlignment.Right;
			this.label1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.label1.FlatStyle = FlatStyle.System;
			this.label1.Location = new Point(71, 132);
			this.label1.Name = "label1";
			this.label1.Size = new Size(10, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "%";
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textBox1);
			this.groupBox2.Controls.Add(this.trackBar2);
			this.groupBox2.FlatStyle = FlatStyle.System;
			this.groupBox2.Location = new Point(99, 19);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(87, 161);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Bend &length:";
			this.textBox1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBox1.Location = new Point(14, 129);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(56, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "0";
			this.textBox1.TextAlign = HorizontalAlignment.Right;
			this.trackBar2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom);
			this.trackBar2.Location = new Point(21, 19);
			this.trackBar2.Maximum = 100;
			this.trackBar2.Name = "trackBar2";
			this.trackBar2.Orientation = Orientation.Vertical;
			this.trackBar2.Size = new Size(45, 104);
			this.trackBar2.TabIndex = 0;
			this.trackBar2.TickFrequency = 10;
			this.trackBar2.TickStyle = TickStyle.Both;
			this.label2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.label2.FlatStyle = FlatStyle.System;
			this.label2.Location = new Point(71, 132);
			this.label2.Name = "label2";
			this.label2.Size = new Size(10, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "%";
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			this.groupBox3.FlatStyle = FlatStyle.System;
			this.groupBox3.Location = new Point(6, 186);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(180, 69);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Add portamento";
			this.checkBox1.AutoSize = true;
			this.checkBox1.FlatStyle = FlatStyle.System;
			this.checkBox1.Location = new Point(18, 20);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new Size(142, 18);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "During &rising movement";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox2.AutoSize = true;
			this.checkBox2.FlatStyle = FlatStyle.System;
			this.checkBox2.Location = new Point(18, 44);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new Size(145, 18);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Text = "During &falling movement";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.groupBox4.Controls.Add(this.groupBox6);
			this.groupBox4.Controls.Add(this.groupBox7);
			this.groupBox4.FlatStyle = FlatStyle.System;
			this.groupBox4.Location = new Point(210, 39);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(192, 261);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Dynamics control";
			this.groupBox6.Controls.Add(this.label3);
			this.groupBox6.Controls.Add(this.textBox2);
			this.groupBox6.Controls.Add(this.trackBar3);
			this.groupBox6.FlatStyle = FlatStyle.System;
			this.groupBox6.Location = new Point(99, 19);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new Size(87, 161);
			this.groupBox6.TabIndex = 1;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "&Accent:";
			this.label3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.label3.FlatStyle = FlatStyle.System;
			this.label3.Location = new Point(71, 132);
			this.label3.Name = "label3";
			this.label3.Size = new Size(10, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "%";
			this.textBox2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBox2.Location = new Point(14, 129);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new Size(56, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "50";
			this.textBox2.TextAlign = HorizontalAlignment.Right;
			this.trackBar3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom);
			this.trackBar3.Location = new Point(21, 19);
			this.trackBar3.Maximum = 100;
			this.trackBar3.Name = "trackBar3";
			this.trackBar3.Orientation = Orientation.Vertical;
			this.trackBar3.Size = new Size(45, 104);
			this.trackBar3.TabIndex = 0;
			this.trackBar3.TickFrequency = 10;
			this.trackBar3.TickStyle = TickStyle.Both;
			this.trackBar3.Value = 50;
			this.groupBox7.Controls.Add(this.textBox3);
			this.groupBox7.Controls.Add(this.label4);
			this.groupBox7.Controls.Add(this.trackBar4);
			this.groupBox7.FlatStyle = FlatStyle.System;
			this.groupBox7.Location = new Point(6, 19);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new Size(87, 161);
			this.groupBox7.TabIndex = 0;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "&Decay:";
			this.textBox3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBox3.Location = new Point(14, 129);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new Size(56, 20);
			this.textBox3.TabIndex = 1;
			this.textBox3.Text = "50";
			this.textBox3.TextAlign = HorizontalAlignment.Right;
			this.label4.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.label4.FlatStyle = FlatStyle.System;
			this.label4.Location = new Point(71, 132);
			this.label4.Name = "label4";
			this.label4.Size = new Size(10, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "%";
			this.trackBar4.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom);
			this.trackBar4.Location = new Point(21, 19);
			this.trackBar4.Maximum = 100;
			this.trackBar4.Name = "trackBar4";
			this.trackBar4.Orientation = Orientation.Vertical;
			this.trackBar4.Size = new Size(45, 104);
			this.trackBar4.TabIndex = 0;
			this.trackBar4.TickFrequency = 10;
			this.trackBar4.TickStyle = TickStyle.Both;
			this.trackBar4.Value = 50;
			this.cmdCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cmdCancel.FlatStyle = FlatStyle.System;
			this.cmdCancel.Location = new Point(327, 306);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new Size(75, 23);
			this.cmdCancel.TabIndex = 5;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdOK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cmdOK.FlatStyle = FlatStyle.System;
			this.cmdOK.Location = new Point(246, 306);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new Size(75, 23);
			this.cmdOK.TabIndex = 4;
			this.cmdOK.Text = "&OK";
			this.cmdOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.cmdOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.cmdCancel;
			base.ClientSize = new Size(414, 341);
			base.Controls.Add(this.cmdOK);
			base.Controls.Add(this.cmdCancel);
			base.Controls.Add(this.groupBox4);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.cboTemplate);
			base.Controls.Add(this.lblTemplate);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NoteExpressionProperty";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Note Expression Properties";
			this.groupBox1.ResumeLayout(false);
			((ISupportInitialize)this.trackBar1).EndInit();
			this.fraBendDepth.ResumeLayout(false);
			this.fraBendDepth.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((ISupportInitialize)this.trackBar2).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			((ISupportInitialize)this.trackBar3).EndInit();
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			((ISupportInitialize)this.trackBar4).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
