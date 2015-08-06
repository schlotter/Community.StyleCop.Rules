// <copyright file="LayoutSettingsPage.cs" company="n/a">
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

using System;
using System.Globalization;
using System.Windows.Forms;

using StyleCop;

namespace Community.StyleCop.CSharp
{
    /// <summary>
    /// Settings tab for configuring the LayoutRules analyzer.
    /// </summary>
    internal partial class LayoutSettingsPage :
        UserControl,
        IPropertyControlPage
    {
        /// <summary>
        /// Title of the settings tab.
        /// </summary>
        private const string SettingsTabName = "Community Settings";

        /// <summary>
        /// Indicates whether the contents of the tab have changed.
        /// </summary>
        private bool dirty;

        /// <summary>
        /// The control that hosts the tabs.
        /// </summary>
        private PropertyControl tabControl;

        /// <summary>
        /// The analyzer that is configured through this tab.
        /// </summary>
        private readonly LayoutRules analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettingsPage"/>
        /// class.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="analyzer"/> is <c>null</c>.</exception>
        public LayoutSettingsPage(LayoutRules analyzer)
        {
            Param.RequireNotNull(analyzer, "analyzer");

            this.InitializeComponent();

            this.dirty = true;
            this.analyzer = analyzer;
        }

        #region IPropertyControlPage Members

        /// <summary>
        /// Gets or sets a value indicating whether this <see
        /// cref="LayoutSettingsPage"/> is dirty.
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        public bool Dirty
        {
            get
            {
                return this.dirty;
            }

            set
            {
                if (this.dirty != value)
                {
                    this.dirty = value;
                    this.tabControl.DirtyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the tab.
        /// </summary>
        /// <value>The name of the tab.</value>
        public string TabName
        {
            get { return SettingsTabName; }
        }

        /// <summary>
        /// This method is unused.
        /// </summary>
        /// <param name="activated">This parameter is unused.</param>
        /// <remarks>This method's implementation is empty.</remarks>
        public void Activate(bool activated)
        {
        }

        /// <summary>
        /// Saves settings the user confirmed.
        /// </summary>
        /// <returns>Always returns <c>true</c>.</returns>
        public bool Apply()
        {
            if (this.analyzer != null)
            {
                if (this.maximumLineLengthTextBox.Text.Length == 0)
                {
                    this.analyzer.ClearSetting(
                        this.tabControl.LocalSettings,
                        Strings.MaximumLineLength);
                }
                else
                {
                    int maximumLineLength = int.Parse(
                        this.maximumLineLengthTextBox.Text,
                        CultureInfo.CurrentCulture);
                    var newIntProperty = new IntProperty(
                        this.analyzer,
                        Strings.MaximumLineLength,
                        maximumLineLength);
                    this.analyzer.SetSetting(
                        this.tabControl.LocalSettings,
                        newIntProperty);
                }
            }

            this.Dirty = false;

            return true;
        }

        /// <summary>
        /// Initializes this tab.
        /// </summary>
        /// <param name="propertyControl">The property control that hosts this
        /// tab.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="propertyControl"/> is <c>null</c>.</exception>
        public void Initialize(PropertyControl propertyControl)
        {
            Param.RequireNotNull(propertyControl, "propertyControl");

            // Save the property control.
            this.tabControl = propertyControl;

            // Load the current settings and initialize the controls on the
            // form.
            this.InitializeSettings();

            // Put the form into 'not-dirty' state.
            this.Dirty = false;
        }

        /// <summary>
        /// Called before <see cref="Apply()"/> is called.
        /// </summary>
        /// <param name="wasDirty">This parameter is unused.</param>
        /// <remarks>This method's implementation is empty.</remarks>
        public void PostApply(bool wasDirty)
        {
        }

        /// <summary>
        /// Called after <see cref="Apply()"/> was called.
        /// </summary>
        /// <returns>Always returns <c>true</c>.</returns>
        /// <remarks>This method only returns <c>true</c> and performs no other
        /// tasks.</remarks>
        public bool PreApply()
        {
            return true;
        }

        /// <summary>
        /// This method is unused.
        /// </summary>
        /// <remarks>This method's implementation is empty.</remarks>
        public void RefreshSettingsOverrideState()
        {
        }

        #endregion

        /// <summary>
        /// Loads the current settings and initializes the controls on the
        /// form.
        /// </summary>
        private void InitializeSettings()
        {
            var maximumLineLength = this.analyzer.GetValue<int>(
                this.tabControl.MergedSettings,
                Strings.MaximumLineLength);

            // Set the value of the property into an edit box on the page.
            this.maximumLineLengthTextBox.Text =
                maximumLineLength.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Handles the <see cref="TextBox.TextChanged"/> event of the <see
        /// cref="maximumLineLengthTextBox"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance
        /// containing the event data.</param>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            this.Dirty = true;
        }
    }
}
