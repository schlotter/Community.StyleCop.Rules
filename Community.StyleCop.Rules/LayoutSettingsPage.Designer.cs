// <copyright file="LayoutSettingsPage.Designer.cs" company="n/a">
// Copyright 2009, 2010 Christian Schlotter
//
// This file is part of StyleCop Community Rules.
//
// StyleCop Community Rules is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or (at your
// option) any later version.
//
// StyleCop Community Rules is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with StyleCop Community Rules. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <author>Christian Schlotter</author>
// <email>again@gmx.de</email>
// <summary>Settings tab for configuring the LayoutRules analyzer.</summary>

namespace Community.StyleCop.CSharp
{
    /// <summary>
    /// Settings tab for configuring the LayoutRules analyzer.
    /// </summary>
    internal partial class LayoutSettingsPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be
        /// disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.maximumLineLengthLabel = new System.Windows.Forms.Label();
            this.maximumLineLengthTextBox = new System.Windows.Forms.TextBox();
            this.charactersLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // maximumLineLengthLabel
            //
            this.maximumLineLengthLabel.AutoSize = true;
            this.maximumLineLengthLabel.Location = new System.Drawing.Point(3, 7);
            this.maximumLineLengthLabel.Name = "maximumLineLengthLabel";
            this.maximumLineLengthLabel.Size = new System.Drawing.Size(105, 13);
            this.maximumLineLengthLabel.TabIndex = 0;
            this.maximumLineLengthLabel.Text = "Maximum line length:";
            //
            // maximumLineLengthTextBox
            //
            this.maximumLineLengthTextBox.Location = new System.Drawing.Point(114, 4);
            this.maximumLineLengthTextBox.Name = "maximumLineLengthTextBox";
            this.maximumLineLengthTextBox.Size = new System.Drawing.Size(38, 20);
            this.maximumLineLengthTextBox.TabIndex = 1;
            this.maximumLineLengthTextBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            //
            // charactersLabel
            //
            this.charactersLabel.AutoSize = true;
            this.charactersLabel.Location = new System.Drawing.Point(158, 7);
            this.charactersLabel.Name = "charactersLabel";
            this.charactersLabel.Size = new System.Drawing.Size(57, 13);
            this.charactersLabel.TabIndex = 2;
            this.charactersLabel.Text = "characters";
            //
            // LayoutSettingsPage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.charactersLabel);
            this.Controls.Add(this.maximumLineLengthTextBox);
            this.Controls.Add(this.maximumLineLengthLabel);
            this.Name = "LayoutSettingsPage";
            this.Size = new System.Drawing.Size(313, 150);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label maximumLineLengthLabel;
        private System.Windows.Forms.TextBox maximumLineLengthTextBox;
        private System.Windows.Forms.Label charactersLabel;
    }
}
