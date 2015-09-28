﻿// <copyright file="LayoutRules.cs" company="n/a">
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
// <summary>Analyzer implementation for verifying layout rules.</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using StyleCop;
using StyleCop.CSharp;

namespace Community.StyleCop.CSharp
{
    /// <summary>
    /// Community rules for checking the layout of a file.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public class LayoutRules : SourceAnalyzer
    {
        /// <summary>
        /// Gets the settings pages to show on the settings dialog.
        /// </summary>
        /// <value>The settings pages.</value>
        public override ICollection<IPropertyControlPage> SettingsPages
        {
            get
            {
                return new IPropertyControlPage[]
                {
                    new LayoutSettingsPage(this)
                };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include generated code.
        /// </summary>
        /// <value><c>true</c> if generated code should be checked; otherwise,
        /// <c>false</c>.</value>
        private bool IncludeGenerated { get; set; }

        /// <summary>
        /// Analyzes the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="document"/> is <c>null</c>.</exception>
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");

            this.IncludeGenerated = this.GetValue<bool>(
                document.Settings,
                Strings.IncludeGenerated);

            var csDocument = (CsDocument)document;
            if (csDocument.RootElement != null &&
                (this.IncludeGenerated ||
                !csDocument.RootElement.Generated))
            {
                if (csDocument.Tokens != null && csDocument.Tokens.Count == 0)
                {
                    // early return if document is empty.
                    return;
                }

                // check start of document
                this.CheckIfFileStartsWithWhitespace(
                    csDocument.RootElement,
                    csDocument.Tokens.First.Value);

                // check whole document
                this.IterateDocumentTokens(csDocument);

                // check end of document
                this.CheckIfDocumentEndsWithMultipleWhitespaceLines(
                    csDocument.RootElement,
                    csDocument.Tokens.Last);
                this.CheckIfFileEndsWithNewline(
                    csDocument.RootElement,
                    csDocument.Tokens.Last.Value);
            }
        }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <typeparam name="T">Type of the property's value.</typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>If available, returns the configured value of the
        /// specified property; otherwise the property's default value is
        /// returned.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="settings"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref
        /// name="propertyName"/> is <c>null</c> or empty.</exception>
        internal T GetValue<T>(Settings settings, string propertyName)
        {
            Param.RequireNotNull(settings, "settings");
            Param.RequireValidString(propertyName, "propertyName");

            var propertyValue =
                this.GetSetting(settings, propertyName) as PropertyValue<T>;
            if (propertyValue != null)
            {
                return propertyValue.Value;
            }

            return this.GetDefaultValue<T>(propertyName);
        }

        /// <summary>
        /// Determines whether the specified string has trailing whitespace.
        /// </summary>
        /// <param name="s">The string to check for trailing whitespace.
        /// </param>
        /// <returns><c>true</c> if <paramref name="s"/> has trailing
        /// whitespace; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="s"/> is <c>null</c>.</exception>
        private static bool HasTrailingWhitespace(string s)
        {
            Param.RequireNotNull(s, "s");

            return s != s.TrimEnd(null);
        }

        /// <summary>
        /// Determines whether the specified token type occupies the rest of
        /// the line.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns><c>true</c> if the specified token occupies the rest of
        /// the line; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="token"/> is <c>null</c>.</exception>
        private static bool IsEatLineToken(CsToken token)
        {
            Param.RequireNotNull(token, "token");

            switch (token.CsTokenType)
            {
                case CsTokenType.PreprocessorDirective:
                case CsTokenType.SingleLineComment:
                case CsTokenType.XmlHeaderLine:
                    Debug.Assert(
                        token.Location.LineSpan == 1,
                        "Token unexpectedly spans multiple lines.");
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the default value of the specified property.
        /// </summary>
        /// <typeparam name="T">Type of the property's value.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The default value of the specified property.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref
        /// name="propertyName"/> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a property
        /// descriptor for <paramref name="propertyName"/> cannot be retrieved.
        /// </exception>
        private T GetDefaultValue<T>(string propertyName)
        {
            Param.RequireValidString(propertyName, "propertyName");

            var descriptor =
                this.PropertyDescriptors[propertyName]
                as PropertyDescriptor<T>;
            if (descriptor == null)
            {
                const string FormatString =
                    "Cannot retrieve descriptor for property '{0}'.";
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    FormatString,
                    propertyName);
                throw new InvalidOperationException(message);
            }

            return descriptor.DefaultValue;
        }

        /// <summary>
        /// Iterates through the document tokens.
        /// </summary>
        /// <param name="csDocument">The C# document.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="csDocument"/> is <c>null</c>.</exception>
        private void IterateDocumentTokens(CsDocument csDocument)
        {
            Param.RequireNotNull(csDocument, "csDocument");

            int maximumLineLength = this.GetValue<int>(
                csDocument.Settings,
                Strings.MaximumLineLength);
            CsToken previousToken = null;

            foreach (
                CsToken token in new ChildTokenCollection(csDocument.Tokens))
            {
                this.CheckForLinesWithTrailingWhitespace(
                    csDocument.RootElement,
                    token,
                    previousToken);
                this.CheckLineLength(
                    csDocument.RootElement,
                    token,
                    maximumLineLength);
                previousToken = token;
            }
        }

        /// <summary>
        /// Checks for lines with trailing whitespace.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="token">The current token.</param>
        /// <param name="previousToken">The previous token or <c>null</c> if
        /// there is no previous token.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/> or <paramref name="token"/> is <c>null</c>.
        /// </exception>
        private void CheckForLinesWithTrailingWhitespace(
            DocumentRoot rootElement,
            CsToken token,
            CsToken previousToken)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(token, "token");

            if (!this.IncludeGenerated && token.Generated)
            {
                return;
            }

            // simple case: whitespace followed by newline
            if (previousToken != null &&
                previousToken.CsTokenType == CsTokenType.WhiteSpace &&
                token.CsTokenType == CsTokenType.EndOfLine)
            {
                this.AddViolation(
                    rootElement,
                    token.LineNumber,
                    Rules.LinesMustNotEndWithWhitespace);
            }

            // check if a token which includes the rest of the line contains
            // trailing whitespace
            if (IsEatLineToken(token) && HasTrailingWhitespace(token.Text))
            {
                this.AddViolation(
                    rootElement,
                    token.LineNumber,
                    Rules.LinesMustNotEndWithWhitespace);
            }

            // multi-line comment (/* */): split by lines and check each line
            // for trailing whitespace
            if (token.CsTokenType == CsTokenType.MultiLineComment)
            {
                this.CheckMultiLineComment(
                    rootElement,
                    token,
                    (s, i) => HasTrailingWhitespace(s),
                    Rules.LinesMustNotEndWithWhitespace);
            }
        }

        /// <summary>
        /// Checks a multi-line comment.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="token">The token.</param>
        /// <param name="checkDelegate">The delegate that checks each comment
        /// line.</param>
        /// <param name="violatedRuleIfCheckFails">The rule that is violated if
        /// the check succeeds.</param>
        /// <param name="violationContext">Context to make the violation
        /// message more meaningful.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/>, <paramref name="token"/> or <paramref
        /// name="checkDelegate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the supplied token is
        /// not a multi-line comment.</exception>
        /// <remarks>This method does not check if the supplied token is
        /// generated or not. If generated code should not be checked, this has
        /// to be taken care of in the calling method.</remarks>
        private void CheckMultiLineComment(
            DocumentRoot rootElement,
            CsToken token,
            Func<string, int, bool> checkDelegate,
            Rules violatedRuleIfCheckFails,
            params object[] violationContext)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(token, "token");
            Param.RequireNotNull(checkDelegate, "checkDelegate");
            Param.Require(
                token.CsTokenType == CsTokenType.MultiLineComment,
                "token",
                "The supplied token is not a multi-line comment.");

            string[] newLine = new[] { Environment.NewLine };
            string[] splitComment =
                token.Text.Split(newLine, StringSplitOptions.None);
            for (int index = 0; index < splitComment.Length; index++)
            {
                if (checkDelegate(splitComment[index], index))
                {
                    this.AddViolation(
                        rootElement,
                        token.LineNumber + index,
                        violatedRuleIfCheckFails,
                        violationContext);
                }
            }
        }

        /// <summary>
        /// Checks length of the lines.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="token">The document token.</param>
        /// <param name="maximumLineLength">Maximum allowed length of a line.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/> or <paramref name="token"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref
        /// name="maximumLineLength"/> is not positive.</exception>
        private void CheckLineLength(
            DocumentRoot rootElement,
            CsToken token,
            int maximumLineLength)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(token, "token");
            Param.RequireGreaterThanZero(
                maximumLineLength,
                "maximumLineLength");

            if (!this.IncludeGenerated && token.Generated)
            {
                return;
            }

            if (token.CsTokenType == CsTokenType.MultiLineComment)
            {
                int startIndex = token.Location.StartPoint.IndexOnLine;
                Func<string, int, bool> checkDelegate = (s, i) =>
                    ((i == 0) ? startIndex : 0) + s.Length > maximumLineLength;
                this.CheckMultiLineComment(
                    rootElement,
                    token,
                    checkDelegate,
                    Rules.LinesMustNotBeLongerThanNumCharacters,
                    maximumLineLength);
            }
            else
            {
                Debug.Assert(
                    token.Location.LineSpan == 1,
                    "Token unexpectedly spans multiple lines.");

                int lineLength;
                if (token.CsTokenType == CsTokenType.EndOfLine)
                {
                    // don't count end-of-line character(s)
                    lineLength = token.Location.StartPoint.IndexOnLine - 1;
                }
                else
                {
                    lineLength = token.Location.EndPoint.IndexOnLine;
                }

                if (lineLength > maximumLineLength)
                {
                    this.AddViolation(
                        rootElement,
                        token.Location.EndPoint.LineNumber,
                        Rules.LinesMustNotBeLongerThanNumCharacters,
                        maximumLineLength);
                }
            }
        }

        /// <summary>
        /// Checks if the document ends with multiple whitespace lines.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="lastNode">The last node of the linked token list.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/> or <paramref name="lastNode"/> is <c>null</c>.
        /// </exception>
        private void CheckIfDocumentEndsWithMultipleWhitespaceLines(
            DocumentRoot rootElement,
            Node<CsToken> lastNode)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(lastNode, "lastNode");

            int? lastLineNumber = null;

            // Rewind through document
            for (Node<CsToken> node = lastNode;
                node != null;
                node = node.Previous)
            {
                // Stop on first occurence of non-whitespace token
                if (node.Value.CsTokenType != CsTokenType.WhiteSpace &&
                    node.Value.CsTokenType != CsTokenType.EndOfLine)
                {
                    return;
                }

                if (!this.IncludeGenerated && node.Value.Generated)
                {
                    return;
                }

                if (node.Value.CsTokenType == CsTokenType.EndOfLine)
                {
                    int currentLineNumber = node.Value.LineNumber;
                    if (!lastLineNumber.HasValue)
                    {
                        lastLineNumber = currentLineNumber;
                    }
                    else if (lastLineNumber - currentLineNumber > 0)
                    {
                        this.AddViolation(
                            rootElement,
                            lastLineNumber.Value,
                            Rules.FilesMustNotEndWithMultipleEmptyLines);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the file starts with whitespace.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="firstToken">The first document token.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/> or <paramref name="firstToken"/> is
        /// <c>null</c>.</exception>
        private void CheckIfFileStartsWithWhitespace(
            DocumentRoot rootElement,
            CsToken firstToken)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(firstToken, "firstToken");

            if (!this.IncludeGenerated && firstToken.Generated)
            {
                return;
            }

            if (firstToken.CsTokenType == CsTokenType.WhiteSpace ||
                firstToken.CsTokenType == CsTokenType.EndOfLine)
            {
                this.AddViolation(
                    rootElement,
                    firstToken.LineNumber,
                    Rules.FilesMustNotStartWithWhitespace);
            }
        }

        /// <summary>
        /// Checks if the file ends with newline.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="lastToken">The last document token.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="rootElement"/> or <paramref name="lastToken"/> is
        /// <c>null</c>.</exception>
        private void CheckIfFileEndsWithNewline(
            DocumentRoot rootElement,
            CsToken lastToken)
        {
            Param.RequireNotNull(rootElement, "rootElement");
            Param.RequireNotNull(lastToken, "lastToken");

            if (!this.IncludeGenerated && lastToken.Generated)
            {
                return;
            }

            if (lastToken.CsTokenType != CsTokenType.EndOfLine)
            {
                this.AddViolation(
                    rootElement,
                    lastToken.LineNumber,
                    Rules.FilesMustEndWithNewline);
            }
        }
    }
}
