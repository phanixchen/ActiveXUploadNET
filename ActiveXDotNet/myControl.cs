using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ActiveXDotNet
{
	public interface AxMyControl 
	{
		String	    UserText			 {  set; get ; }
	}

	/// <summary>
	/// Summary description for myControl.
	/// </summary>
	public class myControl : System.Windows.Forms.UserControl, AxMyControl
	{
		#region Form Components
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtUserText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		private String mStr_UserText;
		public String UserText
		{
			get { return mStr_UserText; }
			set 
			{ 
				mStr_UserText = value; 
				//Update the text box control value also.
				txtUserText.Text = value;
			}
		}


		#region Default autogenerated code... none modified.
		public myControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtUserText = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.txtUserText,
																					this.label1});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 56);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "my ActiveX Control Simulation";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "User Text:";
			// 
			// txtUserText
			// 
			this.txtUserText.Enabled = false;
			this.txtUserText.Location = new System.Drawing.Point(64, 20);
			this.txtUserText.Name = "txtUserText";
			this.txtUserText.Size = new System.Drawing.Size(200, 20);
			this.txtUserText.TabIndex = 1;
			this.txtUserText.Text = "";
			// 
			// myControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.Name = "myControl";
			this.Size = new System.Drawing.Size(288, 72);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

	}
}